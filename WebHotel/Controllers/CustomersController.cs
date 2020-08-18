using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;

namespace WebHotel.Controllers
{
    [Authorize(Roles = "Customers")]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customerss/MyDetails
        public async Task<IActionResult> MyDetails()
        {
            // retrieve the logged-in user's email
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            // check whether the user already exists in database
            var customer = await _context.Customer.FindAsync(_email);

            if (customer == null)
            {
                // if the customerr's record doesn't exist yet:
                // validation will not be checked when creating an object, but in web form
                customer = new Customer { Email = _email };
                return View("~/Views/Customers/MyDetailsCreate.cshtml", customer);
            }
            else
            {
                // if the customer's record already exists
                return View("~/Views/Customers/MyDetailsUpdate.cshtml", customer);
            }
        }

        // POST: Customer's/MyDetailsCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyDetailsCreate([Bind("Email,FamilyName,GivenName,Postalcode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();

                return View("~/Views/Customers/MyDetailsSuccess.cshtml", customer);
            }
            return View(customer);
        }
        // GET: Customers/PeopleDiff
 


        // POST: Customers/MyDetailsCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyDetailsUpdate([Bind("Email,FamilyName,GivenName,Postalcode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

                return View("~/Views/Customers/MyDetailsSuccess.cshtml", customer);
            }
            return View(customer);
        }

        private bool CustomerExists(string id)
        {
            return _context.Customer.Any(e => e.Email == id);
        }
    }
}

