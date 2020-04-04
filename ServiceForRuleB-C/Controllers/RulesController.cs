using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ServiceForRuleB_C.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RulesController : ControllerBase
    {

        [HttpPost]
        public IActionResult Index(NodeRequestModel model)
        {
            var responseModel = new NodeResponseModel();

            if(model.InputData.Count != 1)
                return BadRequest("Nepakankamas parametrų skaičius.");

            var imageAsBase64 = model.InputData[0].Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", "");

            var img = GetBase64StringOfFlippedImage(imageAsBase64);

            responseModel.OutputData = img;

            return Ok(responseModel);
        }

        [HttpPost]
        public IActionResult Health()
        {
            return Ok();
        }

        private string GetBase64StringOfFlippedImage(string base64Image)
        {
            string str = "";

            var bytes = Convert.FromBase64String(base64Image);

            // Creates a new image with all the pixels set as transparent. 
            using (var image = Image.Load(bytes))
            {

                image.Mutate(context =>
                {
                    context.Flip(FlipMode.Vertical);
                });

                str = image.ToBase64String<Rgba32>(PngFormat.Instance);

            }

            return str;
        }
    }
}