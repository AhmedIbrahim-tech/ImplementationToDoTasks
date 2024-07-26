using ImplementationToDoTasks.Services.CreateAccount;
using Microsoft.AspNetCore.Mvc;

namespace ImplementationToDoTasks.Controllers;

public class CreateAccountClientController : Controller
{
    private readonly ICreateAccountClientServices _clientService;

    public CreateAccountClientController(ICreateAccountClientServices clientService)
    {
        _clientService = clientService;
    }

    public async Task<IActionResult> Index()
    {
        var clients = await _clientService.GetClientsAsync();
        return View(clients);
    }


    [HttpPost]
    public async Task<IActionResult> CreateClientAccounts()
    {
        var clients = await _clientService.GetClientsAsync();
        await _clientService.CreateClientAccountsAsync(clients);
        ViewBag.Message = "Clients profile created successfully";
        return View("Index", clients);
    }

}
