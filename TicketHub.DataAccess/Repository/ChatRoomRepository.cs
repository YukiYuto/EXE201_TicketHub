using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class ChatRoomRepository : Repository<ChatRoom>, IChatRoomRepository
{
    private readonly ApplicationDbContext _context;
    
    public ChatRoomRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    
    public void Update(ChatRoom chatRoom)
    {
        _context.ChatRooms.Update(chatRoom);
    }

    public void UpdateRange(IEnumerable<ChatRoom> chatRooms)
    {
        _context.ChatRooms.UpdateRange(chatRooms);
    }

    public async Task<ChatRoom> GetById(Guid chatRoomId)
    {
        return await _context.ChatRooms.FirstOrDefaultAsync(x => x.ChatRoomId == chatRoomId);
    }
}