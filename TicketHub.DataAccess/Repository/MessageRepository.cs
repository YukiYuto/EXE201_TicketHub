using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<Message> GetById(Guid messageId)
    {
        return await _context.Messages.FirstOrDefaultAsync(x => x.MessageId == messageId);
    }

    public void Update(Message message)
    {
        _context.Messages.Update(message);
    }

    public void UpdateRange(IEnumerable<Message> messages)
    {
        _context.Messages.UpdateRange(messages);
    }
}