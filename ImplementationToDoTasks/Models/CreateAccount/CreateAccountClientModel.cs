using System.ComponentModel.DataAnnotations;

namespace ImplementationToDoTasks.Models.CreateAccount;

public class CreateAccountClientModel
{
    public int SNo { get; set; }
    public string FullName { get; set; }
    public string AccNoPremium { get; set; }
    public string Status { get; set; }

}


public class Account
{
    public int ID { get; set; }
    public string AccNo { get; set; }
    public string AccNo1 { get; set; }
    public string AccNo2 { get; set; }
    public string AccNo3 { get; set; }
    public string Acc1 { get; set; }
    public string Acc2 { get; set; }
    public string Acc3 { get; set; }
    public string ClientName { get; set; }
    public string CreatedBy { get; set; }
}