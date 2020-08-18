using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models.BookingViewModels
{
    public class BookRoom
    {
        public int RoomID { get; set; }
        [DataType(DataType.Date)]
        public DateTime userCheckIn { get; set; }
        [DataType(DataType.Date)]
        public DateTime userCheckOut { get; set; }
    }
}
