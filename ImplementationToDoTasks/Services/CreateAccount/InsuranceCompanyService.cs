using ImplementationToDoTasks.Models.CreateAccount;
using ImplementationToDoTasks.Persistence;

namespace ImplementationToDoTasks.Services.CreateAccount;

public interface IInsuranceCompanyService
{
    Task<List<InsuranceCompanyModel>> GetInsuranceCompaniesAsync();
    Task CreateInsuranceCompanyAccountsAsync(List<InsuranceCompanyModel> insuranceCompanies);
}


public class InsuranceCompanyService : IInsuranceCompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public InsuranceCompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<InsuranceCompanyModel>> GetInsuranceCompaniesAsync()
    {
        return await _unitOfWork.InsuranceCompany.GetInsuranceCompaniesAsync();
    }

    public async Task CreateInsuranceCompanyAccountsAsync(List<InsuranceCompanyModel> insuranceCompanies)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var company in insuranceCompanies)
            {
                // Create Accrued Commission Account
                var accNumberAccrued = await _unitOfWork.InsuranceCompany.GetMaxAccNoAsync("Current Assets", "Receivables Commission (Accrued)");
                var accNoCommAccrued = GenerateAccountNumber("1105", accNumberAccrued);
                await _unitOfWork.InsuranceCompany.CreateAccountAsync(accNoCommAccrued, "Current Assets", "Receivables Commission (Accrued)", company.CompanyName + " - Accrued Commission");

                // Create Due Commission Account
                var accNumberDueComm = await _unitOfWork.InsuranceCompany.GetMaxAccNoAsync("Current Assets", "Receivables Commission (Due)");
                var accNoCommDue = GenerateAccountNumber("1106", accNumberDueComm);
                await _unitOfWork.InsuranceCompany.CreateAccountAsync(accNoCommDue, "Current Assets", "Receivables Commission (Due)", company.CompanyName + " - Due Commission");

                // Create VAT Receivables Account
                var accNumberVATReceivable = await _unitOfWork.InsuranceCompany.GetMaxAccNoAsync("Current Assets", "Receivables Commission (VAT)");
                var accNoVATReceivable = GenerateAccountNumber("1107", accNumberVATReceivable);
                await _unitOfWork.InsuranceCompany.CreateAccountAsync(accNoVATReceivable, "Current Assets", "Receivables Commission (VAT)", company.CompanyName + " - VAT Receivables");

                // Create Premium Payable Account
                var accNumberPremium = await _unitOfWork.InsuranceCompany.GetMaxAccNoAsync("Current Liabilities", "Payable Premium");
                var accNoPremium = GenerateAccountNumber("2101", accNumberPremium);
                await _unitOfWork.InsuranceCompany.CreateAccountAsync(accNoPremium, "Current Liabilities", "Payable Premium", company.CompanyName + " - Premium");

                // Create Clients Money Account
                var accNumberClientsMoney = await _unitOfWork.InsuranceCompany.GetMaxAccNoAsync("Current Liabilities", "Payable Premium (Clients Money)");
                var accNoClientsMoney = GenerateAccountNumber("2102", accNumberClientsMoney);
                await _unitOfWork.InsuranceCompany.CreateAccountAsync(accNoClientsMoney, "Current Liabilities", "Payable Premium (Clients Money)", company.CompanyName + " - Clients Money");

                // Update Insurance Company with Account Numbers
                await _unitOfWork.InsuranceCompany.UpdateInsuranceCompanyAccountsAsync(company.SNo, accNoCommAccrued, accNoCommDue, accNoVATReceivable, accNoPremium, accNoClientsMoney);
            }

            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new ApplicationException("An error occurred while creating insurance company accounts", ex);
        }
    }

    private string GenerateAccountNumber(string prefix, int accNumber)
    {
        var accNumberStr = accNumber.ToString().PadLeft(6, '0');
        return prefix + accNumberStr;
    }
}
