using System.Security.Claims;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Payment;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class PaymentService 
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
            var order =
                await _unitOfWork.OrderRepository.GetAppointmentByOrderNumber((createPaymentLink
                    .OrderNumber));
            if (order is null)
            {
                return new ResponseDto()
                {
                    Message = "order is not exist",
                    IsSuccess = false,
                    StatusCode = 404,
                    Result = null
                };
            }

            var totalPrice = Convert.ToInt32(order.TotalPrice);
            var ticket = await _unitOfWork.TicketRepository.GeTicketById();
            var items = new List<ItemData>()
            {
                new ItemData(name: service.ServiceName, quantity: 1, price: totalPrice)
            };


            var paymentData = new PaymentData(
                orderCode: createPaymentLink.AppointmentNumber,
                amount: totalPrice,
                description: "",
                items: items,
                cancelUrl: createPaymentLink.CancelUrl,
                returnUrl: createPaymentLink.ReturnUrl
            );
            if (paymentData is null)
            {
                return new ResponseDto()
                {
                    Message = "Payment is missing data",
                    IsSuccess = false,
                    StatusCode = 500,
                    Result = null
                };
            }

            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);

            PaymentTransactions paymentTransactions = new PaymentTransactions()
            {
                AppointmentNumber = createPaymentLink.AppointmentNumber,
                Amount = result.amount,
                Description = result.description.Trim(),
                CancelUrl = paymentData.cancelUrl,
                ReturnUrl = paymentData.returnUrl,
                ExpiredAt = paymentData.expiredAt,
                Signature = paymentData.signature,
                CreatedAt = DateTime.Now,
                Status = StaticPayment.paymentStatusDefault
            };

            await _unitOfWork.PaymentTransactionsRepository.AddAsync(paymentTransactions);
            await _unitOfWork.SaveAsync();
            return new ResponseDto()
            {
                Message = "Create payment link successfully",
                IsSuccess = true,
                StatusCode = 200,
                Result = new
                {
                    result,
                    PaymentTransactionId = paymentTransactions.PaymentTransactionId
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

    public Task<ResponseDto> UpdatePayOsPaymentStatus(ClaimsPrincipal User, Guid paymentTransactionId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> CancelPayOsPaymentLink(ClaimsPrincipal User, Guid paymentTransactionId,
        string cancellationReason)
    {
        throw new NotImplementedException();
    }*/
}