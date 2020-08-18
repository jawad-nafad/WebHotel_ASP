using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebHotel.Models.BookingViewModels
{
    public class CusPostcodeStat
    {
        public string Postalcode { get; set; }

        [Display(Name = "Number of Customers")]
        public int CustomerCount { get; set; }

    }
}
