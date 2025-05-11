using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using JobApplicationTracker.Models;
using JobApplicationTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using JobApplicationTracker.Views.Home;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JobApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, JobApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Default route for displaying the Jobs page
        [HttpGet]
        public IActionResult Jobs()
        {
            var model = new JobsModel
            {
                Jobs = _context.Jobs.ToList(),
                Company = "",
                Status = "",
                StartDate = "",
                EndDate = "",
                Keyword = ""
            };

            return View(model);
        }

        // Route for filtering jobs
        [HttpGet("Jobs/Filter")]
        [Authorize]
        public IActionResult Jobs(string company, string status, DateTime? startDate, DateTime? endDate, string keyword)
        {
            var userId = _userManager.GetUserId(User);
            var query = _context.Jobs.Where(job => job.UserId == userId);

            // Apply filters
            if (!string.IsNullOrEmpty(company))
            {
                query = query.Where(job => job.Company.Contains(company));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(job => job.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(job => job.ApplicationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(job => job.ApplicationDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(job => job.Title.Contains(keyword) || (job.Description != null && job.Description.Contains(keyword)));
            }

            // Pass the status options to the view
            ViewBag.StatusOptions = new SelectList(new[]
            {
           new { Value = "", Text = "All" },
           new { Value = "Applied", Text = "Applied" },
           new { Value = "Interviewing", Text = "Interviewing" },
           new { Value = "Offer", Text = "Offer" },
           new { Value = "Rejected", Text = "Rejected" }
       }, "Value", "Text", status);

            // Create a JobsModel and populate it
            var model = new JobsModel
            {
                Jobs = query.ToList(),
                Company = company,
                Status = status,
                StartDate = startDate?.ToString("yyyy-MM-dd"),
                EndDate = endDate?.ToString("yyyy-MM-dd"),
                Keyword = keyword
            };

            return View("Jobs", model); // Explicitly specify the view name
        }

        [Authorize]
        public IActionResult CreateEditJob(int? Id)
        {
            if (Id != null)
            {
                // Edit -> Load job details from database
                var jobInDb = _context.Jobs.SingleOrDefault(job => job.Id == Id);
                if (jobInDb != null)
                {
                    return View(jobInDb);
                }
            }
            // Create -> Return empty job model
            return View(new Job());
        }

        [Authorize]
        public IActionResult DeleteJob(int Id)
        {
            var jobInDb = _context.Jobs.SingleOrDefault(job => job.Id == Id);
            if (jobInDb != null)
            {
                _context.Jobs.Remove(jobInDb);
                _context.SaveChanges();
            }
            return RedirectToAction("Jobs");
        }

        public IActionResult CreateEditJobForm(Job model)
        {
            var userId = _userManager.GetUserId(User); // Get the logged-in user's ID
            model.UserId = userId; // Associate the job with the logged-in user

            if (string.IsNullOrWhiteSpace(model.Description))
            {
                model.Description = "No description provided.";
            }

            if (string.IsNullOrWhiteSpace(model.JobLink))
            {
                model.JobLink = "No link provided.";
            }

            if (string.IsNullOrWhiteSpace(model.Status))
            {
                model.Status = "Applied";
            }
            if (model.Id == 0)
            {
                // Create new job
                _context.Jobs.Add(model);
            } else
            {
                // Edit existing job
                _context.Jobs.Update(model);
            }
            _context.SaveChanges();
            return RedirectToAction("Jobs");
        }

        [HttpPost]
        public IActionResult Create(Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Jobs.Add(job);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // If the model is invalid, return the same view with validation errors
            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, [FromServices] EmailSender emailSender)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

                    await emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>");

                    return RedirectToAction("RegisterConfirmation", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }

            return View("Error");
        }

        public async Task<IActionResult> TestEmail([FromServices] EmailSender emailSender)
        {
            await emailSender.SendEmailAsync("recipient@example.com", "Test Email", "This is a test email.");
            return Content("Email sent successfully.");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
                {
                    Port = int.Parse(emailSettings["Port"]),
                    Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        public IActionResult Dashboard()
        {
            // Example data for the pie chart (status distribution)
            var statusData = _context.Jobs
                .GroupBy(j => j.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            //ViewBag.StatusLabels = statusData.Select(s => s.Status).ToArray();
            //ViewBag.StatusData = statusData.Select(s => s.Count).ToArray();
            ViewBag.StatusLabels = new[] { "Applied", "Interviewing", "Offer", "Rejected", "Other" };
            ViewBag.StatusData = new[] { 10, 5, 2, 3, 1 };

            // Fetch data from the database and perform grouping in memory
            var monthData = _context.Jobs
                .Where(j => j.ApplicationDate.HasValue)
                .AsEnumerable() // Switch to client-side evaluation
                .GroupBy(j => j.ApplicationDate.Value.ToString("MMMM"))
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(m => DateTime.ParseExact(m.Month, "MMMM", null))
                .ToList();

            //ViewBag.MonthLabels = monthData.Select(m => m.Month).ToArray();
            //ViewBag.MonthData = monthData.Select(m => m.Count).ToArray();
            ViewBag.MonthLabels = new[] { "January", "February", "March", "April", "May" };
            ViewBag.MonthData = new[] { 5, 8, 12, 7, 10 };
            return View();
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
