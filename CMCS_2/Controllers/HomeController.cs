using CMCS_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

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
            // First check the connection
            try
            {
                Connection con = new Connection();

                // Check connection
                using (SqlConnection connect = new SqlConnection(con.connecting()))
                {
                    // Open the connection
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

        // Register POST method
        [HttpPost]
        public IActionResult Register_user(Register add_user)
        {
            // Collect user data
            string name = add_user.Username;
            string email = add_user.Email;
            string password = add_user.Password;
            string role = add_user.Role;

            // Pass all values to insert method
            string message = add_user.insert_user(name, email, password, role);

            // Check if the user is inserted
            if (message == "done")
            {
                Console.WriteLine(message);
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Console.WriteLine(message);
                return RedirectToAction("Index", "Home");
            }
        }

        // Login page
        public IActionResult Login()
        {
            return View();
        }

        // Login POST method
        [HttpPost]
        public IActionResult Login_user(Check_login user)
        {
            string email = user.Email;
            string password = user.Password;
            string role = user.Role;

            string message = user.login_user(email, role, password);

            if (message == "found")
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                Console.WriteLine(message);
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public IActionResult Claim_Sub(IFormFile file, Claim insert)
        {
            string module_name = insert.User_Name;
            string hour_work = insert.Hours_worked;
            string hour_rate = insert.Hour_Rate;
            string description = insert.Description;
            string filename = "no file";

            if (file != null && file.Length > 0)
            {
                filename = Path.GetFileName(file.FileName);
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, filename);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    Console.WriteLine("File " + filename + " is successfully uploaded");
                }
            }

            string message = insert.insert_claim(module_name, hour_work, hour_rate, description, filename);

            if (message == "done")
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }
        }

        // Open Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // View Claims Method
        public IActionResult View_Claims()
        {
            Get_Claims collect = new Get_Claims();

            // Here we assume that Get_Claims constructor loads data
            if (collect.Email.Count > 0)
            {
                return View(collect);
            }
            else
            {
                Console.WriteLine("No claims found");
                return View("NoClaims"); // View to show no claims available
            }
        }

        // Approve Claims Method
        public IActionResult Approve()
        {
            Get_Claims collect = new Get_Claims();

            // Fetch pending claims
            var pendingClaims = collect.GetPendingClaims();

            return View(pendingClaims);
        }

        // Approve Claim Method
        [HttpPost]
        public IActionResult ApproveClaim(string id, string action)
        {
            if (action == "approve")
            {
                // Logic to update the claim status in the database to "approved"
                UpdateClaimStatus(id, "approved");
                TempData["Message"] = "Claim approved successfully.";
            }
            else if (action == "decline")
            {
                // Logic to update the claim status in the database to "declined"
                UpdateClaimStatus(id, "declined");
                TempData["Message"] = "Claim declined successfully.";
            }

            return RedirectToAction("Approve");
        }

        private void UpdateClaimStatus(string claimId, string status)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(new Connection().connecting()))
                {
                    connect.Open();
                    string query = "UPDATE claiming SET status = @status WHERE user_id = @id;";

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
                // Handle exceptions as needed
                Console.WriteLine("Error updating claim status: " + ex.Message);
            }
        }
    }
}

