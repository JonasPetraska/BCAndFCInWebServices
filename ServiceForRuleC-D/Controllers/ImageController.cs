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

namespace ServiceForRuleC_D.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImageController : ControllerBase
    {
        private string _prefixText = "Copyright";

        [HttpPost]
        public IActionResult Index(NodeRequestModel model)
        {
            var responseModel = new NodeResponseModel();

            if (model.InputData.Count != 2)
                return BadRequest("Nepakankamas parametrų skaičius.");

            var imageAsBase64 = model.InputData[0].Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", "");
            var strToPutOnImage = model.InputData[1];

            var img = GetBase64StringOfImageWithText(imageAsBase64, _prefixText + " " + strToPutOnImage);

            responseModel.OutputData = img;

            return Ok(responseModel);
        }

        [HttpPost]
        public IActionResult Health()
        {
            return Ok();
        }

        private string GetBase64StringOfImageWithText(string base64Image, string text)
        {
            string str = "";

            var bytes = Convert.FromBase64String(base64Image);

            // Creates a new image with all the pixels set as transparent. 
            using (var image = Image.Load(bytes))
            {
                image.Mutate(context =>
                {
                    var textGraphicsOptions = new TextGraphicsOptions(true)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    //Pick any font size, since it will be replaced later
                    var initialFont = SystemFonts.CreateFont("Arial", 10);
                    var size = TextMeasurer.Measure(text, new RendererOptions(initialFont));
                    var scalingFactor = Math.Min(image.Width / 2 / size.Width, image.Height / 2 / size.Height);

                    var font = new Font(initialFont, scalingFactor * initialFont.Size, FontStyle.Bold);

                    var textColor = GetContrastColorBW(image);

                    var bottom = new PointF(image.Width / 2, image.Height - font.Size);
                    context.DrawText(textGraphicsOptions, text, font, textColor, bottom);
                });

                str = image.ToBase64String<Rgba32>(PngFormat.Instance);

            }

            return str;
        }

        private Color GetContrastColorBW(Image<Rgba32> image)
        {
            var sizeOfOne = new SixLabors.Primitives.Size(1, 1);

            var croppedImageResizedToOnePixel = image.Clone(
                img => img.Resize(sizeOfOne));

            var averageColor = croppedImageResizedToOnePixel[0, 0];

            var luminance = (0.299 * averageColor.R + 0.587 * averageColor.G + 0.114 * averageColor.B) / 255;

            return luminance > 0.5 ? Color.Black : Color.White;
        }
    }
}