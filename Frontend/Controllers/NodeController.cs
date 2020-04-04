using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Frontend.Extensions;
using Frontend.Services;
using Frontend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class NodeController : Controller
    {
        private INodesService _nodesService;

        public NodeController(INodesService nodesService)
        {
            _nodesService = nodesService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Tiekėjai";

            var nodes = await _nodesService.GetAllNodes();
            return View(nodes.result);
        }

        public IActionResult Add()
        {
            ViewData["Title"] = "Pridėti tiekėją";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddNodeViewModel viewModel)
        {
            ViewData["Title"] = "Pridėti tiekėją";
            if (ModelState.IsValid)
            {
                var endpointValidationResult = await viewModel.ValidateEndpoint();
                if (!endpointValidationResult.isSuccessful)
                {
                    ModelState.AddModelError("HealthEndPoint", endpointValidationResult.ToStringErrors());
                    return View(viewModel);
                }

                var ruleValidationResult = viewModel.ValidateRule();
                if (!ruleValidationResult.isSuccessful)
                {
                    ModelState.AddModelError("Rule", ruleValidationResult.ToStringErrors());
                    return View(viewModel);
                }

                var node = (Node)viewModel;
                node.SetRuleFromString(viewModel.Rule);

                var validationResult = node.ValidateInputDataTypes();
                if (!validationResult.isSuccessful)
                {
                    ModelState.AddModelError("", validationResult.ToStringErrors());
                    return View(viewModel);
                }

                var nodesResult = await _nodesService.GetAllNodes();
                var nodes = nodesResult.result;

                if (nodes.Any(x => x.Rule.ToStringFull() == node.Rule.ToStringFull()))
                {
                    ModelState.AddModelError("Rule", "Taisyklė jau pridėta.");
                    return View(viewModel);
                }

                //Check input type compatibility
                var nodesWithOutputsSameAsInput = new List<Node>();
                foreach(var inputDataType in node.InputDataType)
                {
                    var temp = nodes.Where(x => node.Rule.LeftSide.Contains(x.Rule.RightSide)).ToList();
                    nodesWithOutputsSameAsInput.AddRange(temp);
                }

                if (nodesWithOutputsSameAsInput.Any())
                {
                    var message = "";
                    foreach(var node1 in nodesWithOutputsSameAsInput)
                    {
                        var node1RightSide = node1.Rule.RightSide;
                        var nodeLeftSide = node.Rule.LeftSide;

                        var commonIndex = nodeLeftSide.IndexOf(node1RightSide);

                        if (node.InputDataType[commonIndex] != node1.OutputDataType)
                            message += "Antecedento '" + nodeLeftSide[commonIndex] + "' tipas turi būti '" + node1.OutputDataType.DisplayName() + "'" + System.Environment.NewLine;
                    }

                    if(!String.IsNullOrEmpty(message))
                    {
                        ModelState.AddModelError("", message);
                        return View(viewModel);
                    }

                }

                //Check output type compatibilty
                var nodesWithInputsSameAsOutput = nodes.Where(x => x.Rule.LeftSide.Contains(node.Rule.RightSide)).ToList();
                if (nodesWithInputsSameAsOutput.Any())
                {
                    var message = "";
                    foreach (var node1 in nodesWithInputsSameAsOutput)
                    {
                        var nodeRightSide = node.Rule.RightSide;
                        var node1LeftSide = node1.Rule.LeftSide;

                        var commonIndex = node1LeftSide.IndexOf(nodeRightSide);

                        if (node1.InputDataType[commonIndex] != node.OutputDataType)
                            message += "Konsekvento '" + nodeRightSide + "' tipas turi būti '" + node1.InputDataType[commonIndex].DisplayName() + "'" + System.Environment.NewLine;
                    }

                    if (!String.IsNullOrEmpty(message))
                    {
                        ModelState.AddModelError("", message);
                        return View(viewModel);
                    }
                }


                await _nodesService.AddNode(node);

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Remove(int id)
        {
            await _nodesService.RemoveNode(id);

            return RedirectToAction("Nodes");
        }
    }
}