using System.Security.Claims;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Payment;
using TicketHub.Services.IService;
using TicketHub.Utility.Payment;
using Transaction = TicketHub.Models.Domain.Transaction;

namespace TicketHub.Services.Service;

public class PaymentService : IPaymentService
//: IPaymentService
{
    private readonly IMapper _mapper;
    private readonly PayOS _payOS;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IConfiguration configuration, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _payOS = new PayOS(
            configuration["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find PAYOS_CLIENT_ID"),
            configuration["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find PAYOS_API_KEY"),
            configuration["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find PAYOS_CHECKSUM_KEY")
        );
    }

    public async Task<ResponseDto> CreatePayOsPaymentLink(ClaimsPrincipal User, CreatePaymentLinkDto createPaymentLink)
    {
        try
        {
            // 🔹 Lấy đơn hàng dựa trên OrderNumber
            var order = await _unitOfWork.OrderRepository.GetOrderByOrderNumber(createPaymentLink.OrderNumber);
            if (order is null)
                return new ResponseDto
                {
                    Message = "Order does not exist",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };

            // 🔹 Lấy danh sách TicketTemplateId từ OrderDetail
            var orderDetails =
                await _unitOfWork.OrderDetailRepository.GetListAsync(od => od.OrderId == order.OrderId,
                    "TicketTemplate");
            if (orderDetails == null || !orderDetails.Any())
                return new ResponseDto
                {
                    Message = "No tickets found for this order",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };

            // 🔹 Nhóm vé theo `TicketTemplate` và tính tổng tiền
            var groupedTickets = orderDetails
                .GroupBy(od => od.TicketTemplate!.TicketName)
                .Select(g => new ItemData(
                    g.Key,
                    g.Sum(od => od.Quantity),
                    Convert.ToInt32(g.First().TicketTemplate!.TicketPrice)
                ))
                .ToList();

            // 🔹 Kiểm tra nếu không có vé hợp lệ
            if (!groupedTickets.Any())
                return new ResponseDto
                {
                    Message = "No valid tickets found for payment",
                    IsSuccess = false,
                    StatusCode = 400,
                    Result = null
                };

            // 🔹 Tính tổng tiền
            var totalPrice = groupedTickets.Sum(i => i.price * i.quantity);

            // 🔹 Kiểm tra tổng tiền hợp lệ
            if (totalPrice <= 0)
                return new ResponseDto
                {
                    Message = "Total price must be greater than 0",
                    IsSuccess = false,
                    StatusCode = 400,
                    Result = null
                };

            // 🔹 Tạo dữ liệu thanh toán
            var paymentData = new PaymentData(
                createPaymentLink.OrderNumber,
                totalPrice,
                "Payment for ticket(s)",
                groupedTickets,
                createPaymentLink.CancelUrl,
                createPaymentLink.ReturnUrl
            );

            var result = await _payOS.createPaymentLink(paymentData);

            // 🔹 Lưu thông tin thanh toán vào database
            var payment = new Payment
            {
                OrderNumber = createPaymentLink.OrderNumber,
                Amount = result.amount,
                Description = result.description.Trim(),
                CancelUrl = paymentData.cancelUrl,
                ReturnUrl = paymentData.returnUrl,
                CreatedAt = DateTime.Now,
                Status = StaticPayment.paymentStatusPending
            };

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveAsync();

            return new ResponseDto
            {
                Message = "Create payment link successfully",
                IsSuccess = true,
                StatusCode = 200,
                Result = new
                {
                    result,
                    payment.PaymentTransactionId
                }
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Message = "An error occurred: " + e.Message,
                IsSuccess = false,
                StatusCode = 500,
                Result = null
            };
        }
    }

    public async Task<ResponseDto> ConfirmPayOsTransaction(ConfirmPayment confirmPayment)
    {
        try
        {
            // 🔹 Lấy thông tin đơn hàng
            var order = await _unitOfWork.OrderRepository.GetOrderByOrderNumber(confirmPayment.orderNumber);
            if (order is null)
                return new ResponseDto
                {
                    Message = "Order does not exist",
                    IsSuccess = false,
                    StatusCode = 404
                };

            if (order.Status == StaticPayment.paymentStatusSucess)
                return new ResponseDto
                {
                    Message = "Order has already been completed",
                    IsSuccess = false,
                    StatusCode = 400
                };

            // 🔹 Kiểm tra trạng thái thanh toán từ PayOS
            var transactionInfo = await _payOS.getPaymentLinkInformation(confirmPayment.orderNumber);
            if (transactionInfo == null || transactionInfo.status != StaticPayment.paymentStatusSucess)
                return new ResponseDto
                {
                    Message = "Transaction not found or not successful",
                    IsSuccess = false,
                    StatusCode = 400
                };

            // 🔹 Cập nhật trạng thái thanh toán
            var payment = await _unitOfWork.PaymentRepository.GetPaymentByOrderNumber(confirmPayment.orderNumber);
            if (payment == null)
                return new ResponseDto
                {
                    Message = "Payment record not found",
                    IsSuccess = false,
                    StatusCode = 404
                };

            payment.Status = StaticPayment.paymentStatusSucess;
            _unitOfWork.PaymentRepository.Update(payment);

            // 🔹 Lấy danh sách vé đã mua từ OrderDetails
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetListByOrderIdAsync(order.OrderId);

            var ticketList = new List<Ticket>();
            var ticketTemplates = new List<TicketTemplate>();

            foreach (var orderDetail in orderDetails)
            {
                // 🔹 Lấy TicketTemplate dựa vào TicketTemplateId trong OrderDetail
                var ticketTemplate =
                    await _unitOfWork.TicketTemplateRepository.GetAsync(tt =>
                        tt.TicketTemplateId == orderDetail.TicketTemplateId);

                if (ticketTemplate == null)
                    return new ResponseDto
                    {
                        Message = "TicketTemplate not found for OrderDetail",
                        StatusCode = 400,
                        IsSuccess = false
                    };

                // 🔹 Kiểm tra nếu số vé còn lại đủ để cấp phát
                if (ticketTemplate.AvailableQuantity < orderDetail.Quantity)
                    return new ResponseDto
                    {
                        Message = $"Not enough tickets available for {ticketTemplate.TicketName}!",
                        StatusCode = 400,
                        IsSuccess = false
                    };

                // 🔹 Giảm số lượng vé còn lại
                ticketTemplate.AvailableQuantity -= orderDetail.Quantity;

                // 🔹 Tạo số lượng vé tương ứng với số lượng đã đặt
                for (var i = 0; i < orderDetail.Quantity; i++)
                    ticketList.Add(new Ticket
                    {
                        TicketId = Guid.NewGuid(),
                        CustomerId = order.CustomerId,
                        TicketTemplateId = ticketTemplate.TicketTemplateId, // ✅ Lấy từ TicketTemplate
                        Status = TicketStatus.Success,
                        TicketDescription =
                            "Ticket for event: " + ticketTemplate.TicketName, // ✅ Dùng TicketTemplate.TicketName
                        IsFromExternal = false,
                        IsVisible = true
                    });
            }

            // 🔹 Lưu tất cả Tickets vào database
            await _unitOfWork.TicketRepository.AddRangeAsync(ticketList);

            // 🔹 Cập nhật số lượng vé còn lại trong TicketTemplate
            _unitOfWork.TicketTemplateRepository.UpdateRange(ticketTemplates);

            // 🔹 Cập nhật trạng thái đơn hàng
            order.Status = StaticPayment.paymentStatusSucess;
            _unitOfWork.OrderRepository.Update(order);

            // 🔹 Tạo và lưu transaction vào database
            var transaction = new Transaction
            {
                CustomerId = order.CustomerId,
                OrderId = order.OrderId,
                PaymentId = confirmPayment.paymentTransactionId,
                Amount = payment.Amount,
                TransactionMethod = "Transfer",
                TransactionDateTime = DateTime.UtcNow,
                Status = StaticPayment.paymentStatusSucess
            };

            await _unitOfWork.TransactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveAsync();

            return new ResponseDto
            {
                Message = "Payment confirmed! Tickets generated and assigned successfully.",
                IsSuccess = true,
                StatusCode = 200,
                Result = new
                {
                    TransactionId = transaction.PaymentId,
                    PaymentStatus = payment.Status,
                    TicketsAssigned = ticketList.Count,
                    PayOS_ConfirmUrl = transactionInfo.transactions
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto
            {
                Message = $"An error occurred: {ex.Message}",
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
}