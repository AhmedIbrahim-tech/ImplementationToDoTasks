using ImplementationToDoTasks.Repository;

namespace ImplementationToDoTasks.Persistence;

public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    void BeginTransactionScope();
    void CommitTransactionScope();



    // Repository
    IClientRepository Client { get; }
    IInsuranceCompanyRepository InsuranceCompany { get; }
}
