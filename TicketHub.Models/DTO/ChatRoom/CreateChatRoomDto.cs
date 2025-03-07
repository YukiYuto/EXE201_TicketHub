namespace TicketHub.Models.DTO.ChatRoom;

public class CreateChatRoomDto
{
    public Guid ChatRoomId { get; set; }
    public string NameRoom { get; set; } = null!;
    public string SendMessageUserId { get; set; }
    public string ReceiveMessageUserId { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}