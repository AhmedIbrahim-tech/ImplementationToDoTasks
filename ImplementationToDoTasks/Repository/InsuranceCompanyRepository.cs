using Dapper;
using ImplementationToDoTasks.Models.CreateAccount;
using ImplementationToDoTasks.Persistence;

namespace ImplementationToDoTasks.Repository;

public interface IInsuranceCompanyRepository
{
    Task<List<InsuranceCompanyModel>> GetInsuranceCompaniesAsync();
    Task<int> GetMaxAccNoAsync(string acc1, string acc2);
    Task CreateAccountAsync(string accNo, string acc1, string acc2, string acc3);
    Task UpdateInsuranceCompanyAccountsAsync(int sno, string accNoCommAccrued, string accNoCommDue, string accNoVATReceivable, string accNoPremium, string accNoClientsMoney);
}


public class InsuranceCompanyRepository : IInsuranceCompanyRepository
{
    private readonly AppDbContext _context;

    public InsuranceCompanyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<InsuranceCompanyModel>> GetInsuranceCompaniesAsync()
    {
        var query = "SELECT SNo, CompanyName FROM InsuranceCompanies";
        var companies = await _context.Connection.QueryAsync<InsuranceCompanyModel>(query, transaction: _context.Transaction);
        return companies.AsList();
    }

    public async Task<int> GetMaxAccNoAsync(string acc1, string acc2)
    {
        var query = "SELECT ISNULL(MAX(AccNo3), 0) FROM Acc WHERE Acc1 = @Acc1 AND Acc2 = @Acc2";
        var maxAccNo = await _context.Connection.ExecuteScalarAsync<int>(query, new { Acc1 = acc1, Acc2 = acc2 }, _context.Transaction);
        return maxAccNo;
    }

    public async Task CreateAccountAsync(string accNo, string acc1, string acc2, string acc3)
    {
        var query = "INSERT INTO Acc (AccNo, AccNo1, AccNo2, AccNo3, Acc1, Acc2, Acc3, CreatedBy) " +
                    "VALUES (@AccNo, @AccNo1, @AccNo2, @AccNo3, @Acc1, @Acc2, @Acc3, @CreatedBy)";
        await _context.Connection.ExecuteAsync(query, new
        {
            AccNo = accNo,
            AccNo1 = accNo.Substring(0, 1),
            AccNo2 = accNo.Substring(1, 1),
            AccNo3 = accNo.Substring(2),
            Acc1 = acc1,
            Acc2 = acc2,
            Acc3 = acc3,
            CreatedBy = "Admin."
        }, _context.Transaction);
    }

    public async Task UpdateInsuranceCompanyAccountsAsync(int sno, string accNoCommAccrued, string accNoCommDue, string accNoVATReceivable, string accNoPremium, string accNoClientsMoney)
    {
        var query = "UPDATE InsuranceCompanies " +
                    "SET AccNoCommAccrued = @AccNoCommAccrued, AccNoCommDue = @AccNoCommDue, AccNoVATReceivable = @AccNoVATReceivable, " +
                    "AccNoPremium = @AccNoPremium, AccNoClientsMoney = @AccNoClientsMoney, ApprovedBy = @ApprovedBy " +
                    "WHERE SNo = @SNo";
        await _context.Connection.ExecuteAsync(query, new
        {
            AccNoCommAccrued = accNoCommAccrued,
            AccNoCommDue = accNoCommDue,
            AccNoVATReceivable = accNoVATReceivable,
            AccNoPremium = accNoPremium,
            AccNoClientsMoney = accNoClientsMoney,
            ApprovedBy = "Admin.",
            SNo = sno
        }, _context.Transaction);
    }
}
