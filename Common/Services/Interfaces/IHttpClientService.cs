using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<HttpRequestResult<T>> GetAsync<T>(string url);
        HttpRequestResult<T> Get<T>(string url);
        Task<HttpRequestResult<T>> PostAsync<T, V>(string url, V value, string mediaType = "application/json");
        HttpRequestResult<T> Post<T, V>(string url, V value, string mediaType = "application/json");
        Task<HttpRequestResult<T>> PutAsync<T, V>(string url, V value, string mediaType = "application/json");
        HttpRequestResult<T> Put<T, V>(string url, V value, string mediaType = "application/json");
        Task<HttpRequestResult<T>> DeleteAsync<T>(string url);
        HttpRequestResult<T> Delete<T>(string url);
    }
}
