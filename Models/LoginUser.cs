using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using wedding_planner.Models;
using System.Collections.Generic;

namespace wedding_planner.Models
{
	public class LoginUser
	{
		// No other fields!
		[Required(ErrorMessage = "Please enter correct email or change other email!")]
		[EmailAddress]
		public string LoginEmail { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string LoginPassword { get; set; }
	}
}