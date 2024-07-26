using ImplementationToDoTasks.Models.CreateAccount;
using ImplementationToDoTasks.Persistence;
using ImplementationToDoTasks.Repository;

namespace ImplementationToDoTasks.Services.CreateAccount;

public interface ICreateAccountClientServices
{
    Task<List<CreateAccountClientModel>> GetClientsAsync();
    Task CreateClientAccountsAsync(List<CreateAccountClientModel> clients);
}


public class CreateAccountClientServices : ICreateAccountClientServices
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountClientServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CreateAccountClientModel>> GetClientsAsync()
    {
        try
        {
            return await _unitOfWork.Client.GetClientsAsync();
        }
        catch (Exception ex)
        {
            // يمكنك تسجيل الخطأ هنا إذا رغبت في ذلك
            throw new ApplicationException("An error occurred while fetching clients", ex);
        }
    }

    public async Task CreateClientAccountsAsync(List<CreateAccountClientModel> clients)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var client in clients)
            {
                string premiumAccNumber = (await _unitOfWork.Client.GetMaxAccNo3Async() + 1).ToString();


                if (int.Parse(premiumAccNumber) > 999999)
                {
                    await _unitOfWork.RollbackAsync();
                    throw new InvalidOperationException("You reached the maximum number");
                }


                int premiumAccNumberLenght = premiumAccNumber.Length;
                if (premiumAccNumber.Length < 6)
                {
                    for (int i = 0; i < 6 - premiumAccNumberLenght; i++)
                    {
                        premiumAccNumber = "0" + premiumAccNumber;
                    }
                }
                string premiumAccNo = "1104" + premiumAccNumber;


                var account = new Account
                {
                    AccNo = premiumAccNo,
                    AccNo1 = "1",
                    AccNo2 = "4",
                    AccNo3 = premiumAccNumber,
                    Acc1 = "Current Assets",
                    Acc2 = "Receivables (Clients - Premium)",
                    Acc3 = client.FullName + " - Premium",
                    CreatedBy = "Admin."
                };

                await _unitOfWork.Client.AddAccountAsync(account);

                client.AccNoPremium = premiumAccNo;
                client.Status = "Active";
                await _unitOfWork.Client.UpdateClientAsync(client);
            }

            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new ApplicationException("An error occurred while creating client accounts", ex);
        }
    }
}
