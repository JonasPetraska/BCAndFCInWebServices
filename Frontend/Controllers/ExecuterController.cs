using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.Services;
using Frontend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class ExecuterController : Controller
    {
        private IProgramService _programService;

        public ExecuterController(IProgramService programService)
        {
            _programService = programService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Vykdymo planai";

            var programsResult = await _programService.GetAllPrograms();
            var programs = programsResult.result;

            return View(programs);
        }

        [HttpGet]
        public async Task<IActionResult> Execute(int id)
        {
            ViewData["Title"] = "Plano vykdymas";

            var programsResult = await _programService.GetAllPrograms();
            var programs = programsResult.result;
            var program = programs.FirstOrDefault(x => x.Id == id);

            if (program == null)
                return RedirectToAction("Index");

            var model = new ExecuteProgramViewModel()
            {
                ProgramId = program.Id,
                ProgramName = program.Name,
                Nodes = new List<Common.Models.Node>(program.Nodes)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Execute(ExecuteProgramViewModel model)
        {
            ViewData["Title"] = "Plano vykdymas";

            var programsResult = await _programService.GetAllPrograms();
            var programs = programsResult.result;
            var program = programs.FirstOrDefault(x => x.Id == model.ProgramId);

            if (program == null)
                return RedirectToAction("Index");

            var nodes = program.Nodes;

            if (nodes == null || !nodes.Any())
                return RedirectToAction("Index");

            var firstNode = nodes.First();
            model.Nodes = program.Nodes;

            var validationResult = firstNode.ValidateInputValues(model.NodeInputValues);
            if (!validationResult.isSuccessful)
            {
                ModelState.AddModelError("", validationResult.ToStringErrors());
                return View(model);
            }

            var validationRes = validationResult.result;

            if (!validationRes)
            {
                ModelState.AddModelError("", "Netinkamos reikšmės.");
                return View(model);
            }

            var count = nodes.Count;
            var i = 0;
            var result = "";
            do
            {
                if (String.IsNullOrEmpty(result))
                {
                    var res = await nodes[i].GetNodeResultAsync(new Common.Models.NodeRequestModel()
                    {
                        InputData = model.NodeInputValues
                    });

                    if (!res.isSuccessful)
                    {
                        model.Error = res.ToStringErrors();
                        break;
                    }

                    result = res.result.OutputData;
                }
                else
                {
                    var res = await nodes[i].GetNodeResultAsync(new Common.Models.NodeRequestModel()
                    {
                        InputData = new List<string>() { result }
                    });

                    if (!res.isSuccessful)
                    {
                        model.Error = res.ToStringErrors();
                        break;
                    }

                    result = res.result.OutputData;
                }

                i++;
            } while (i < count);

            if (String.IsNullOrEmpty(model.Error))
            {
                var finalResult = result;

                model.Result = finalResult;
            }
            return View(model);
        }

        public async Task<IActionResult> Remove(int id)
        {
            await _programService.RemoveProgram(id);

            TempData["SuccessMessage"] = "Gamybos planas sėkmingai ištrintas.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Json()
        {
            var programsResult = await _programService.GetAllPrograms();
            if (!programsResult.isSuccessful)
            {
                TempData["DangerMessage"] = programsResult.ToStringErrors();
                return RedirectToAction("Index");
            }

            var programs = programsResult.result;

            return Json(programs, new System.Text.Json.JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }
    }
}