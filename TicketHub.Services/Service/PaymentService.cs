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

    public async Task<ResponseDto> CreatePayOsPaymentLink(ClaimsPrincipal User, CreatePaymentLinkDto createPaymentLink)
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

    public async Task<ResponseDto> UpdatePayOsPaymentStatus(ClaimsPrincipal User, Guid paymentTransactionId)
    {
        /*try
        {
            PaymentTransactions paymentTransactions =
                await _unitOfWork.PaymentTransactionsRepository.GetById(paymentTransactionId);
            if (paymentTransactions is null)
            {
                return new ResponseDto()
                {
                    Message = "Payment transaction is not exist",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };
            }

            long orderNumber = (long)paymentTransactions.AppointmentNumber;
            var paymentStatus = _payOS.getPaymentLinkInformation(orderNumber);

            if (paymentStatus != null)
            {
                paymentTransactions.Status = paymentStatus.Result.status;
            }

            _unitOfWork.PaymentTransactionsRepository.Update(paymentTransactions);
            await _unitOfWork.SaveAsync();

            if (paymentTransactions.Status.Equals(StaticPayment.paymentStatusSucess))
            {
                var appointment = await _unitOfWork.AppointmentRepository.GetAppointmentByAppmointNumer(orderNumber);
                Transaction transaction = new Transaction()
                {
                    CustomerId = appointment.CustomerId,
                    AppointmentId = appointment.AppointmentId,
                    PaymentTransactionId = paymentTransactionId,
                    Amount = paymentTransactions.Amount,
                    TransactionMethod = "Tranfer",
                    TransactionDateTime = DateTime.Now
                };

                await _unitOfWork.TransactionsRepository.AddAsync(transaction);
                await _unitOfWork.SaveAsync();

                appointment.BookingStatus = 1;
                _unitOfWork.AppointmentRepository.Update(appointment);
                await _unitOfWork.SaveAsync();
            }*/

        return new ResponseDto()
        {
            Message = "Update status successfully",
            IsSuccess = true,
            StatusCode = 200,
            //Result = paymentStatus.Result
        };
        /*}
        catch (Exception e)
        {
            return new ResponseDto()
            {
                Message = e.Message,
                IsSuccess = false,
                StatusCode = 500,
                Result = null
            };
        }*/
    }

    public Task<ResponseDto> CancelPayOsPaymentLink(ClaimsPrincipal User, Guid paymentTransactionId,
        string cancellationReason)
    {
        throw new NotImplementedException();
    }
}