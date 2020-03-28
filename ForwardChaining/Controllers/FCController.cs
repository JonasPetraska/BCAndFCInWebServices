using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using ForwardChaining.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForwardChaining.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class FCController : ControllerBase
    {
        [HttpPost]
        public IActionResult Process(RequestModel model)
        {
            try
            {
                var fcAlgorithm = new ForwardChainingAlgorithm(model);
                var result = fcAlgorithm.Execute();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.StackTrace);
            }
        }
    }
}