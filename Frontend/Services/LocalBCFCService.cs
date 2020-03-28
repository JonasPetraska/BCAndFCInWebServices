using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;

namespace Frontend.Services
{
    public class LocalBCFCService : IBCFCService
    {
        public async Task<HttpRequestResult<ResponseModel>> BackwardTrackAsync(RequestModel model)
        {
            var service = new BackwardChainingAlgorithm(model);
            return new HttpRequestResult<ResponseModel>(service.Execute());
        }

        public async Task<HttpRequestResult<ResponseModel>> ForwardTrackAsync(RequestModel model)
        {
            var service = new ForwardChainingAlgorithm(model);
            return new HttpRequestResult<ResponseModel>(service.Execute());
        }
    }
}
