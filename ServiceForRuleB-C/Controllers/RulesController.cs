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

            var img = GetBase64StringOfImageWithText(model.InputData.FirstOrDefault());

            responseModel.OutputData = img;

            return Ok(responseModel);
        }

        [HttpPost]
        public IActionResult Health()
        {
            return Ok();
        }

        private string GetBase64StringOfImageWithText(string text)
        {
            int width = 640;
            int height = 480;
            string str = "";
            // Creates a new image with all the pixels set as transparent. 
            using (var image = new Image<Rgba32>(width, height))
            {
                image.Mutate(context =>
                {
                    context.BackgroundColor(Color.Black);
                    var textGraphicsOptions = new TextGraphicsOptions(true)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    var font = SystemFonts.CreateFont("Arial", 26);
                    var center = new PointF(image.Width/2, image.Height / 2);
                    context.DrawText(textGraphicsOptions, text, font, Color.White, center);
                });

                str = image.ToBase64String<Rgba32>(PngFormat.Instance);

            }

            return str;
        }
    }
}