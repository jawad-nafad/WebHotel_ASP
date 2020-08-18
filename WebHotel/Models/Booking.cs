using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebHotel.Models
{
    public class Booking
    {
        // primary key
        public int ID { get; set; }

        // foreign key by naming convention
        public int RoomID { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        // foreign key by naming convention
        public string CustomerEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        public decimal Cost { get; set; }

        // Navigation properties
        public Room TheRoom { get; set; }
        public Customer TheCustomer { get; set; }
    }
}
