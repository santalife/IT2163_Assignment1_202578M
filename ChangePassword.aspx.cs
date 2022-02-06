using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;

namespace IT2163_Assignment1_202578M
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public static string email
        {
            get;
            set;
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["ChangePassword"] == null && email == null)
            {
                Response.Redirect("Login.aspx", false);
            }
            else
            {
                if (email == null)
                {
                    email = Session["ChangePassword"].ToString();
                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();

                }

            }
        }

        protected void UpdatePassword(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string finalHash;
                string salt;
                string pwd = passwordTB.Text.ToString().Trim();
                string confirmpwd = ConfirmPassword.Text.ToString().Trim();
                if (pwd == confirmpwd)
                {

                    string oldpasswordhash1 = GetPasswordHash1();
                    string oldpasswordsalt1 = getPasswordSalt1();
                    string oldpasswordhash2 = GetPasswordHash2();
                    string oldpasswordsalt2 = getPasswordSalt2();

                    if (ComparePasswordHash(pwd, oldpasswordhash1, oldpasswordsalt1) || ComparePasswordHash(pwd, oldpasswordhash2, oldpasswordsalt2))
                    {
                        ChangeSuccess.Visible = false;
                        ChangeFail.Visible = true;
                        ChangeFail.Text = "Please change to another password that was not used the pass 2 times";
                    }
                    else
                    {
                        //Generate random "salt" 
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] saltByte = new byte[8];

                        //Fills array of bytes with a cryptographically strong sequence of random values.
                        rng.GetBytes(saltByte);
                        salt = Convert.ToBase64String(saltByte);

                        SHA512Managed hashing = new SHA512Managed();

                        string pwdWithSalt = pwd + salt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                        finalHash = Convert.ToBase64String(hashWithSalt);


                        RijndaelManaged cipher = new RijndaelManaged();
                        cipher.GenerateKey();


                        try
                        {
                            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                            {

                                using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash = @hash, PasswordSalt = @salt WHERE EMAIL Like @email"))
                                //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                                {
                                    using (SqlDataAdapter sda = new SqlDataAdapter())
                                    {
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Parameters.AddWithValue("@hash", finalHash);
                                        cmd.Parameters.AddWithValue("@salt", salt);
                                        cmd.Parameters.AddWithValue("@email", email);
                                        cmd.Connection = con;
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        con.Close();
                                        UpdatePasswordTime();
                                        UpdatePassword2(oldpasswordhash1, oldpasswordsalt1);
                                        UpdatePassword1(finalHash, salt);

                                        SendEmail(email.ToString(), "You have succesfully changed your password", "Password Change for SITConnect");
                                        ChangeSuccess.ForeColor = System.Drawing.Color.Green;
                                        ChangeSuccess.Visible = true;
                                        ChangeFail.Visible = false;

                                        Response.Redirect("Login.aspx", false);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }
                    }

                }
                else
                {
                    PasswordError.ForeColor = System.Drawing.Color.Red;
                    PasswordError.Visible = true;
                    ChangeSuccess.Visible = false;
                    ChangeFail.Visible = false;
                }
            }
            else
            {
                PasswordError.ForeColor = System.Drawing.Color.Red;
                PasswordError.Visible = true;
                ChangeSuccess.Visible = false;
                ChangeFail.Visible = false;
                PasswordError.Text = "Please Check your fields";
            }


        }

        protected void UpdatePasswordTime()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LastPasswordChange = @time WHERE EMAIL Like @email"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected bool ComparePasswordHash(string newpassword, string oldpasswordhash, string salt)
        {
            SHA512Managed hashing = new SHA512Managed();

            string pwdWithSalt = newpassword + salt;
            byte[] newhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            string newHash = Convert.ToBase64String(newhashWithSalt);

            if (newHash.Equals(oldpasswordhash))
            {
                return true;
            }
            return false;
        }


        protected void UpdatePassword1(string passwordhash, string passwordsalt)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("UPDATE AccountHistory SET PasswordHash1 = @hash, PasswordSalt1 = @salt WHERE EMAIL Like @email"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@hash", passwordhash);
                            cmd.Parameters.AddWithValue("@salt", passwordsalt);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void UpdatePassword2(string passwordhash, string passwordsalt)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("UPDATE AccountHistory SET PasswordHash2 = @hash, PasswordSalt2 = @salt WHERE EMAIL Like @email"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@hash", passwordhash);
                            cmd.Parameters.AddWithValue("@salt", passwordsalt);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected string GetPasswordHash1()
        {
            string h = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordHash1 FROM AccountHistory WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash1"] != null)
                        {
                            if (reader["PasswordHash1"] != DBNull.Value)
                            {
                                h = reader["PasswordHash1"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string GetPasswordHash2()
        {
            string h = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordHash2 FROM AccountHistory WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash2"] != null)
                        {
                            if (reader["PasswordHash2"] != DBNull.Value)
                            {
                                h = reader["PasswordHash2"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getPasswordSalt1()
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordSalt1 FROM AccountHistory WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt1"] != null)
                        {
                            if (reader["PasswordSalt1"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt1"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected string getPasswordSalt2()
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordSalt2 FROM AccountHistory WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt2"] != null)
                        {
                            if (reader["PasswordSalt2"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt2"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected void SendEmail(string email, string message, string subject)
        {
            try
            {
                MailMessage emailmessage = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                emailmessage.From = new MailAddress("nyppolyclinic@gmail.com");
                emailmessage.To.Add(new MailAddress(email));
                emailmessage.Subject = subject;
                emailmessage.IsBodyHtml = true;
                emailmessage.Body = message;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("nyppolyclinic@gmail.com", "Ewan_alex123");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(emailmessage);
            }
            catch (Exception) { }
        }

    }
}