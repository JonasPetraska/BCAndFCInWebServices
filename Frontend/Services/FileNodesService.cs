using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Frontend.Services
{
    public class FileNodesService : INodesService
    {
        private string _fileName;
        public FileNodesService(IWebHostEnvironment env)
        {
            _fileName = Path.Combine(env.ContentRootPath, "Nodes.txt");

            if (!System.IO.File.Exists(_fileName))
                System.IO.File.Create(_fileName);
        }

        public Task<HttpRequestResult<Node>> AddNode(Node node)
        {
            var fileContents = System.IO.File.ReadAllText(_fileName);
            var items = new List<Node>();
            if (!String.IsNullOrEmpty(fileContents))
            {
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<List<Node>>(fileContents);
                    items = deserialized;
                }
                catch { }
            }

            var highestIdNode = items.OrderByDescending(x => x.Id).FirstOrDefault();
            var id = highestIdNode == null ? 1 : highestIdNode.Id + 1;

            node.Id = id;
            items.Add(node);

            try
            {
                System.IO.File.WriteAllText(_fileName, JsonConvert.SerializeObject(items));
            }
            catch(Exception ex)
            {
                return Task.FromResult(new HttpRequestResult<Node>(ex.StackTrace));
            }

            return Task.FromResult(new HttpRequestResult<Node>(node));
        }

        public Task<HttpRequestResult<List<Node>>> GetAllNodes()
        {
            var fileContents = System.IO.File.ReadAllText(_fileName);
            if (!String.IsNullOrEmpty(fileContents))
            {
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<List<Node>>(fileContents);
                    return Task.FromResult(new HttpRequestResult<List<Node>>(deserialized));
                }
                catch { return Task.FromResult(new HttpRequestResult<List<Node>>(new List<Node>())); }
            }

            return Task.FromResult(new HttpRequestResult<List<Node>>(new List<Node>()));
        }

        public Task<HttpRequestResult<Node>> RemoveNode(int id)
        {
            var fileContents = System.IO.File.ReadAllText(_fileName);
            var items = new List<Node>();
            if (!String.IsNullOrEmpty(fileContents))
            {
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<List<Node>>(fileContents);
                    items = deserialized;
                }
                catch { }
            }

            var node = items.FirstOrDefault(x => x.Id == id);

            var removedItemsCount = items.RemoveAll(x => x.Id == id);

            if (removedItemsCount > 0)
            {
                try
                {
                    System.IO.File.WriteAllText(_fileName, JsonConvert.SerializeObject(items));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new HttpRequestResult<Node>(ex.StackTrace));
                }

            }
            return Task.FromResult(new HttpRequestResult<Node>(node));
        }
    }
}
