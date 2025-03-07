namespace TicketHub.Models.DTO.Message;

public class UpdateMessageDto
{
    public Guid MessageId { get; set; }
    public string MessageContent { get; set; } = null!;
    public Guid SendMessageUserId { get; set; }  
    public Guid ReceiveMessageUserId { get; set; }
    public DateTime CreateTime { get; set; }
    public Guid? ChatRoomId { get; set; }
}