using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncFunctionApp.Models
{
    public class MappingModel
    {
        public string ReferenceType { get; set; }
        public string SqlQuery { get; set; }
        public string DocumentId { get; set; }
        public string PartitionValue { get; set; }
    }
}
