using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackwardChaining.Services;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackwardChaining.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class BCController : ControllerBase
    {
        [HttpPost]
        public IActionResult Process(RequestModel model)
        {
            try
            {
                var bcAlgorithm = new BackwardChainingAlgorithm(model);
                var result = bcAlgorithm.Execute();
                return Ok(result);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.StackTrace);
            }
        }
    }
}