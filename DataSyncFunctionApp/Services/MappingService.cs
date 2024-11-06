using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Castle.Core.Configuration;
using DataSyncFunctionApp.Models;

namespace DataSyncFunctionApp.Services
{
    public class MappingService
    {
        public static List<MappingModel> LoadTableConfigurations(string filePath)
        {
            var jsonContent = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<MappingModel>>(jsonContent);
        }
    }
}
