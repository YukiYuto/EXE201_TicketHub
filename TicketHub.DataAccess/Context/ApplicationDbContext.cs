using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketHub.Models.Domain;

namespace TicketHub.DataAccess.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Organizer> Organizers { get; set; }

    //public DbSet<Feedback> Feedbacks { get; set; }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<Ticket> Tickets { get; set; }

    //public DbSet<TicketTransfers> TicketTransfers { get; set; }
    public DbSet<TicketTemplate> TicketTemplates { get; set; }
    public DbSet<ResaleListing> ResaleListings { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }

    public DbSet<Orders> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    //public DbSet<ChatRoom> ChatRooms { get; set; }
    //public DbSet<Message> Messages { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /*// Seed data
        ApplicationDbContextSeed.SeedAdminAccount(modelBuilder);


        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.NoAction); // Ngăn chặn xóa cascade gây lỗi

        /*modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Ticket)
            .WithMany() // Nếu không có navigation property ngược, để trống
            .HasForeignKey(ci => ci.TicketId)
            .OnDelete(DeleteBehavior.NoAction);#1# // Ngăn chặn xóa cascade gây lỗi
        /*modelBuilder.Entity<CartItem>()
            .HasOne(c => c.)
            .WithOne(t => t.Seller)
            .HasForeignKey(t => t.SellerId)
            .OnDelete(DeleteBehavior.NoAction);#1#
        /*
        modelBuilder.Entity<CartItem>()
            .HasKey(ci => new { ci.CartId, ci.TicketId });

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Ticket)
            .WithMany(t => t.CartItems)
            .HasForeignKey(ci => ci.TicketId)
            .OnDelete(DeleteBehavior.NoAction);
            #1#


        /*
        modelBuilder.Entity<OrderTicket>()
            .HasKey(ot => new { ot.OrderId, ot.TicketId });

        modelBuilder.Entity<OrderTicket>()
            .HasOne(ot => ot.Orders)
            .WithMany(o => o.OrderTickets)
            .HasForeignKey(ot => ot.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderTicket>()
            .HasOne(ot => ot.Ticket)
            .WithMany(t => t.OrderTickets)
            .HasForeignKey(ot => ot.TicketId)
            .OnDelete(DeleteBehavior.NoAction);
            #1#


        // Configure TicketTransfer's foreign keys to avoid multiple cascade paths
        /*modelBuilder.Entity<TicketTransfers>()
            .HasOne(tt => tt.Seller)
            .WithMany() // One-to-many relationship with ApplicationUser
            .HasForeignKey(tt => tt.SellerId)
            .OnDelete(DeleteBehavior.NoAction); // Set to NoAction to avoid cascade delete conflicts

        modelBuilder.Entity<TicketTransfers>()
            .HasOne(tt => tt.Buyer)
            .WithMany() // One-to-many relationship with ApplicationUser
            .HasForeignKey(tt => tt.BuyerId)
            .OnDelete(DeleteBehavior.NoAction); // Set to NoAction to avoid cascade delete conflicts

        modelBuilder.Entity<TicketTransfers>()
            .HasOne(tt => tt.Ticket)
            .WithMany() // One-to-many relationship with Ticket
            .HasForeignKey(tt => tt.TicketId)
            .OnDelete(DeleteBehavior.Cascade); // Allow cascading delete for Ticket when deleted


        modelBuilder.Entity<ChatRoom>()
            .HasOne(c => c.SendMessageUser)
            .WithMany()
            .HasForeignKey(c => c.SendMessageUserId)
            .OnDelete(DeleteBehavior.Restrict); // Or NoAction, depending on your needs

        modelBuilder.Entity<ChatRoom>()
            .HasOne(c => c.ReceiveMessageUser)
            .WithMany()
            .HasForeignKey(c => c.ReceiveMessageUserId)
            .OnDelete(DeleteBehavior.Restrict); // Avoid cascade#1#

        //OrderNumber is unique
        modelBuilder.Entity<Orders>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Orders)
            .WithMany()
            .HasForeignKey(p => p.OrderNumber)
            .HasPrincipalKey(o => o.OrderNumber)
            .OnDelete(DeleteBehavior.Restrict);

        /*modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Customer)
            .WithMany()
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Cascade); // Giữ 1 cái CASCADE

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Order)
            .WithMany()
            .HasForeignKey(t => t.OrderNumber)
            .OnDelete(DeleteBehavior.NoAction); // Đổi thành NoAction#1#

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Payment)
            .WithMany()
            .HasForeignKey(t => t.PaymentId)
            .OnDelete(DeleteBehavior.NoAction); // Đổi thành NoAction*/

        // Đảm bảo OrderNumber là unique
        modelBuilder.Entity<Orders>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();

        // Quan hệ giữa Cart và CartItem
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.TicketTemplate)
            .WithMany(tt => tt.CartItems)
            .HasForeignKey(ci => ci.TicketTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        //  Quan hệ giữa Order và OrderDetail
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Orders)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Ticket)
            .WithMany(t => t.OrderDetails)
            .HasForeignKey(od => od.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Quan hệ giữa Customer và Ticket
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Customer)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        //  Quan hệ giữa Organizer và Event
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Category)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Event>()
            .HasMany(e => e.TicketTemplates)
            .WithOne(tt => tt.Event)
            .HasForeignKey(tt => tt.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        //  Quan hệ giữa Ticket và ResaleListing
        modelBuilder.Entity<ResaleListing>()
            .HasOne(rl => rl.Ticket)
            .WithMany(t => t.ResaleListings)
            .HasForeignKey(rl => rl.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ResaleListing>()
            .HasOne(rl => rl.Customer)
            .WithMany(c => c.ResaleListings)
            .HasForeignKey(rl => rl.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        //  Quan hệ giữa Order, Payment, và Transaction
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Orders)
            .WithMany()
            .HasForeignKey(p => p.OrderNumber)
            .HasPrincipalKey(o => o.OrderNumber)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Orders)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Ticket)
            .WithMany(t => t.OrderDetails)
            .HasForeignKey(od => od.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Payment)
            .WithMany()
            .HasForeignKey(t => t.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Orders)
            .WithMany()
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}