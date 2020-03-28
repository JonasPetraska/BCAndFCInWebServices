using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
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
            ViewData["Title"] = "Nodes";

            var nodes = await _nodesService.GetAllNodes();
            return View(nodes.result);
        }

        public IActionResult Add()
        {
            ViewData["Title"] = "Add node";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddNodeViewModel viewModel)
        {
            ViewData["Title"] = "Add node";
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
                    ModelState.AddModelError("InputDataType", validationResult.ToStringErrors());
                    return View(viewModel);
                }

                var nodesResult = await _nodesService.GetAllNodes();
                var nodes = nodesResult.result;

                if (nodes.Any(x => x.Rule.ToStringFull() == node.Rule.ToStringFull()))
                {
                    ModelState.AddModelError("Rule", "Rule already exists.");
                    return View(viewModel);
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