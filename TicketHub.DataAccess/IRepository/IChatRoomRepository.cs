using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IChatRoomRepository : IRepository<ChatRoom>
{
    void Update(ChatRoom chatRoom);
    void UpdateRange(IEnumerable<ChatRoom> chatRooms);
    Task<ChatRoom> GetById(Guid chatRoomId);
}