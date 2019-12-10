using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitekSky.Models
{
    public class Customer : Person
    {
               
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Subscription Date")]
        public DateTime SubscriptionDate { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }
    }
}