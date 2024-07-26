using Dapper;
using ImplementationToDoTasks.Models.CreateAccount;
using ImplementationToDoTasks.Persistence;
using Microsoft.Data.SqlClient;


namespace ImplementationToDoTasks.Repository;

public interface IClientRepository
{
    Task<List<CreateAccountClientModel>> GetClientsAsync();
    Task UpdateClientAsync(CreateAccountClientModel client);
    Task<int> GetMaxAccNo3Async();
    Task AddAccountAsync(Account account);
}



public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _context;

    public ClientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CreateAccountClientModel>> GetClientsAsync()
    {
        var query = "SELECT SNo, FullName FROM Clients";
        var clients = await _context.Connection.QueryAsync<CreateAccountClientModel>(query, transaction: _context.Transaction);
        return clients.AsList();
    }

    public async Task UpdateClientAsync(CreateAccountClientModel client)
    {
        var query = "UPDATE Clients SET AccNoPremium = @AccNoPremium, Status = 'Active' WHERE SNo = @SNo";
        await _context.Connection.ExecuteAsync(query, new { client.AccNoPremium, client.SNo }, _context.Transaction);
    }

    public async Task<int> GetMaxAccNo3Async()
    {
        var query = "Select IsNull(Max(AccNo3),0) From Acc Where Acc1=N'Current Assets' and Acc2=N'Receivables (Clients - Premium)'";
        var result = await _context.Connection.ExecuteScalarAsync<int>(query, transaction: _context.Transaction);
        return result;
    }

    public async Task AddAccountAsync(Account account)
    {
        var query = "Insert Into Acc (AccNo,AccNo1,AccNo2,AccNo3,Acc1,Acc2,Acc3,CreatedBy) Values (@AccNo,@AccNo1,@AccNo2,@AccNo3,@Acc1,@Acc2,@Acc3,@CreatedBy)";
        await _context.Connection.ExecuteAsync(query, account, _context.Transaction);
    }

    public async Task<int> TotalRecords()
    {
        return await _context.Connection.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM Clients", null, _context.Transaction);
    }
}
