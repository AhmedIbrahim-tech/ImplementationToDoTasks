using Microsoft.AspNetCore.Mvc;
using ImplementationToDoTasks.Models;
using ImplementationToDoTasks.Services;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ImplementationToDoTasks.Controllers
{
    public class FindReplacerAndValueReplacerController : Controller
    {
        private readonly IFindReplacerAndValueReplacerService _valueReplacerService;

        public FindReplacerAndValueReplacerController(IFindReplacerAndValueReplacerService valueReplacerService)
        {
            _valueReplacerService = valueReplacerService;
        }

        public IActionResult Index()
        {
            return View(new ReplaceValueModel());
        }

        [HttpPost]
        public IActionResult FetchValues(ReplaceValueModel model)
        {
            if (ModelState.IsValid)
            {
                var results = _valueReplacerService.FetchValues(model.ColumnName, model.OldValue);
                ViewBag.Results = results;
                ViewBag.ColumnName = model.ColumnName;
                ViewBag.SearchValue = model.OldValue;
                ViewBag.NewValue = model.NewValue;
                return View("Index", model);
            }
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult GenerateUpdateQueries(ReplaceValueModel model)
        {
            if (ModelState.IsValid)
            {
                var queries = _valueReplacerService.GenerateUpdateQueries(model.ColumnName, model.OldValue, model.NewValue);
                ViewBag.GeneratedQueries = queries;
                ViewBag.ColumnName = model.ColumnName;
                ViewBag.SearchValue = model.OldValue;
                ViewBag.NewValue = model.NewValue;
                return View("Index", model);
            }
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult ExecuteUpdateQueries(ReplaceValueModel model)
        {
            if (ModelState.IsValid)
            {
                _valueReplacerService.ReplaceValues(model.ColumnName, model.OldValue, model.NewValue);
                ViewBag.Message = "Values replaced successfully!";
                ViewBag.AlertClass = "alert-success";
                ViewBag.ExecutedQueries = _valueReplacerService.GetExecutedQueries();
                ViewBag.ProcessedTables = _valueReplacerService.GetProcessedTables();
                return View("Index", model);
            }
            return View("Index", model);
        }
    }
}
