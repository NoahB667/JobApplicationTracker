using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public string Company { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } // e.g., Applied, Interviewing, Offer, Rejected
        public DateTime? ApplicationDate { get; set; }
        public DateTime? InterviewDate { get; set; } // Nullable if not applicable
        public string? JobLink { get; set; } // Link to the job posting

        [Required]
        public string UserId { get; set; }
    }
}
