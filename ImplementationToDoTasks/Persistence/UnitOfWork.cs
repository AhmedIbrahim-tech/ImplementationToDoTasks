
using System.Threading.Tasks;
using ImplementationToDoTasks.Repository;
using System.Transactions;
using Microsoft.Extensions.Configuration;

namespace ImplementationToDoTasks.Persistence;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    private IClientRepository _clientRepository;
    private IInsuranceCompanyRepository _insuranceCompanyRepository;

    public UnitOfWork(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public IClientRepository Client => _clientRepository ?? new ClientRepository(_context);
    public IInsuranceCompanyRepository InsuranceCompany => _insuranceCompanyRepository ?? new InsuranceCompanyRepository(_context);


    public void BeginTransaction()
    {
        _context.Transaction = _context.Connection.BeginTransaction();
    }

    public async Task BeginTransactionAsync()
    {
        _context.Transaction = await _context.Connection.BeginTransactionAsync();
    }

    public void Commit()
    {
        _context.Transaction.Commit();
        _context.Dispose();
        Dispose();
    }

    public async Task CommitAsync()
    {
        await _context.Transaction.CommitAsync();
        await _context.DisposeAsync();
        Dispose();
    }

    public void Rollback()
    {
        _context.Transaction.Rollback();
    }

    public async Task RollbackAsync()
    {
        await _context.Transaction.RollbackAsync();
    }

    public void Dispose() => _context.Transaction?.Dispose();

    public void BeginTransactionScope()
    {
        _context.TransactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }

    public void CommitTransactionScope()
    {
        _context.TransactionScope.Complete();
        _context.TransactionScope.Dispose();
    }
}

