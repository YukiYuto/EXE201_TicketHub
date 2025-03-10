using Microsoft.EntityFrameworkCore;
using TicketHub.DataAccess.Context;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Repository;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Payment> GetPaymentByOrderNumber(long orderNumber)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
    }

    public async Task<(List<Payment> Payments, int TotalPayments)> GetPaymentsAsync
    (
        int pageNumber,
        int pageSize,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        Guid? customerId = null
    )
    {
        var query = _context.Payments.AsQueryable();

        // 🔹 Lọc theo CustomerId nếu được cung cấp
        if (customerId.HasValue) query = query.Where(p => p.Orders != null && p.Orders.CustomerId == customerId);

        // 🔹 Lọc theo cột cụ thể (ví dụ: trạng thái thanh toán)
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            switch (filterOn.ToLower())
            {
                case "status":
                    query = query.Where(p => p.Status != null && p.Status.Contains(filterQuery));
                    break;
                case "amount":
                    if (int.TryParse(filterQuery, out var amount)) query = query.Where(p => p.Amount == amount);
                    break;
            }

        // 🔹 Sắp xếp kết quả
        query = sortBy?.ToLower() switch
        {
            "amount" => query.OrderBy(p => p.Amount),
            "amount_desc" => query.OrderByDescending(p => p.Amount),
            "createdat" => query.OrderBy(p => p.CreatedAt),
            "createdat_desc" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt) // Mặc định sắp xếp theo ngày tạo mới nhất
        };

        // 🔹 Tính tổng số thanh toán trước khi phân trang
        var totalPayments = await query.CountAsync();

        // 🔹 Phân trang kết quả
        var payments = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (payments, totalPayments);
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }

    public async Task<Payment> GetPaymentTransactionIdByOrderNumber(Guid paymentTransactionId)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.PaymentTransactionId == paymentTransactionId);
    }
}