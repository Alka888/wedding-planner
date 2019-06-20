using Microsoft.EntityFrameworkCore;

namespace wedding_planner.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get;set;}
        public DbSet<Rsvp> Rsvps {get;set;}
        public DbSet<Wedding> Weddings {get;set;}
    }
}