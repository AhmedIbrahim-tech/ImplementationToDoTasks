using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Transactions;

namespace ImplementationToDoTasks.Persistence;

public sealed class AppDbContext : IDisposable
{
    private Guid _id;
    //private readonly IConfiguration _configuration;
    public AppDbContext(string configuration)
    {
        _id = Guid.NewGuid();
        //_configuration = configuration;
        Connection = new SqlConnection(configuration);
        Connection.Open();
    }

    public DbConnection Connection { get; }
    public DbTransaction Transaction { get; set; }
    public TransactionScope TransactionScope { get; set; }
    public void Dispose() => Connection?.Dispose();
    public async Task DisposeAsync() => await Connection?.CloseAsync();
}
