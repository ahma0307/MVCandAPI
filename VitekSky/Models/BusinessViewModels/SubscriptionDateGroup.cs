using System;
using System.ComponentModel.DataAnnotations;

namespace VitekSky.Models.BusinessViewModels
{
    public class SubscriptionDateGroup
    {
        [DataType(DataType.Date)]
        public DateTime? SubscriptionDate { get; set; }

        public int CustomerCount { get; set; }
    }
}