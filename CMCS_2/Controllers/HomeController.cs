using CMCS_2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;

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
            //1st check the connection
            try
            {
                Connection con = new Connection();

                //then check 
                using (SqlConnection connect = new SqlConnection(con.connecting()))
                {
                    //open the connection
                    connect.Open();
                    Console.WriteLine("connected");
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
            //collect user data
            string name = add_user.Username;
            string email = add_user.Email;
            string password = add_user.Password;
            string role = add_user.Role;

            /*
            //check if all are collected
            Console.WriteLine("Name: " + name + "\nEmail: " + email + "\nRole: " + role);
            */

            //pass all the values to insert method
            string message = add_user.insert_user(name, email, password, role);

            //the check if the user inserted
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

            // You can retrieve the list of claims from your database or model here.
            // collect.LoadClaims(); (example function to load claims)

            return View(collect);
        }

        // Approve Claim Method
        public IActionResult Approve()
        {
            // Add logic to fetch the list of pending claims for approval, if needed.
            // You could also handle the logic for approving a claim here.

            return View();
        }
    }
}