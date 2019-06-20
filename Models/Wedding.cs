using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace wedding_planner.Models
{
	public class Wedding
	{
		[Key]
		public int WeddingId { get; set; }
		public string Groom { get; set; }
		public string Bride { get; set; }
		public DateTime WeddingDate { get; set; }
		public string Address { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public List<Rsvp> Guests { get; set; }
		// List<RSVP> Guest is the name, u can call it whatever u want to call.
	}

}