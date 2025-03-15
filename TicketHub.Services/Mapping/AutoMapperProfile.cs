using AutoMapper;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO.Event;
using TicketHub.Models.DTO.Order;
using TicketHub.Models.DTO.Payment;
using TicketHub.Models.DTO.Ticket;
using TicketHub.Models.DTO.TicketSerialNumber;
using TicketHub.Models.DTO.TicketTemplate;
using TicketHub.Models.DTO.Transaction;

namespace TicketHub.Services.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<TicketSerialNumber, CreateTicketSerialNumberDto>()
            .ForMember(dest => dest.TicketTemplateId, opt => opt.MapFrom(src => src.TicketTemplateId))
            .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.SerialNumber))
            .ReverseMap();
        CreateMap<TicketSerialNumber, GetTicketSerialNumberDto>().ReverseMap();
        CreateMap<TicketSerialNumber, UpdateTicketSerialNumberDto>().ReverseMap();
        CreateMap<TicketTemplate, GetTicketTemplateDto>().ReverseMap();
        CreateMap<Orders, GetOrderDto>().ReverseMap();
        CreateMap<Event, GetEventDto>();
        CreateMap<Ticket, GetTicketDto>()
            // Map các trường trực tiếp
            .ForMember(dest => dest.SerialNumberId, opt => opt.MapFrom(src => src.SerialNumberId ?? Guid.Empty))
            // Map từ TicketTemplate
            .ForMember(dest => dest.TicketName, opt => opt.MapFrom(src => src.TicketTemplate.TicketName))
            .ForMember(dest => dest.TicketPrice, opt => opt.MapFrom(src => src.TicketTemplate.TicketPrice))
            .ForMember(dest => dest.TicketImage, opt => opt.MapFrom(src => src.TicketTemplate.ImageTicket))
            .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.TicketTemplate.Rank))
            // Map từ Event
            .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.TicketTemplate.Event.EventId))
            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.TicketTemplate.Event.EventName))
            .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.TicketTemplate.Event.EventDate))
            // Map từ Category
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.TicketTemplate.Event.Category.CategoryId))
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.TicketTemplate.Event.Category.CategoryName))
            // Map từ SerialNumber
            .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.TicketSerialNumber.SerialNumber));
        ;
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