using Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Endpoint { get; set; }
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
                return new HttpRequestResult<bool>("Endpoint should start with https (ex.: https://example.com).");

            if (!HealthEndPoint.StartsWith("https"))
                return new HttpRequestResult<bool>("Health endpoint should start with https (ex.: https://example.com).");

            if(!Uri.TryCreate(Endpoint, UriKind.Absolute, out var url1) && url1.Scheme == Uri.UriSchemeHttps)
            {
                return new HttpRequestResult<bool>("Endpoint is not a valid url.");
            }

            if (!Uri.TryCreate(HealthEndPoint, UriKind.Absolute, out var url2) && url2.Scheme == Uri.UriSchemeHttps)
            {
                return new HttpRequestResult<bool>("Health endpoint is not a valid url.");
            }

            if(url1.Host != url2.Host)
            {
                return new HttpRequestResult<bool>("Endpoint and endpoint health urls are not of the same host.");
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
                return new HttpRequestResult<bool>("Health endpoint is not reachable. Please check that the url is right. Also, only POST method is acceptable for the endpoint.");
            }

            return new HttpRequestResult<bool>(true);
        }

        public static HttpRequestResult<bool> ValidateRule(string rule)
        {
            if(String.IsNullOrEmpty(rule))
                return new HttpRequestResult<bool>("Rule is required.");

            if (!rule.Contains("->"))
                return new HttpRequestResult<bool>("Rules has to be in format: A->B.");

            return new HttpRequestResult<bool>(true);
        }

        public HttpRequestResult<bool> ValidateInputDataTypes()
        {
            if (InputDataType.Count() == Rule.LeftSide.Count())
                return new HttpRequestResult<bool>(true);

            if (InputDataType.Count() > Rule.LeftSide.Count())
                return new HttpRequestResult<bool>("Too many data input types.");

            if (InputDataType.Count() < Rule.LeftSide.Count())
                return new HttpRequestResult<bool>("Not enought input types.");

            return new HttpRequestResult<bool>("Invalid data types.");
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
