using System;
using System.Collections;
using Microsoft.Data.SqlClient; 

namespace CMCS_2.Models
{
    public class Get_Claims
    {
        public ArrayList Email { get; set; } = new ArrayList();
        public ArrayList Module { get; set; } = new ArrayList();
        public ArrayList Id { get; set; } = new ArrayList();
        public ArrayList Hours { get; set; } = new ArrayList();
        public ArrayList Rate { get; set; } = new ArrayList();
        public ArrayList Note { get; set; } = new ArrayList();
        public ArrayList Total { get; set; } = new ArrayList();
        public ArrayList Status { get; set; } = new ArrayList();
        public ArrayList Filename { get; set; } = new ArrayList();

        Connection connect = new Connection();

        public Get_Claims()
        {
            string emails = gets_email();

            try
            {
                using (SqlConnection connects = new SqlConnection(connect.connecting()))
                {
                    // Open connection
                    connects.Open();

                    using (SqlCommand prepare = new SqlCommand("SELECT * FROM claiming WHERE email = @Email", connects))
                    {
                        prepare.Parameters.AddWithValue("@Email", emails);  // Parameterized query to avoid SQL injection

                        using (SqlDataReader getEmail = prepare.ExecuteReader())
                        {
                            // Iterate through each row of the result set
                            while (getEmail.Read())
                            {
                                // Add the data to the respective ArrayLists
                                Email.Add(getEmail["email"].ToString());
                                Module.Add(getEmail["module"].ToString());
                                Id.Add(getEmail["user_id"].ToString());
                                Hours.Add(getEmail["hours"].ToString());
                                Rate.Add(getEmail["rate"].ToString());
                                Note.Add(getEmail["note"].ToString());
                                Total.Add(getEmail["total"].ToString());
                                Status.Add(getEmail["status"].ToString());
                                Filename.Add(getEmail["file_name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (IOException error)
            {
                Console.WriteLine(error.Message);
            }
        }

        // Method to retrieve email from the 'active' table
        public string gets_email()
        {
            string hold_email = "";

            try
            {
                using (SqlConnection connects = new SqlConnection(connect.connecting()))
                {
                    // Open connection
                    connects.Open();

                    using (SqlCommand prepare = new SqlCommand("SELECT * FROM active", connects))
                    {
                        using (SqlDataReader getEmail = prepare.ExecuteReader())
                        {
                            // Get the email from the 'active' table
                            if (getEmail.Read())
                            {
                                hold_email = getEmail["email"].ToString();
                            }
                        }
                    }
                }
            }
            catch (IOException error)
            {
                Console.WriteLine(error.Message);
                hold_email = error.Message;
            }

            return hold_email;


        }
        public List<ClaimModel> GetPendingClaims()
        {
            List<ClaimModel> claims = new List<ClaimModel>();

            try
            {
                using (SqlConnection connects = new SqlConnection(connect.connecting()))
                {
                    connects.Open();
                    using (SqlCommand prepare = new SqlCommand("SELECT * FROM claiming WHERE status = 'pending';", connects))
                    {
                        using (SqlDataReader reader = prepare.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                claims.Add(new ClaimModel
                                {
                                    Email = reader["email"].ToString(),
                                    Module = reader["module"].ToString(),
                                    Id = reader["user_id"].ToString(),
                                    Hours = reader["hours"].ToString(),
                                    Rate = reader["rate"].ToString(),
                                    Note = reader["note"].ToString(),
                                    Total = reader["total"].ToString(),
                                    Status = reader["status"].ToString(),
                                    Filename = reader["file_name"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (IOException error)
            {
                Console.WriteLine(error.Message);
            }

            return claims;
        }

    }
}
