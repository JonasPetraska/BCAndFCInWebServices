using Common.Enums;
using Common.Extensions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Helpers
{
    public static class ProgramHelpers
    {
        public static HttpRequestResult<bool> ValidateInputValues(List<ProgramInputWithValue> values)
        {
            if (!values.TrueForAll(x => !String.IsNullOrEmpty(x.Value)))
                return new HttpRequestResult<bool>("Reikšmių kiekis per mažas.");

            var message = "";

            for (int i = 0; i < values.Count; i++)
            {
                switch (values[i].Type)
                {
                    case DataTypeEnum.Double:
                        var parsedDouble = Double.TryParse(values[i].Value, out var resDouble);
                        if (!parsedDouble)
                            message += "'" + values[i].Letter + "' tipas turi būti '" + values[i].Type.DisplayName() + "'." + System.Environment.NewLine;
                        break;
                    case DataTypeEnum.Integer:
                        var parsedInt = int.TryParse(values[i].Value, out var resInt);
                        if (!parsedInt)
                            message += "'" + values[i].Letter + "' tipas turi būti '" + values[i].Type.DisplayName() + "'." + System.Environment.NewLine;
                        break;
                    case DataTypeEnum.String:
                        var parsedStr = values[i].Value as string;
                        if (parsedStr == null)
                            message += "'" + values[i].Letter + "' tipas turi būti '" + values[i].Type.DisplayName() + "'." + System.Environment.NewLine;
                        break;
                    case DataTypeEnum.ImageAsByteArray:
                        try
                        {
                            Encoding.UTF8.GetBytes(values[i].Value);
                        }
                        catch
                        {
                            message += "'" + values[i].Letter + "' tipas turi būti '" + values[i].Type.DisplayName() + "'." + System.Environment.NewLine;
                        }
                        break;
                    case DataTypeEnum.ImageAsBase64String:
                        var val = values[i].Value.Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", "");
                        try
                        {
                            Convert.FromBase64String(val);
                        }
                        catch
                        {
                            message += "'" + values[i].Letter + "' tipas turi būti '" + values[i].Type.DisplayName() + "'." + System.Environment.NewLine;
                        }
                        break;
                }
            }

            if (!String.IsNullOrEmpty(message))
                return new HttpRequestResult<bool>(message);

            return new HttpRequestResult<bool>(true);
        }
    }
}
