using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;
using WebHotel.Models.BookingViewModels;


namespace WebHotel.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Booking.Include(b => b.TheCustomer).Include(b => b.TheRoom);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

                 var booking = await _context.Booking
                 .Include(b => b.TheCustomer)
                 .Include(b => b.TheRoom)
                 .SingleOrDefaultAsync(m => m.ID == id);
           /* string query = "SELECT * FROM Room WHERE ID = @para0";
            var parameter0 = new SqliteParameter("para0", id);
            var booking = await _context.Booking.FromSql(query, parameter0).SingleOrDefaultAsync();*/


            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create

        [Authorize(Roles = "Customers, Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            var checkIn = DateTime.Today;
            var checkOut = checkIn.AddDays(1);

            string _email = User.FindFirst(ClaimTypes.Name).Value;
            var customer = await _context.Customer.FindAsync(_email);

            if (customer != null) // Redirect to BookARoom action
            {
                var booking = new Booking { CustomerEmail = _email };
                booking.CheckIn = checkIn;
                booking.CheckOut = checkOut;
                return View("~/Views/Bookings/BookingDiff.cshtml", booking);
            }
            else if (User.IsInRole("Admin")) // Stay in Create action
            {
                ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email");
                return View();
            }
            else // Redirect to MyDetails action to create a profile
            {
                return RedirectToAction("MyDetails", "Customers");
            }
            // ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email");
            //ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "Level");
            // return View();
        }


        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customers, Admin")]
        public async Task<IActionResult> Create([Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "Level", "ID", booking.RoomID);
            return View(booking);
        }
        // GET: Bookings/BookRoom
        [AllowAnonymous]
        [Authorize(Roles = "Customer")]
        public IActionResult BookRoom()
        {
            // Get the options for the Customers select list from the database
            // and save them in ViewBag for passing to View
            ViewBag.BookingList = new SelectList(_context.Room, "ID", "ID");
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        [AllowAnonymous]
        [Authorize(Roles = "Customers")]
        public async Task<IActionResult> BookRoom(BookRoom bookRoom)
        {
            // prepare the parameters to be inserted into the query
            var IDA = new SqliteParameter("ida", bookRoom.RoomID);
            var usercheckinA = new SqliteParameter("checkinA", bookRoom.userCheckIn);
            var usercheckoutA = new SqliteParameter("checkoutA", bookRoom.userCheckOut);
            
            var selectedRoom = _context.Booking.FromSql("select * from [Booking] where [Booking].RoomID=@ida in"
                              + "(select [Room].ID from [Room] inner join [Booking] on [Booking].RoomID = [Room].ID"
                             + " where  [Booking].CheckIN < @checkoutA and [Booking].CheckOut >@checkinA )", IDA, usercheckinA,usercheckoutA)
            .Select(b=> new Booking { RoomID=b.ID, CustomerEmail = b.CustomerEmail, CheckIn = b.CheckIn, CheckOut = b.CheckOut, Cost= b.Cost });

            // ViewBag.RowsAffected = await _context.Database.ExecuteSqlCommandAsync("UPDATE RoomID, CustomerEmail, CheckIn, CheckOut, Cost");

            // Run the query and save the results in ViewBag for passing to view
            ViewBag.SelectedRooms = await selectedRoom.ToListAsync();
            await _context.SaveChangesAsync();
            // Save the options for both dropdown lists in ViewBag for passing to view
            ViewBag.BookingList = new SelectList(_context.Room, "ID", "ID");
            // invoke the view with the ViewModel object
            return View(bookRoom);

        }




        // GET: Bookings/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "Level", booking.RoomID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (id != booking.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.ID))
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
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "Level", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.ID == id);
        }
        // GET: Bookings/MyBookings
        [Authorize(Roles = "Customers")]
        public async Task<IActionResult> MyBookings(string sortBooking)
        {
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            var customer = await _context.Customer.FindAsync(_email);

            if (customer != null)
            {
                if (String.IsNullOrEmpty(sortBooking))
                {
                    sortBooking = "checkIn_asc";
                }

                var myBookings = (IQueryable<Booking>)_context.Booking
                    .Where(b => b.CustomerEmail == _email)
                    .Include(b => b.TheRoom)
                    .Include(b => b.TheCustomer);

                switch (sortBooking)
                {
                    case "checkIn_asc":
                        myBookings = myBookings.OrderBy(b => b.CheckIn);
                        break;
                    case "checkIn_desc":
                        myBookings = myBookings.OrderByDescending(b => b.CheckIn);
                        break;
                    case "cost_asc":
                        myBookings = myBookings.OrderBy(b => b.Cost);
                        break;
                    case "cost_desc":
                        myBookings = myBookings.OrderByDescending(b => b.Cost);
                        break;
                }

                ViewData["NextCheckInBooking"] = sortBooking != "checkIn_asc" ? "checkIn_asc" : "checkIn_desc";
                ViewData["NextCostBooking"] = sortBooking != "cost_asc" ? "cost_asc" : "cost_desc";


                return View(await myBookings.AsNoTracking().ToListAsync());
            }
            else // Redirect customer to create a profile
            {
                return RedirectToAction("MyDetails", "Customers");
            }
        }

        // GET: Bookings/Statistics
        [Authorize(Roles = "Admin")]
        public IActionResult Statistics()
        {
            var customerGroup = _context.Customer.GroupBy(c => c.Postalcode);
            var postcodeStats = customerGroup.Select(p => new CusPostcodeStat { Postalcode = p.Key, CustomerCount = p.Count() });

            var bookingGroup = _context.Booking.GroupBy(b => b.RoomID);
            var roomStats = bookingGroup.Select(r => new BookRoomStat { RoomID = r.Key, BookingCount = r.Count() });

            Statistics st = new Statistics
            {
                CalcPostcodeStats = postcodeStats,
                RoomStats = roomStats
            };

            return View(st);
        }


    }


}