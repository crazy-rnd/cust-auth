using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomActionFilter.Model;
using Microsoft.EntityFrameworkCore;
namespace CustomActionFilter.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> context) : base(context)
        {

        }

        public DbSet<Student> Students { get; set; }

    }
}
