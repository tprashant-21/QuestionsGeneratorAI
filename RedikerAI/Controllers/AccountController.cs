using RedikerAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace RedikerAI.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserRepository _userRepo = new UserRepository();

		[HttpPost]
		public ActionResult Register(User user, string password)
		{
			if (ModelState.IsValid)
			{
				user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
				int userId = _userRepo.AddUser(user);

				// Redirect to login page or dashboard
				return RedirectToAction("Login");
			}

			return View(user);
		}

		[HttpGet]
		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(string email, string password)
		{
			var user = _userRepo.GetUserByEmail(email);
			if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
			{
				Session["UserId"] = user.Id;
				return RedirectToAction("Index", "Home"); 
			}

			ModelState.AddModelError("", "Invalid email or password.");
			return View();
		}

		[HttpGet]
		public ActionResult Login()
		{
			return View();

		}

		// Logout action to clear the session and redirect to Login page
		public ActionResult Logout()
		{
			// Clear the session (this will log the user out)
			Session.Clear();

			// Optionally, you can also abandon the session if you want
			Session.Abandon();

			// Redirect the user to the login page after logging out
			return RedirectToAction("Login", "Account");
		}

	}
}