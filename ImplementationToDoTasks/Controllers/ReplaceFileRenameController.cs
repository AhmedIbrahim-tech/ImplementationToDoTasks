using ImplementationToDoTasks.Models;
using ImplementationToDoTasks.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImplementationToDoTasks.Controllers;

public class ReplaceFileRenameController : Controller
{
    private readonly IReplaceFileRenameServices _service;

    public ReplaceFileRenameController(IReplaceFileRenameServices service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult RenameFiles(ReplaceFileRenameModel model)
    {
        if (ModelState.IsValid)
        {
            List<string> results = _service.RenameFiles(model);
            ViewBag.Results = results;
        }
        return View("Index", model);
    }

}
