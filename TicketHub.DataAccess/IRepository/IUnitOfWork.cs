namespace TicketHub.DataAccess.IRepository;

public interface IUnitOfWork
{
    IEventRepository EventRepository { get; }
    Task<int> SaveAsync();
}