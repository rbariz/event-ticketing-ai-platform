using EventTicketingAiPlatform.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Infrastructure.Persistence
{
    public sealed class EventTicketingDbContext : DbContext
    {
        public EventTicketingDbContext(DbContextOptions<EventTicketingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events => Set<Event>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<ScanAttempt> ScanAttempts => Set<ScanAttempt>();

        public DbSet<AgentDecisionLog> AgentDecisionLogs => Set<AgentDecisionLog>();
        public DbSet<AgentNotification> AgentNotifications => Set<AgentNotification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventTicketingDbContext).Assembly);
        }
    }
}
