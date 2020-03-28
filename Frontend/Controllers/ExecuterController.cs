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
            ViewData["Title"] = "Programs";

            var programsResult = await _programService.GetAllPrograms();
            var programs = programsResult.result;

            return View(programs);
        }

        [HttpGet]
        public async Task<IActionResult> Execute(int id)
        {
            ViewData["Title"] = "Execute program";

            var programsResult = await _programService.GetAllPrograms();
            var programs = programsResult.result;
            var program = programs.FirstOrDefault(x => x.Id == id);

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
            ViewData["Title"] = "Execute program";

            var programsResult = await _programService.GetAllPrograms();
            var programs = programsResult.result;
            var program = programs.FirstOrDefault(x => x.Id == model.ProgramId);


            var count = program.Nodes.Count;
            var i = 0;
            var result = "";
            do
            {
                if (String.IsNullOrEmpty(result))
                {
                    var res = await program.Nodes[i].GetNodeResultAsync(new Common.Models.NodeRequestModel()
                    {
                        InputData = model.NodeInputValues
                    });

                    result = res.result.OutputData;
                }
                else
                {
                    var res = await program.Nodes[i].GetNodeResultAsync(new Common.Models.NodeRequestModel()
                    {
                        InputData = new List<string>() { result }
                    });

                    result = res.result.OutputData;
                }

                i++;
            } while (i < count);

            var finalResult = result;

            model.Result = finalResult;
            model.Nodes = program.Nodes;
            return View(model);
        }
    }
}