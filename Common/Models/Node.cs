using Common.Enums;
using Common.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Node
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Endpoint { get; set; }

        [Required]
        public string HealthEndPoint { get; set; }

        public Rule Rule { get; set; }
        public List<DataTypeEnum> InputDataType { get; set; }
        public DataTypeEnum OutputDataType { get; set; }

        /// <summary>
        /// Validates if the endpoint is in correct format and reachable.
        /// </summary>
        /// <returns>true - if endpoint is in correct format and reachable. false - otherwise</returns>
        public async Task<HttpRequestResult<bool>> ValidateEndpoint()
        {
            if (!Endpoint.StartsWith("https"))
                return new HttpRequestResult<bool>("Adresas turi prasidėti https schema (pvz: https://example.com).");

            if (!HealthEndPoint.StartsWith("https"))
                return new HttpRequestResult<bool>("Ping-pong adresas turi prasidėti https schema (pvz: https://example.com).");

            if(!Uri.TryCreate(Endpoint, UriKind.Absolute, out var url1) && url1.Scheme == Uri.UriSchemeHttps)
            {
                return new HttpRequestResult<bool>("Blogas adresas.");
            }

            if (!Uri.TryCreate(HealthEndPoint, UriKind.Absolute, out var url2) && url2.Scheme == Uri.UriSchemeHttps)
            {
                return new HttpRequestResult<bool>("Blogas ping-pong adresas.");
            }

            if(url1.Host != url2.Host)
            {
                return new HttpRequestResult<bool>("Adresas ir ping-pong adresas turi būti viena ir ta pati svetainė.");
            }


            try
            {

                using(var client = new HttpClient())
                {
                    var response = await client.PostAsync(HealthEndPoint, new StringContent(""));
                    if (!response.IsSuccessStatusCode)
                        return new HttpRequestResult<bool>(response.StatusCode);

                }
            }
            catch (Exception ex)
            {
                return new HttpRequestResult<bool>("Ping-pong adresas nepasiekiamas. Patikrinkite ar jis priima POST tipo užklausas.");
            }

            return new HttpRequestResult<bool>(true);
        }

        public static HttpRequestResult<bool> ValidateRule(string rule)
        {
            if(String.IsNullOrEmpty(rule))
                return new HttpRequestResult<bool>("Laukas 'Taisyklė' yra privalomas.");

            if (!rule.Contains("->"))
                return new HttpRequestResult<bool>("Laukas 'Taisyklė' turi būti formato A->B.");

            if (rule.Split(new string[] { "->" }, StringSplitOptions.None)[1].Split(',').Length > 1)
                return new HttpRequestResult<bool>("Taisyklėje leidžiamas tik vienas konsekventas.");

            if (rule.Distinct().Count(x => x != ',') != rule.Count(x => x != ','))
                return new HttpRequestResult<bool>("Pasikartojantys antecendantai neleidžiami.");

            return new HttpRequestResult<bool>(true);
        }

        public HttpRequestResult<bool> ValidateInputDataTypes()
        {
            if (InputDataType.Count() == Rule.LeftSide.Count())
                return new HttpRequestResult<bool>(true);

            if (InputDataType.Count() > Rule.LeftSide.Count())
                return new HttpRequestResult<bool>("Per daug antecedentų.");

            if (InputDataType.Count() < Rule.LeftSide.Count())
                return new HttpRequestResult<bool>("Nepakankamai antecedentų.");

            return new HttpRequestResult<bool>("Netinkami antecedentai.");
        }

        public HttpRequestResult<bool> ValidateInputValues(List<string> values)
        {
            if (InputDataType.Count != values.Count)
                return new HttpRequestResult<bool>("Reikšmių kiekis per mažas.");

            var message = "";

            for(int i = 0; i < values.Count; i++)
            {
                switch (InputDataType[i])
                {
                    case DataTypeEnum.Double:
                        var parsedDouble = Double.TryParse(values[i], out var resDouble);
                        if (!parsedDouble)
                            message += "'" + values[i] + "' tipas turi būti '" + InputDataType[i].DisplayName() + "'." + System.Environment.NewLine;
                        break;
                    case DataTypeEnum.Integer:
                        var parsedInt = int.TryParse(values[i], out var resInt);
                        if(!parsedInt)
                            message += "'" + values[i] + "' tipas turi būti '" + InputDataType[i].DisplayName() + "'." + System.Environment.NewLine;
                        break;
                    case DataTypeEnum.String:
                        var parsedStr = values[i] as string;
                        if (parsedStr == null)
                            message += "'" + values[i] + "' tipas turi būti '" + InputDataType[i].DisplayName() + "'." + System.Environment.NewLine;
                        break;
                    case DataTypeEnum.ImageAsByteArray:
                        try
                        {
                            Encoding.UTF8.GetBytes(values[i]);
                        }
                        catch
                        {
                            message += "'" + Rule.LeftSide[i] + "' tipas turi būti '" + InputDataType[i].DisplayName() + "'." + System.Environment.NewLine;
                        }
                        break;
                    case DataTypeEnum.ImageAsBase64String:
                        var val = values[i].Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", "");
                        try
                        {
                            Convert.FromBase64String(val);
                        }
                        catch
                        {
                            message += "'" + Rule.LeftSide[i] + "' tipas turi būti '" + InputDataType[i].DisplayName() + "'." + System.Environment.NewLine;
                        }
                        break;
                }
            }

            if (!String.IsNullOrEmpty(message))
                return new HttpRequestResult<bool>(message);

            return new HttpRequestResult<bool>(true);
        }

        public void SetRuleFromString(string rule)
        {
            if (!ValidateRule(rule).isSuccessful)
                return;

            var split = rule.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);

            var ruleObj = new Rule();
            ruleObj.LeftSide = split[0].Split(',').ToList().Select(x => Convert.ToChar(x)).ToList();
            ruleObj.RightSide = Convert.ToChar(split[1]);
            ruleObj.Number = Name;

            Rule = ruleObj;
        }

        public async Task<bool> IsHealthy()
        {

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(HealthEndPoint, new StringContent(""));
                if (!response.IsSuccessStatusCode)
                    return false;
            }

            return true;
        }

        public async Task<HttpRequestResult<NodeResponseModel>> GetNodeResultAsync(NodeRequestModel model)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return new HttpRequestResult<NodeResponseModel>(JsonConvert.DeserializeObject<NodeResponseModel>(responseContent));

                return new HttpRequestResult<NodeResponseModel>(response.StatusCode, responseContent);
            }
        }

        //public async Task<HttpRequestResult<NodeResponseModel>> GetRule(List<char> facts, char goal)
        //{
        //    var requestModel = new NodeRequestModel();
        //    requestModel.Facts = facts;
        //    requestModel.Goal = goal;

        //    using (var client = new HttpClient())
        //    {
        //        var content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");
        //        var response = await client.PostAsync(Endpoint, content);
        //        var content2 = await response.Content.ReadAsStringAsync();
        //        if (response.IsSuccessStatusCode)
        //            return new HttpRequestResult<NodeResponseModel>(JsonConvert.DeserializeObject<NodeResponseModel>(await response.Content.ReadAsStringAsync()));

        //        return new HttpRequestResult<NodeResponseModel>(response.StatusCode);
        //    }
        //}
    }
}
