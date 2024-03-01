using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooPing.Database.Entities
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		public string Username { get; set; }
		[EmailAddress]
		public string Email { get; set; }
		public virtual ICollection<Transaction>? Transactions { get; set; }
	}
}
