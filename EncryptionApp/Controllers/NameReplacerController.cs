using ImplementationToDoTasks.Models;
using ImplementationToDoTasks.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImplementationToDoTasks.Controllers;

public class NameReplacerController : Controller
{
    private readonly INameReplacerService _service;

    public NameReplacerController(INameReplacerService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Replace(ReplaceNameModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _service.ReplaceNames(model);
                ViewBag.Message = "Names replaced successfully!";
                ViewBag.AlertClass = "alert-success";
                ViewBag.ExecutedQueries = _service.GetExecutedQueries();
                ViewBag.ProcessedTables = _service.GetProcessedTables();
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
                ViewBag.AlertClass = "alert-danger";
                ViewBag.ExecutedQueries = new List<string>();
                ViewBag.ProcessedTables = new List<string>();
            }
        }
        return View("Index", model);
    }
}



