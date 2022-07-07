using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Core.UseCases.IPS.Models
{
    public class IndoorPositionStatusDto
    {
        public string SiteId { get; set; }
        public DateTime TimeStamp { get; set; } 
        public ICollection<string> In { set; get; }
        public ICollection<string> Out { set; get; }
    }
}
