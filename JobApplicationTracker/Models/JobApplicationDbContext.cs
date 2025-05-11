using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Models
{
    public class JobApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public JobApplicationDbContext(DbContextOptions<JobApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
    }
}
