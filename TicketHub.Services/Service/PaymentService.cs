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
            // Lấy đơn hàng theo OrderNumber
            var order = await _unitOfWork.OrderRepository.GetAppointmentByOrderNumber(createPaymentLink.OrderNumber);
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

            //  Lấy danh sách TicketId từ bảng quan hệ OrderTickets
            var ticketIds = order.OrderTickets?.Select(ot => ot.TicketId).ToList();
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

            // Lấy danh sách vé từ TicketRepository dựa vào danh sách TicketId
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

            // Tạo danh sách items từ danh sách vé
            var items = tickets.Select(ticket => new ItemData(
                name: ticket.TicketName,
                quantity: 1,
                price: Convert.ToInt32(ticket.TicketPrice)
            )).ToList();

            // Tính tổng giá trị đơn hàng dựa trên danh sách vé
            var totalPrice = items.Sum(i => i.price);

            //  Tạo dữ liệu thanh toán
            var paymentData = new PaymentData(
                orderCode: createPaymentLink.OrderNumber,
                amount: totalPrice,
                description: "Payment for tickets",
                items: items,
                cancelUrl: createPaymentLink.CancelUrl,
                returnUrl: createPaymentLink.ReturnUrl
            );

            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);

            //  Lưu thông tin thanh toán vào database
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