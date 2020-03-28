using Common.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public class FileProgramService : IProgramService
    {
        private string _fileName;
        public FileProgramService(IWebHostEnvironment env)
        {
            _fileName = Path.Combine(env.ContentRootPath, "Programs.txt");

            if (!System.IO.File.Exists(_fileName))
                System.IO.File.Create(_fileName);
        }

        public Task<HttpRequestResult<Common.Models.Program>> AddProgram(Common.Models.Program program)
        {
            var fileContents = System.IO.File.ReadAllText(_fileName);
            var items = new List<Common.Models.Program>();
            if (!String.IsNullOrEmpty(fileContents))
            {
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<List< Common.Models.Program>> (fileContents);
                    items = deserialized;
                }
                catch { }
            }

            var highestIdNode = items.OrderByDescending(x => x.Id).FirstOrDefault();
            var id = highestIdNode == null ? 1 : highestIdNode.Id + 1;

            program.Id = id;
            items.Add(program);

            try
            {
                System.IO.File.WriteAllText(_fileName, JsonConvert.SerializeObject(items));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HttpRequestResult<Common.Models.Program>(ex.StackTrace));
            }

            return Task.FromResult(new HttpRequestResult<Common.Models.Program>(program));
        }

        public Task<HttpRequestResult<List<Common.Models.Program>>> GetAllPrograms()
        {
            var fileContents = System.IO.File.ReadAllText(_fileName);
            if (!String.IsNullOrEmpty(fileContents))
            {
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<List< Common.Models.Program>> (fileContents);
                    return Task.FromResult(new HttpRequestResult<List<Common.Models.Program>>(deserialized));
                }
                catch { return Task.FromResult(new HttpRequestResult<List<Common.Models.Program>>(new List<Common.Models.Program>())); }
            }

            return Task.FromResult(new HttpRequestResult<List<Common.Models.Program>>(new List<Common.Models.Program>()));
        }

        public Task<HttpRequestResult<Common.Models.Program>> RemoveProgram(int id)
        {
            var fileContents = System.IO.File.ReadAllText(_fileName);
            var items = new List<Common.Models.Program>();
            if (!String.IsNullOrEmpty(fileContents))
            {
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<List<Common.Models.Program>> (fileContents);
                    items = deserialized;
                }
                catch { }
            }

            var program = items.FirstOrDefault(x => x.Id == id);

            var removedItemsCount = items.RemoveAll(x => x.Id == id);

            if (removedItemsCount > 0)
            {
                try
                {
                    System.IO.File.WriteAllText(_fileName, JsonConvert.SerializeObject(items));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new HttpRequestResult<Common.Models.Program>(ex.StackTrace));
                }

            }
            return Task.FromResult(new HttpRequestResult<Common.Models.Program>(program));
        }
    }
}
