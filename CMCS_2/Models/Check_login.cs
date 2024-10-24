using Microsoft.Data.SqlClient; 
using System;

namespace CMCS_2.Models
{
    public class Check_login
    {
        // Properties
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }

        // Connection string
        Connection connect = new Connection();

        // Method to check the user
        public string login_user(string emails, string roles, string passwords)
        {
            string message = "";
            try
            {
                // Connect and open
                using (SqlConnection connects = new SqlConnection(connect.connecting()))
                {
                    // Open connection
                    connects.Open();

                    // Query to retrieve user data
                    string query = "SELECT * FROM users WHERE email='" + emails + "' AND password='" + passwords + "';";

                    // Alternatively, you can use parameterized query to prevent SQL injection:
                    // string query = "SELECT * FROM users WHERE email = @Email AND password = @Password";
                    // SqlCommand prepare = new SqlCommand(query, connects);
                    // prepare.Parameters.AddWithValue("@Email", emails);
                    // prepare.Parameters.AddWithValue("@Password", passwords);

                    // Prepare to execute
                    using (SqlCommand prepare = new SqlCommand(query, connects))
                    {
                        // Read the data
                        using (SqlDataReader find_user = prepare.ExecuteReader())
                        {
                            // Check if the user is found
                            if (find_user.HasRows)
                            {
                                message = "found";
                            }
                            else
                            {
                                message = "not";
                            }
                        }
                    }

                    // Close the connection
                    connects.Close();

                    if (message == "found")
                    {
                        update_active(emails); // Pass the correct email
                    }
                }
            }
            catch (IOException error_db)
            {
                message = error_db.Message;
            }

            return message;
        }

        // Update active method
        public void update_active(string email)
        {
            try
            {
                using (SqlConnection connects = new SqlConnection(connect.connecting()))
                {
                    connects.Open();

                    string query = "UPDATE active SET email='" + email + "'";

                    using (SqlCommand command = new SqlCommand(query, connects))
                    {
                        command.ExecuteNonQuery();
                    }

                    connects.Close();
                }
            }
            catch (IOException error)
            {
                Console.WriteLine("Error: " + error.Message);
            }
        }
    }
}
