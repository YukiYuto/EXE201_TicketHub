using AutoMapper;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO.Event;
using TicketHub.Models.DTO.Order;
using TicketHub.Models.DTO.Payment;
using TicketHub.Models.DTO.Ticket;
using TicketHub.Models.DTO.Transaction;

namespace TicketHub.Services.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Orders, GetOrderDto>().ReverseMap();
        CreateMap<Event, GetEventDto>();
        CreateMap<Ticket, GetTicketDto>().ReverseMap();
        CreateMap<Payment, PaymentDto>().ReverseMap();
        CreateMap<Transaction, GetAllTransactionByCustomerIdDto>()
            .ForMember(dest => dest.Transaction, opt => opt.MapFrom(src => src.TransactionId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.TransactionDateTime, opt => opt.MapFrom(src => src.TransactionDateTime))
            .ForMember(dest => dest.TransactionMethod, opt => opt.MapFrom(src => src.TransactionMethod))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
    }
}