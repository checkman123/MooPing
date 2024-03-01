using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooPing.Database.Entities
{
	public class Transaction
	{
		[Key]
		public int TransactionId { get; set; }
		[ForeignKey("Category")]
		public int CategoryID { get; set; }
		[ForeignKey("User")]
		public int UserID { get; set; }
		[Required]
		[StringLength(200)]
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public DateTime Date { get; set; }
        public bool IsRecurring { get; set; }
        public RecurrenceFrequency RecurrenceFrequency { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Category? Category { get; set; }
		public virtual User? User { get; set; }
	}
}
