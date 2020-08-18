using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models.BookingViewModels
{
    public class Statistics
    {
        public IEnumerable<CusPostcodeStat> CalcPostcodeStats { get; set; }
        public IEnumerable<BookRoomStat> RoomStats { get; set; }

    }
}
