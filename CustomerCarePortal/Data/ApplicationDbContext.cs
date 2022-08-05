using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CustomerCarePortal.Models;

namespace CustomerCarePortal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Department> Departments {get; set;}
        public DbSet<History> Histories {get; set;}
        public DbSet<State> States {get; set;}
        public DbSet<Team> Teams {get; set;}
        public DbSet<Ticket> Tickets {get; set;}
        public DbSet<Transition> Transitions {get; set;}
        public DbSet<Workflow> Workflows {get; set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Team>()
                .HasMany(p => p.Agents)
                .WithOne(p => p.Team);
            modelBuilder.Entity<State>()
                .HasMany(s => s.Transitions)
                .WithOne(t => t.SourceState);

            modelBuilder.Entity<State>()
                .HasOne(s => s.Workflow)
                .WithMany(w => w.States)
                .HasForeignKey(w => w.WorkflowId);
        }
    }
}