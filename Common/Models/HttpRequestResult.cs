using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Common.Models
{
    public class HttpRequestResult<T>
    {
        public HttpRequestResult() { }

        public HttpRequestResult(IEnumerable<HttpRequestResultError> errors)
        {
            isSuccessful = false;
            result = default(T);
            this.errors = errors;
        }

        public HttpRequestResult(T result)
        {
            isSuccessful = true;
            this.result = result;
            errors = null;
        }

        public HttpRequestResult(string message)
        {
            isSuccessful = false;
            result = default(T);
            errors = new List<HttpRequestResultError>()
            {
                new HttpRequestResultError()
                {
                    message = message,
                    code = HttpErrorEnum.GenericError
                }
            };
        }

        public HttpRequestResult(HttpStatusCode httpStatusCode)
        {
            isSuccessful = false;
            result = default(T);
            errors = new List<HttpRequestResultError>()
            {
                new HttpRequestResultError()
                {
                    message = httpStatusCode.ToString(),
                    code = (HttpErrorEnum)((int)httpStatusCode)
                }
            };
        }

        public HttpRequestResult(HttpStatusCode httpStatusCode, string message)
        {
            isSuccessful = false;
            result = default(T);
            errors = new List<HttpRequestResultError>()
            {
                new HttpRequestResultError()
                {
                    message = message,
                    code = (HttpErrorEnum)((int)httpStatusCode)
                }
            };
        }

        public HttpRequestResult(Exception ex)
        {
            isSuccessful = false;
            result = default(T);
            errors = new List<HttpRequestResultError>()
            {
                new HttpRequestResultError()
                {
                    message = ex.StackTrace,
                    code = HttpErrorEnum.Exception
                }
            };
        }

        public T result { get; set; }
        public bool isSuccessful { get; set; }
        public IEnumerable<HttpRequestResultError> errors { get; set; }

        public string ToStringErrors()
        {
            if (errors.Any())
            {
                var type = ConvertTypeToHumanReadableString(typeof(T).ToString());
                return (!String.IsNullOrEmpty(type) ? type + ": " : "") +
                       string.Join(Environment.NewLine,
                           errors.Select(x =>
                                   String.IsNullOrEmpty(x.message)
                                       ? x.code.ToString()
                                       : x.message.Trim().Trim('"', '\'').Replace("\\r\\n", System.Environment.NewLine))
                               .ToList());
            }

            return ConvertTypeToHumanReadableString(typeof(T).ToString()) + ": " + "An error has occured";
        }

        private string ConvertTypeToHumanReadableString(string type)
        {
            string result = "";

            int lastAppearanceOfDot = type.LastIndexOf('.');

            // Remove namespace
            type = type.Remove(0, lastAppearanceOfDot);

            // Replace list, arrays and etc.
            type = type.Replace("List", "");
            type = type.Replace("Array", "");
            type = type.Replace("System.", "");
            type = type.Replace("CollectionGeneric", "");
            type = type.Replace("Boolean", "");
            type = type.Replace("Double", "");
            type = type.Replace("int", "");

            // Replace other characters
            foreach (var letter in type)
                if (letter != '.' && letter != '<' && letter != '>' && letter != '[' && letter != ']')
                    result += letter;

            return result.Trim();

        }
    }

    public class HttpRequestResultError
    {
        public string message { get; set; }
        public HttpErrorEnum code { get; set; }
    }
}
