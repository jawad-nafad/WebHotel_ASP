using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebHotel.Models
{
    public class Room
    {
        public int ID { get; set; }

        [Required (ErrorMessage = "Shosen level should be in between G or 1-3.")]
        [RegularExpression(@"[G,1-3]")]
        public string Level { get; set; }

        [RegularExpression(@"^[1-3]$", ErrorMessage = "Maximum bed size is 3")]
        public int BedCount { get; set; }

        [Range(50, 300, ErrorMessage = "Booking costs between $50 and $300!")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        // Navigation properties
        public ICollection<Booking> TheBookings { get; set; }
    }
}
