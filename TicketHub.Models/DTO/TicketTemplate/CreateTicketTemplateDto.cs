namespace TicketHub.Models.DTO.TicketTemplate;

public class CreateTicketTemplateDto
{
    public List<TicketTemplateDto> TicketTemplates { get; set; } = new();
}