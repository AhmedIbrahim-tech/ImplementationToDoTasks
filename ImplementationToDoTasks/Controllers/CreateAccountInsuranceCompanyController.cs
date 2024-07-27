using ImplementationToDoTasks.Models.CreateAccount;
using ImplementationToDoTasks.Services.CreateAccount;
using Microsoft.AspNetCore.Mvc;

namespace ImplementationToDoTasks.Controllers;

public class CreateAccountInsuranceCompanyController : Controller
{
    private readonly IInsuranceCompanyService _insuranceCompanyService;

    public CreateAccountInsuranceCompanyController(IInsuranceCompanyService insuranceCompanyService)
    {
        _insuranceCompanyService = insuranceCompanyService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var companies = await _insuranceCompanyService.GetInsuranceCompaniesAsync();
        return View(companies);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccounts()
    {
        var companies = await _insuranceCompanyService.GetInsuranceCompaniesAsync();

        await _insuranceCompanyService.CreateInsuranceCompanyAccountsAsync(companies);

        ViewBag.Message = "Insurance company accounts created successfully";
        return View("Index", companies);
    }
}
