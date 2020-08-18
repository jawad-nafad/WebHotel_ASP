using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebHotel.Models.BookingViewModels
{
    public class BookRoomStat
    {

        [Display(Name = "Room ID")]
        public int RoomID { get; set; }

        [Display(Name = "Number of Bookings")]
        public int BookingCount { get; set; }
    }
}
