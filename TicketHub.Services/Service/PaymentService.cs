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
    private readonly PayOS _payOS;
    private readonly IMapper _mapper;
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

    /*public async Task<ResponseDto> CreatePayOsPaymentLink(ClaimsPrincipal User, CreatePaymentLinkDto createPaymentLink)
    {
        try
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByOrderNumber(createPaymentLink.OrderNumber);
            if (order is null)
            {
                return new ResponseDto()
                {
                    Message = "Order does not exist",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };
            }

            // 🔹 Lấy danh sách TicketId từ OrderTicketRepository
            var ticketIds = await _unitOfWork.OrderTicketRepository.GetTicketIdsByOrderId(order.OrderId);
            if (ticketIds == null || !ticketIds.Any())
            {
                return new ResponseDto()
                {
                    Message = "No tickets found for this order",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };
            }

            // 🔹 Lấy danh sách Ticket từ TicketRepository
            var tickets = await _unitOfWork.TicketRepository.GetListAsync(t => ticketIds.Contains(t.TicketId));
            if (tickets == null || !tickets.Any())
            {
                return new ResponseDto()
                {
                    Message = "No valid tickets found",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };
            }

            // 🔹 Nhóm vé theo loại (nếu cùng loại thì cộng dồn số lượng)
            var groupedTickets = tickets
                .GroupBy(t => t.TicketName)
                .Select(g => new ItemData(
                    name: g.Key, // Tên vé
                    quantity: g.Count(), // Số lượng vé cùng loại
                    price: Convert.ToInt32(g.First().TicketPrice) // Giá 1 vé
                ))
                .ToList();

            // 🔹 Tính tổng tiền
            var totalPrice = groupedTickets.Sum(i => i.price * i.quantity);

            // 🔹 Tạo dữ liệu thanh toán
            var paymentData = new PaymentData(
                orderCode: createPaymentLink.OrderNumber,
                amount: totalPrice,
                description: "Payment for ticket(s)",
                items: groupedTickets, // Danh sách vé theo nhóm
                cancelUrl: createPaymentLink.CancelUrl,
                returnUrl: createPaymentLink.ReturnUrl
            );

            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);

            // 🔹 Lưu thông tin thanh toán vào database
            Payment payment = new Payment()
            {
                OrderNumber = createPaymentLink.OrderNumber,
                Amount = result.amount,
                Description = result.description.Trim(),
                CancelUrl = paymentData.cancelUrl,
                ReturnUrl = paymentData.returnUrl,
                ExpiredAt = paymentData.expiredAt,
                Signature = paymentData.signature,
                CreatedAt = DateTime.Now,
                Status = StaticPayment.paymentStatusPending
            };

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveAsync();

            return new ResponseDto()
            {
                Message = "Create payment link successfully",
                IsSuccess = true,
                StatusCode = 200,
                Result = new
                {
                    result,
                    PaymentTransactionId = payment.PaymentTransactionId
                }
            };
        }
        catch (Exception e)
        {
            return new ResponseDto()
            {
                Message = e.Message,
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
        var order = await _unitOfWork.OrderRepository.GetOrderByOrderNumber(confirmPayment.orderNumber);
        if (order is null)
        {
            return new ResponseDto()
            {
                Message = "Order does not exist",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };
        }

        // 🔹 Kiểm tra giao dịch thanh toán trên PayOS
        PaymentLinkInformation transactionInfo = await _payOS.getPaymentLinkInformation(confirmPayment.orderNumber);
        if (transactionInfo == null || transactionInfo.status != StaticPayment.paymentStatusSucess)
        {
            return new ResponseDto()
            {
                Message = "Transaction not found or not successful",
                IsSuccess = false,
                StatusCode = 400,
                Result = null
            };
        }

        // 🔹 Cập nhật trạng thái thanh toán
        var payment = await _unitOfWork.PaymentRepository.GetPaymentByOrderNumber(confirmPayment.orderNumber);
        if (payment == null)
        {
            return new ResponseDto()
            {
                Message = "Payment record not found",
                IsSuccess = false,
                StatusCode = 404,
                Result = null
            };
        }

        payment.Status = StaticPayment.paymentStatusSucess;
        _unitOfWork.PaymentRepository.Update(payment);

        // 🔹 Lấy danh sách vé thuộc đơn hàng
        var orderTickets = await _unitOfWork.OrderTicketRepository.GetTicketsByOrderId(order.OrderId);
        var ticketIds = orderTickets.Select(ot => ot.TicketId).ToList();

        // 🔹 Cập nhật chủ sở hữu vé
        var tickets = await _unitOfWork.TicketRepository.GetListAsync(t => ticketIds.Contains(t.TicketId));
        foreach (var ticket in tickets)
        {
            ticket.UserId = order.UserId; // Chuyển quyền sở hữu sang người mua
        }

        _unitOfWork.TicketRepository.UpdateRange(tickets);

        // 🔹 Lưu transaction vào database
        var transaction = new Transaction()
        {
            CustomerId = order.UserId,
            OrderId = order.OrderId,
            PaymentId = confirmPayment.paymentTransactionId,
            Amount = payment.Amount,
            TransactionMethod = "Tranfer",
            TransactionDateTime = DateTime.UtcNow
        };

        await _unitOfWork.TransactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveAsync();

        return new ResponseDto()
        {
            Message = "Payment confirmed successfully, tickets ownership updated, and transaction saved",
            IsSuccess = true,
            StatusCode = 200,
            Result = new
            {
                TransactionId = transaction.PaymentId,
                Status = payment.Status,
                PayOS_ConfirmUrl = transactionInfo.transactions // 🔹 Link xác nhận của PayOS
            }
        };
    }
    catch (Exception ex)
    {
        return new ResponseDto()
        {
            Message = ex.Message,
            IsSuccess = false,
            StatusCode = 500,
            Result = null
        };
    }
}

    public Task<ResponseDto> CancelPayOsPaymentLink(ClaimsPrincipal User, Guid paymentTransactionId,
        string cancellationReason)
    {
        throw new NotImplementedException();
    }*/
}