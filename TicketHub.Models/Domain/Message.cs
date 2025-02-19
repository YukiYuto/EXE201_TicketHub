using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Message
{
    [Key]
    public Guid MessageId { get; set; }
    [StringLength(200)] 
    public string MessageContent { get; set; } = null!;
    public Guid SendMessageUserId { get; set; }  
    public Guid ReceiveMessageUserId { get; set; }
    public DateTime CreateTime { get; set; }
    public Guid? ChatRoomId { get; set; }
    [ForeignKey("ChatRoomId")] 
    public virtual ChatRoom ChatRoom { get; set; } = null!;
}