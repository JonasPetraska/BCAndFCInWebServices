using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public interface IProgramService
    {
        Task<HttpRequestResult<List<Common.Models.Program>>> GetAllPrograms();
        Task<HttpRequestResult<Common.Models.Program>> AddProgram(Common.Models.Program program);
        Task<HttpRequestResult<Common.Models.Program>> RemoveProgram(int id);
    }
}
