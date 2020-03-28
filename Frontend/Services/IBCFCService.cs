using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public interface IBCFCService
    {
        Task<HttpRequestResult<ResponseModel>> BackwardTrackAsync(RequestModel model);
        Task<HttpRequestResult<ResponseModel>> ForwardTrackAsync(RequestModel model);
    }
}
