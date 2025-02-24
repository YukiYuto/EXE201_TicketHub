using AutoMapper;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO.Order;
using TicketHub.Models.DTO.Ticket;

namespace TicketHub.Services.Mapping;

public class AutoMapperProfile : Profile
{

    public AutoMapperProfile()
    {
        CreateMap<Orders, GetOrderDto>().ReverseMap();
    }

}