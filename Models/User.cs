using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using wedding_planner.Models;
using System.Collections.Generic;

namespace wedding_planner.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[EmailAddress]
		[Required]
		public string Email { get; set; }
		[DataType(DataType.Password)]
		[Required]
		[MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
		public string Password { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdateAt { get; set; } = DateTime.Now;
		// Will not be mapped to your users table!
		[NotMapped]
		[Compare("Password")]
		[DataType(DataType.Password)]
		public string Confirm { get; set; }
		public List<Rsvp> listofrcvp { get; set; }

	}
}