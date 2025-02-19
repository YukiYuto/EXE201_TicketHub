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
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketTransfers> TicketTransfers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderTicket> OrderTickets { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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


        // Configure TicketTransfer's foreign keys to avoid multiple cascade paths
        modelBuilder.Entity<TicketTransfers>()
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
        .OnDelete(DeleteBehavior.Restrict);  // Or NoAction, depending on your needs

        modelBuilder.Entity<ChatRoom>()
            .HasOne(c => c.ReceiveMessageUser)
            .WithMany()
            .HasForeignKey(c => c.ReceiveMessageUserId)
            .OnDelete(DeleteBehavior.Restrict);  // Avoid cascade
    }
}