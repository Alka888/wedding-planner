using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//added these line
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using wedding_planner.Models;


namespace wedding_planner.Controllers
{
	public class HomeController : Controller
	{
		private MyContext dbContext;

		public HomeController(MyContext context)
		{
			dbContext = context;
		}
		////////////////home page////////////////
		[Route("")]
		[HttpGet]
		public IActionResult Index()
		{

			return View();
		}

		/////////////method for registration, when you hit the register button, this method runs////

		[HttpPost("register")]
		public IActionResult register(User user)
		{
			// Check initial ModelState
			if (ModelState.IsValid)
			{
				// If a User exists with provided email
				if (dbContext.Users.Any(u => u.Email == user.Email))
				{
					// Manually add a ModelState error to the Email field, with provided
					// error message
					ModelState.AddModelError("Email", "Email already in use!");
					// You may consider returning to the View at this point
					return View("Index");
				}
				PasswordHasher<User> Hasher = new PasswordHasher<User>();
				user.Password = Hasher.HashPassword(user, user.Password);
				dbContext.Add(user);
				dbContext.SaveChanges();
				HttpContext.Session.SetInt32("userInSession", user.UserId);
				return RedirectToAction("Index");
			}
			return View("Index");
		}
		// other code


		////////////this method is for login method, when you hit the login button this code runs/////////

		[HttpPost("login")]
		public IActionResult LoginUser(LoginUser userSubmission)
		{
			if (ModelState.IsValid)
			{
				// If inital ModelState is valid, query for a user with provided email
				var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
				// If no user exists with provided email
				if (userInDb == null)
				{
					// Add an error to ModelState and return to View!
					ModelState.AddModelError("Email", "Invalid Email/Password");
					return View("Index");
				}

				// Initialize hasher object
				var hasher = new PasswordHasher<LoginUser>();

				// varify provided password against hash stored in db
				var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);

				// result can be compared to 0 for failure
				if (result == 0)
				{
					ModelState.AddModelError("Password", "Invalid Password");
					return View("Index");
					// handle failure (this should be similar to how "existing email" is handled)
				}
				HttpContext.Session.SetInt32("userInSession", userInDb.UserId);

				HttpContext.Session.SetString("User", userInDb.FirstName);


				return RedirectToAction("Dashboard");
			}
			else
			{
				return View("Index");
			}
		}

		///////////////Logout method//////////////////

		[Route("Logout")]
		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}

		///////////////////Start working on the wedding - dashboard, this page is showing the list of wedding!////////////

		[Route("dashboard")]
		[HttpGet]
		public IActionResult Dashboard()
		{
			if (HttpContext.Session.GetInt32("userInSession") == null)
				return RedirectToAction("Index");
			User CurrentUser = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("userInSession"));
			List<Wedding> AllWedding = dbContext.Weddings
			.Include(wedding => wedding.Guests)
			.ThenInclude(Guest => Guest.User).ToList();

			List<Rsvp> UserRsvp = dbContext.Rsvps
			.Where(Rsvp => Rsvp.User.Equals(CurrentUser)).ToList();

			ViewBag.id = HttpContext.Session.GetInt32("userInSession");
			ViewBag.CurrentUser = CurrentUser;
			ViewBag.AllWeddings = AllWedding;
			ViewBag.UserRsvp = UserRsvp;
			return View();
		}

		////////////////////////showing page where i can create a new wedding/////////////

		[Route("Newwedding")]
		[HttpGet]
		public IActionResult Newwedding()
		{
			if (HttpContext.Session.GetInt32("userInSession") == null)
			{
				return RedirectToAction("Index");
			}
			ViewBag.UserId = HttpContext.Session.GetInt32("userInSession");
			
			return View();
		}


		////////////////THAN I HIT THE ADDWEDDING BUTTON, IT IS TAKING ME TO THE PAGE WITH DETAILS ABOUT WEDDING//////////

		[Route("AddWedding")]
		[HttpPost]
		public IActionResult AddWedding(Wedding Wedform)
		{
			Console.WriteLine("//////////////////////////here//////////////////");
			Console.WriteLine(Wedform.UserId);
			if (Wedform.WeddingDate < DateTime.Now)
			{
				ModelState.AddModelError("WeddingDate", "Wedding must be in the future");
			}
			if (ModelState.IsValid)
			{
				Console.WriteLine("//////////////////////////here//////////////////");
				dbContext.Add(Wedform);
				dbContext.SaveChanges();
				return RedirectToAction("Dashboard");
			}
			else
			{
				Console.WriteLine("///////////////// MODEL INVALID /////////////");
				ViewBag.errors = ModelState.Values;
				return View("Newwedding");
			}
		}

		

		//////////////////when i click the link, it showing details about the wedding, date, address, who is the bride and groom/////////
		[HttpGet]
		[Route("Wedding/{WeddingId}")]
		public IActionResult Wedding(int WeddingId)
		{

			Wedding CurrentWedding = dbContext.Weddings
			.Include(wedding => wedding.Guests)
			.ThenInclude(Guest => Guest.User)
			.FirstOrDefault(a => a.WeddingId == WeddingId);
			ViewBag.CurrentWedding = CurrentWedding;

			ViewBag.CurrentWedding = CurrentWedding;
			return View("Wedding");
		}

		[HttpGet("rsvp/{wid}/{uid}")]
        public IActionResult AddRsvp(int wid, int uid)
        {
			Wedding newWed = dbContext.Weddings.Include(a => a.Guests).ThenInclude(b => b.User).FirstOrDefault(wed => wed.WeddingId == wid);
            User newUser = dbContext.Users.Include(a => a.listofrcvp).ThenInclude(b => b.Wedding).FirstOrDefault(us => us.UserId == uid);

			foreach(var wedding in newUser.listofrcvp)
            {
                if(wedding.Wedding.WeddingDate.Date == newWed.WeddingDate.Date)
                {
                    ModelState.AddModelError("WeddingDate", "Date Cannot be in the past");
                    return RedirectToAction("Dashboard", new { id = HttpContext.Session.GetInt32("userInSession") });

                }
            }
			Rsvp add = new Rsvp();
            add.WeddingId = wid;
            add.UserId = uid;
            // add.User = dbContext.Users.FirstOrDefault(a => a.UserId == uid);
            // add.Wedding = dbContext.Weddings.FirstOrDefault(b => b.WeddingId == wid);
            dbContext.Add(add);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
		}

		[HttpGet("cancel/{id}")]
		public IActionResult DeleteRsvp(int id)
		{
			List<Rsvp> canceled = dbContext.Rsvps.Where(a => a.WeddingId == id).ToList();
			Rsvp first = canceled.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("userInSession"));
			dbContext.Remove(first);
			dbContext.SaveChanges();
			return RedirectToAction("Dashboard");
		}


		[HttpGet("delete/{wid}")]
        public IActionResult Delete(int wid)
        {
            Wedding deleted = dbContext.Weddings.FirstOrDefault(a => a.WeddingId == wid);
            dbContext.Remove(deleted);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }


	}


}




