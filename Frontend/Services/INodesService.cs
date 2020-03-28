using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public interface INodesService
    {
        Task<HttpRequestResult<List<Node>>> GetAllNodes();
        Task<HttpRequestResult<Node>> AddNode(Node node);
        Task<HttpRequestResult<Node>> RemoveNode(int id);
    }
}
