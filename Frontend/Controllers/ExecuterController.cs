using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Helpers;
using Common.Models;
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
                Nodes = new List<Common.Models.Node>(program.Nodes),
                NodeInputValues = program.Inputs.Select(x => new ProgramInputWithValue(x)).ToList()
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

            model.Nodes = program.Nodes;

            var validationResult = ProgramHelpers.ValidateInputValues(model.NodeInputValues);
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
                var inputValuesForNode = new List<string>();
                if (!String.IsNullOrEmpty(result))
                    inputValuesForNode.Add(result);

                inputValuesForNode.AddRange(model.NodeInputValues.Where(x => x.NodeId == nodes[i].Id).Select(x => x.Value).ToList());
                
                var res = await nodes[i].GetNodeResultAsync(new Common.Models.NodeRequestModel()
                {
                    InputData = inputValuesForNode
                });

                if (!res.isSuccessful)
                {
                    model.Error = res.ToStringErrors();
                    break;
                }

                result = res.result.OutputData;

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