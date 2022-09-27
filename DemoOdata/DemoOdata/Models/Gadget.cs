using System;
using System.Collections.Generic;

namespace DemoOdata.Models
{
    public partial class Gadget
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Type { get; set; }
    }
}
