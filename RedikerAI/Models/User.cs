using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedikerAI.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string SchoolName { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
		public string PasswordHash { get; set; }
	}

}