using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;
// add this one to import the GenreStatistic class definition
using WebHotel.Models.SearchRoomViewModels;
// add this to support the type SqliteParameter 
using Microsoft.Data.Sqlite;

namespace WebHotel.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Room.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
              .SingleOrDefaultAsync(m => m.ID == id);
            /*string query = "SELECT * FROM Room WHERE ID = @para0";
            var parameter0 = new SqliteParameter("para0", id);
            var room = await _context.Booking.FromSql(query, parameter0).SingleOrDefaultAsync();*/
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Level,BedCount,Price")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        [AllowAnonymous]
        public IActionResult SearchRoom()
        {
            // Get the options for the Customers select list from the database
            // and save them in ViewBag for passing to View
           
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> SearchRoom (SearchRoom searchroom)
        {

            
            // prepare the parameters to be inserted into the query
            var bedCountA = new SqliteParameter("bed", searchroom.userBedCount);
            var checkInA = new SqliteParameter("in", searchroom.userCheckIn);
            var checkOutA = new SqliteParameter("out", searchroom.userCheckOut);
            var getRooms = _context.Room.FromSql("select * from [Room] where [Room].BedCount=@bed in"
                              + "(select [Booking].RoomID from [Booking] inner join [Room] on [Room].ID = [Booking].RoomID"                      
                             + " where  [Booking].CheckIN < @out and [Booking].CheckOut >@in )",bedCountA , checkInA, checkOutA)
                   .Select(mo => new Room { ID = mo.ID, Level = mo.Level, BedCount = mo.BedCount, Price = mo.Price });
            ViewBag.GetRooms = await getRooms.ToListAsync();
            return View(searchroom);

        }
            // GET: Rooms/Edit/5
            public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.SingleOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Level,BedCount,Price")] Room room)
        {
            if (id != room.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .SingleOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.SingleOrDefaultAsync(m => m.ID == id);
            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.ID == id);
        }
    }
}
