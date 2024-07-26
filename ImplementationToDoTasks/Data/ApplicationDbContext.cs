using ImplementationToDoTasks.Models.CreateAccount;
using Microsoft.EntityFrameworkCore;

namespace ImplementationToDoTasks.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<CreateAccountClientModel> Clients { get; set; }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.ID);
        });
    }
}
