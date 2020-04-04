using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Enums
{
    public enum DataTypeEnum
    {
        [Display(Name = "Sveikasis skaičius")]
        [Description("Sveikasis skaičius")]
        Integer,

        [Display(Name = "Tekstas")]
        [Description("Tekstas")]
        String,

        [Display(Name = "Dešimtainis skaičius")]
        [Description("Dešimtainis skaičius")]
        Double,

        [Display(Name = "Paveiksliukas kaip baitų masyvas")]
        [Description("Paveiksliukas kaip baitų masyvas")]
        ImageAsByteArray,

        [Display(Name = "Paveiksliukas kaip tekstas užkoduotas Base64")]
        [Description("Paveiksliukas kaip tekstas užkoduotas Base64")]
        ImageAsBase64String,
        None
    }
}
