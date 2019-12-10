using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitekSky.Models.BusinessViewModels
{
    public class AssignedProductData
    {
        public int ProductID { get; set; }
        public string ProductName{ get; set; }
        public bool Assigned { get; set; }
    }
}