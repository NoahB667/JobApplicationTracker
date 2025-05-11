using JobApplicationTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JobApplicationTracker.Views.Home
{
    public class JobsModel : PageModel
    {
        public string Company { get; set; }
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Keyword { get; set; }

        // Add a property to hold the list of jobs
        public List<Job> Jobs { get; set; }

        public void OnGet()
        {
            // Retrieve query parameters
            Company = HttpContext.Request.Query["company"];
            Status = HttpContext.Request.Query["status"];
            StartDate = HttpContext.Request.Query["startDate"];
            EndDate = HttpContext.Request.Query["endDate"];
            Keyword = HttpContext.Request.Query["keyword"];

            // Example: Fetch jobs from the database (replace with actual logic)
            Jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Software Engineer", Company = "TechCorp", Location = "New York", Description = "Develop software", Status = "Applied", ApplicationDate = System.DateTime.Now, InterviewDate = null, JobLink = "https://example.com" },
                new Job { Id = 2, Title = "Data Analyst", Company = "DataCorp", Location = "San Francisco", Description = "Analyze data", Status = "Interview Scheduled", ApplicationDate = System.DateTime.Now.AddDays(-10), InterviewDate = System.DateTime.Now.AddDays(5), JobLink = "https://example.com" }
            };
        }
    }
}
