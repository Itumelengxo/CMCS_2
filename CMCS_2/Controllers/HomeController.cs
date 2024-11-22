using CMCS_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace CMCS_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                Connection con = new Connection();

                using (SqlConnection connect = new SqlConnection(con.connecting()))
                {
                    connect.Open();
                    Console.WriteLine("Connected");
                    connect.Close();
                }
            }
            catch (IOException error)
            {
                Console.WriteLine("Error: " + error.Message);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Register_user(Register add_user)
        {
            string message = add_user.insert_user(add_user.Username, add_user.Email, add_user.Password, add_user.Role);

            if (message == "done")
            {
                TempData["Message"] = "User registered successfully.";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                TempData["Error"] = "Registration failed. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login_user(Check_login user)
        {
            string message = user.login_user(user.Email, user.Role, user.Password);

            if (message == "found")
            {
                TempData["Message"] = "Login successful.";
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                TempData["Error"] = "Invalid credentials.";
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public IActionResult Claim_Sub(IFormFile file, Claim insert)
        {
            string filename = SaveUploadedFile(file);

            if (filename == null)
            {
                TempData["Error"] = "File upload failed.";
                return RedirectToAction("Dashboard", "Home");
            }

            string message = insert.insert_claim(insert.User_Name, insert.Hours_worked, insert.Hour_Rate, insert.Description, filename);

            if (message == "done")
            {
                // Auto-approve claim if it meets criteria
                bool isAutoApprovable = CheckAutoApprovalCriteria(insert.Hours_worked, insert.Hour_Rate);

                if (isAutoApprovable)
                {
                    AutoApproveClaim(insert.ClaimId);
                    TempData["Message"] = "Claim submitted and auto-approved.";
                }
                else
                {
                    TempData["Message"] = "Claim submitted successfully. Pending approval.";
                }
            }
            else
            {
                TempData["Error"] = "Error submitting claim. Please try again.";
            }

            return RedirectToAction("Dashboard", "Home");
        }

        private string SaveUploadedFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string filename = Path.GetFileName(file.FileName);
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, filename);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        Console.WriteLine($"File {filename} uploaded successfully.");
                        return filename;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error uploading file: " + ex.Message);
                    return null;
                }
            }

            return null;
        }

        private bool CheckAutoApprovalCriteria(string hoursWorked, string hourlyRate)
        {
            int maxHours = 40; // Example criteria: Maximum hours for auto-approval
            decimal minRate = 50.0m;
            decimal maxRate = 200.0m;

            if (int.TryParse(hoursWorked, out int hours) &&
                decimal.TryParse(hourlyRate, out decimal rate))
            {
                return hours <= maxHours && rate >= minRate && rate <= maxRate;
            }

            return false;
        }

        private void AutoApproveClaim(string claimId)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(new Connection().connecting()))
                {
                    connect.Open();
                    string query = "UPDATE claiming SET status = 'Approved' WHERE claim_id = @id;";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@id", claimId);
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine($"Claim {claimId} auto-approved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error auto-approving claim: " + ex.Message);
            }
        }

        private void UpdateClaimStatus(string claimId, string status)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(new Connection().connecting()))
                {
                    connect.Open();
                    string query = "UPDATE claiming SET status = @status WHERE claim_id = @id;";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@id", claimId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating claim status: " + ex.Message);
            }
        }
    }
}
