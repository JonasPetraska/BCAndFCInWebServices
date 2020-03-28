using Common.Models;
using Common.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class HttpClientService : IHttpClientService
    {
        private static readonly Lazy<HttpClient> httpClientlazy = new Lazy<HttpClient>(() => new HttpClient() { Timeout = TimeSpan.FromMinutes(10) });
        public static HttpClient HttpClient { get { return httpClientlazy.Value; } }

        private static readonly Lazy<IHttpClientService> lazy = new Lazy<IHttpClientService>(() => new HttpClientService());
        public static IHttpClientService Instance { get { return lazy.Value; } }

        public async Task<HttpRequestResult<T>> GetAsync<T>(string url)
        {
            var response = await HttpClient.GetAsync(url).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {

                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var contents = await StreamToStringAsync(stream).ConfigureAwait(false);
                return new HttpRequestResult<T>(response.StatusCode, contents);
            }
            else
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return new HttpRequestResult<T>()
                {
                    isSuccessful = true,
                    result = DeserializeJsonFromStream<T>(stream)
                };
            }
        }

        public HttpRequestResult<T> Get<T>(string url)
        {
            return GetAsync<T>(url).Result;
        }

        public async Task<HttpRequestResult<T>> PostAsync<T, V>(string url, V value, string mediaType = "application/json")
        {
            var content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, mediaType);
            var response = await HttpClient.PostAsync(url, content).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var contents = await StreamToStringAsync(stream).ConfigureAwait(false);
                return new HttpRequestResult<T>(response.StatusCode, contents);
            }
            else
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var contents = await StreamToStringAsync(stream).ConfigureAwait(false);
                return new HttpRequestResult<T>()
                {
                    isSuccessful = true,
                    result = JsonConvert.DeserializeObject<T>(contents)
                };
            }
        }

        public HttpRequestResult<T> Post<T, V>(string url, V value, string mediaType = "application/json")
        {
            return PostAsync<T, V>(url, value, mediaType).Result;
        }

        public async Task<HttpRequestResult<T>> PutAsync<T, V>(string url, V value, string mediaType = "application/json")
        {
            var content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, mediaType);
            var response = await HttpClient.PutAsync(url, content).ConfigureAwait(false);


            if (!response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var contents = await StreamToStringAsync(stream).ConfigureAwait(false);
                return new HttpRequestResult<T>(response.StatusCode, contents);
            }
            else
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return new HttpRequestResult<T>()
                {
                    isSuccessful = true,
                    result = DeserializeJsonFromStream<T>(stream)
                };
            }
        }

        public HttpRequestResult<T> Put<T, V>(string url, V value, string mediaType = "application/json")
        {
            return PutAsync<T, V>(url, value, mediaType).Result;
        }

        public async Task<HttpRequestResult<T>> DeleteAsync<T>(string url)
        {
            var response = await HttpClient.DeleteAsync(url).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var contents = await StreamToStringAsync(stream).ConfigureAwait(false);
                return new HttpRequestResult<T>(response.StatusCode, contents);
            }
            else
            {
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return new HttpRequestResult<T>()
                {
                    isSuccessful = true,
                    result = DeserializeJsonFromStream<T>(stream)
                };
            }
        }

        public HttpRequestResult<T> Delete<T>(string url)
        {
            return DeleteAsync<T>(url).Result;
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync().ConfigureAwait(false);

            return content;
        }
        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
        }
    }

}
