using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models.SearchRoomViewModels
{
    public class SearchRoom
    {
        public int userBedCount { get; set; }
        [DataType(DataType.Date)]
        public DateTime userCheckIn { get; set; }
        [DataType(DataType.Date)]
        public DateTime userCheckOut { get; set; }
    }
}
