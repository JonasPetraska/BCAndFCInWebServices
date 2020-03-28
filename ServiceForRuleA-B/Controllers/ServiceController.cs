using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ServiceForRuleA_B.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ServiceController : ControllerBase
    {
        private string _coreStr = "Testuojam";

        [HttpPost]
        public IActionResult Index(NodeRequestModel model)
        {
            var responseModel = new NodeResponseModel();

            var stringAsInt = int.Parse(model.InputData.FirstOrDefault());

            var res = stringAsInt * 2 + _coreStr;

            responseModel.OutputData = res.ToString();

            return Ok(responseModel);

        }

        [HttpPost]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}