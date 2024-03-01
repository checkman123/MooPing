using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooPing.Database.Entities
{
	public class Category
	{
		public int CategoryId { get; set; }
		public string Name { get; set; }
		public virtual ICollection<Transaction>? Transactions { get; set; }
	}
}
