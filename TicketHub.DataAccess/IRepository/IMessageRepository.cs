using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.IRepository;

public interface IMessageRepository : IRepository<Message>
{
    void Update(Message message);
    void UpdateRange(IEnumerable<Message> messages);
    Task<Message> GetById(Guid messageId);
}