using Microsoft.EntityFrameworkCore;
using MooPing.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooPing.Database
{
	public class MooPingDbContext : DbContext
	{
		#region Constructors

		public MooPingDbContext() { }

		public MooPingDbContext(DbContextOptions<MooPingDbContext> options) : base(options) { }

		#endregion

		#region DbSets
		public DbSet<Category> Category { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		#endregion
	}
}
