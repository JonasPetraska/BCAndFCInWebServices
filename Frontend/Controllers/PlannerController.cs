using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class PlannerController : Controller
    {
        private INodesService _nodesService;
        private IBCFCService _bcfcService;
        private IProgramService _programService;

        public PlannerController(INodesService nodesService, IBCFCService bcfcService, IProgramService programService)
        {
            _nodesService = nodesService;
            _bcfcService = bcfcService;
            _programService = programService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Planavimas";

            var nodesResult = await _nodesService.GetAllNodes();
            var nodes = nodesResult.result;
            var rules = string.Join(System.Environment.NewLine, nodes.Select(x => x.Rule.ToStringFull()).ToList());

            ViewData["Rules"] = rules;

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ProcessRequest(RequestModel model)
        //{
        //    var nodesResult = await _nodesService.GetAllNodes();
        //    if (!nodesResult.isSuccessful)
        //    {
        //        ModelState.AddModelError("", nodesResult.ToStringErrors());
        //        return View(model);
        //    }

        //    var nodes = nodesResult.result;
        //    var rules = string.Join(System.Environment.NewLine, nodes.Select(x => x.Rule.ToStringFull()).ToList());

        //    foreach (var rule in model.Rules)
        //        if (!rules.Contains(rule.ToStringFull()))
        //        {
        //            return PartialView("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //        }

        //    model.Nodes = nodes;

        //    model.AssignNumbersToRules();

        //    HttpRequestResult<ResponseModel> result;

        //    if (model.MethodType.ToLower() == "backward")
        //    {
        //        result = await _bcfcService.BackwardTrackAsync(model);
        //    }
        //    else
        //    {
        //        result = await _bcfcService.ForwardTrackAsync(model);
        //    }

        //    if (result.isSuccessful)
        //    {
        //        var resObj = result.result;

        //        if (resObj.MethodType.ToLower() == "forward")
        //            resObj.GenerateIterationsFromTrace();

        //        return PartialView("_ResultPartialView", resObj);
        //    }
        //    else
        //    {
        //        return PartialView("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //    }

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessRequest(RequestModel model)
        {
            model.AssignNumbersToRules();

            var nodesResult = await _nodesService.GetAllNodes();
            if (!nodesResult.isSuccessful)
            {
                ModelState.AddModelError("", nodesResult.ToStringErrors());
                return View(model);
            }

            var nodes = nodesResult.result;
            model.Nodes = nodes;

            var rules = string.Join(System.Environment.NewLine, nodes.Select(x => x.Rule.ToStringFull()).ToList());

            foreach (var rule in model.Rules)
                if (!rules.Contains(rule.ToStringFull()))
                {
                    return PartialView("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }

            HttpRequestResult<ResponseModel> result;

            if (model.MethodType.ToLower() == "backward")
            {
                result = await _bcfcService.BackwardTrackAsync(model);
            }
            else
            {
                result = await _bcfcService.ForwardTrackAsync(model);
            }

            if (result.isSuccessful)
            {
                var resObj = result.result;

                if (resObj.MethodType.ToLower() == "forward")
                    resObj.GenerateIterationsFromTrace();

                resObj.OrderNodesByProduction();

                return PartialView("_ResultPartialView", resObj);
            }
            else
            {
                return PartialView("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveAsProgram(SaveAsProgramViewModel model)
        {
            var program = new Common.Models.Program();
            var nodes = new List<Node>();
            var allNodesRes = await _nodesService.GetAllNodes();
            var allNodes = allNodesRes.result;

            foreach(var id in model.NodeIds)
            {
                var node = allNodes.FirstOrDefault(x => x.Id == id);
                if (node != null)
                    nodes.Add(node);
            }

            program.Nodes = nodes;
            program.Name = model.ProgramName;

            //Find all input values
            var firstNode = nodes.First();
            for(int i = 0; i < firstNode.Rule.LeftSide.Count; i++)
                program.Inputs.Add(new ProgramInput()
                {
                    Letter = firstNode.Rule.LeftSide[i],
                    NodeId = firstNode.Id,
                    Type  = firstNode.InputDataType[i]
                });

            var nodesWithoutFirst = nodes.Where(x => x.Id != firstNode.Id).ToList();
            var nodesWithoutFirstCopy = new List<Node>(nodesWithoutFirst);
            foreach (var node in nodesWithoutFirst)
                for (int i = 0; i < node.Rule.LeftSide.Count; i++)
                    if (!nodesWithoutFirstCopy.Any(x => x.Rule.RightSide == node.Rule.LeftSide[i]) &&
                        firstNode.Rule.RightSide != node.Rule.LeftSide[i])
                        program.Inputs.Add(new ProgramInput()
                        {
                            Letter = node.Rule.LeftSide[i],
                            NodeId = node.Id,
                            Type = node.InputDataType[i]
                        });

            await _programService.AddProgram(program);
            return Json("Gamybos planas išsaugotas.");

        }
    }
}