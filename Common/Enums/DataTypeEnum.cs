using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Enums
{
    public enum DataTypeEnum
    {
        [Display(Name = "Number")]
        Integer,
        [Display(Name = "Text")]
        String,
        [Display(Name = "Decimal number")]
        Double,
        [Display(Name = "Image as byte array")]
        ImageAsByteArray,
        [Display(Name = "Image as base64 string")]
        ImageAsBase64String
    }
}
