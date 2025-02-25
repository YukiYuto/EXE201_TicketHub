using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class ChatRoom
{
    [Key]
    public Guid ChatRoomId { get; set; }
    [StringLength(20)]
    public string NameRoom { get; set; } = null!;
    public string SendMessageUserId { get; set; } = null!;
    [ForeignKey("SendMessageUserId")] public virtual required ApplicationUser SendMessageUser { get; set; }
    public string ReceiveMessageUserId { get; set; } = null!;
    [ForeignKey("ReceiveMessageUserId")] public virtual required ApplicationUser ReceiveMessageUser { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}