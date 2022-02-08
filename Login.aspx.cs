using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Web.Security;
using System.Configuration;

namespace IT2163_Assignment1_202578M
{
    public partial class Login : System.Web.UI.Page
    {
        string emailaddress = ConfigurationManager.AppSettings["Email"];
        string emailpassword = ConfigurationManager.AppSettings["EmailPasswrod"];
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
           
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Remove("ToAuthenticate");
        }

        protected void LoginMe(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (ValidateCaptcha())
                {
                    if (checkEmailExist())
                    {
                        if (retrieveLockout())
                        {
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            lblMessage.Text = "Please check if you correctly entered Email & Password";

                        }
                        else
                        {

                            string pwd = passwordTB.Text.ToString();
                            string email = EmailTB.Text.ToString();

                            SHA512Managed hashing = new SHA512Managed();

                            string dbHash = getDBHash(email);
                            string dbSalt = getDBSalt(email);

                            try
                            {
                                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                                {
                                    string pwdWithSalt = pwd + dbSalt;
                                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                    string userHash = Convert.ToBase64String(hashWithSalt);
                                    if (userHash.Equals(dbHash))
                                    {
                                        if(Retrieve2FA() == "1")
                                        {
                                            Session["ToAuthenticate"] = email;
                                            Response.Redirect("2FAPage.aspx", false);
                                        }
                                        else
                                        {
                                            if (CheckPasswordAge(RetrieveLastPasswordChange(email)) >= 2)
                                            {
                                                Session["ChangePassword"] = email;
                                                Response.Redirect("ChangePassword.aspx", false);
                                            }
                                            else
                                            {
                                                Session["LoggedIn"] = email;

                                                string guid = Guid.NewGuid().ToString();
                                                Session["AuthToken"] = guid;

                                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));


                                                createAuditLog(email);

                                                Response.Redirect("Profile.aspx", false);
                                            }
                                        }
                                        
                                    }

                                    else
                                    {
                                        int failCount;
                                        try
                                        {
                                            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                                            {
                                                using (SqlCommand cmd = new SqlCommand("SELECT AccessFailedCount FROM ACCOUNT WHERE EMAIL LIKE @email"))
                                                //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                                                {
                                                    using (SqlDataAdapter sda = new SqlDataAdapter())
                                                    {

                                                        cmd.CommandType = CommandType.Text;
                                                        cmd.Parameters.AddWithValue("@email", EmailTB.Text);
                                                        cmd.Connection = con;
                                                        con.Open();
                                                        failCount = (int)cmd.ExecuteScalar();
                                                        con.Close();

                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(ex.ToString());
                                        }
                                        failCount++;
                                        addFailCount(failCount);
                                        if (failCount == 3)
                                        {
                                            enableLockout(1);
                                            SendEmail(email, "Your Account has been lockout for 3 failed attempts, please come back after 1 minute using a password we will provide you to login", "Account Lockout Notice");
                                        }

                                        lblMessage.ForeColor = System.Drawing.Color.Red;
                                        lblMessage.Text = "Please check if you correctly entered Email & Password";

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.ToString());
                            }
                            finally { }


                        }

                    }
                    else
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = "Please check if you correctly entered Email & Password";
                    }
                }
                else
                {
                    //lblMessage.Text = "Are you a human or a bot?";
                }
            }
            else
            {
                lblMessage.Visible = true;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please Check your fields!";
            }

        }


        protected void createAuditLog(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO AuditLog VALUES(@UserId, @DateOfAction, @Action)"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.Parameters.AddWithValue("@UserId", email);
                            cmd.Parameters.AddWithValue("@DateOfAction", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Action", "Login");
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

        protected void addFailCount(int failedCount)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET AccessFailedCount = @Count WHERE EMAIL Like @email"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Count", failedCount);
                            cmd.Parameters.AddWithValue("@email", EmailTB.Text);
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

        protected void enableLockout(int lockout)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LockoutEnabled = @lockout, LockoutDisabled = @time WHERE EMAIL Like @email"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@lockout", lockout);
                            cmd.Parameters.AddWithValue("@time", DateTime.Now.AddMinutes(1));
                            cmd.Parameters.AddWithValue("@email", EmailTB.Text);
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

        protected int retrieveFailCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT AccessFailedCount FROM ACCOUNT WHERE EMAIL LIKE @email"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@email", EmailTB.Text);
                            cmd.Connection = con;
                            con.Open();
                            int failCount = (int)cmd.ExecuteScalar();
                            con.Close();

                            return failCount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected bool retrieveLockout()
        {
            bool lockoutbool = false;
            
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT LockoutEnabled FROM ACCOUNT WHERE EMAIL LIKE @email"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@email", EmailTB.Text);
                            cmd.Connection = con;
                            con.Open();
                            int lockout = (int)cmd.ExecuteScalar();
                            con.Close();
                            if(lockout == 1)
                            {
                                lockoutbool = true;
                            }
                            return lockoutbool;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected string Retrieve2FA()
        {
            string twoFactor = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select TwoFactor FROM Account WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", EmailTB.Text);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["TwoFactor"] != null)
                        {
                            if (reader["TwoFactor"] != DBNull.Value)
                            {
                                twoFactor = reader["TwoFactor"].ToString();

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
            return twoFactor;
        }

        protected bool checkEmailExist()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ACCOUNT WHERE EMAIL LIKE @email"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@email", EmailTB.Text);
                            cmd.Connection = con;
                            con.Open();
                            int userCount = (int)cmd.ExecuteScalar();
                            con.Close();

                            if (userCount == 1)
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected string getDBHash(string email)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
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

        protected string getDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
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

        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LdF7lkeAAAAAEgNn8cqcJAc5XuryxUCljv9qrjt&response=" + captchaResponse);

            try
            {
                using(WebResponse wResponse = req.GetResponse())
                {
                    using(StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        lblMessage.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch(WebException ex)
            {
                throw ex;
            }
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
                smtp.Credentials = new NetworkCredential(emailaddress, emailpassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(emailmessage);
            }
            catch (Exception) { }
        }

        protected string RetrieveLastPasswordChange(string email)
        {
            string date = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LastPasswordChange FROM Account WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["LastPasswordChange"] != null)
                        {
                            if (reader["LastPasswordChange"] != DBNull.Value)
                            {
                                date = reader["LastPasswordChange"].ToString();                                     

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
            return date;
        }

        protected double CheckPasswordAge(string date)
        {
            DateTime lastpasswordchange = Convert.ToDateTime(date);
            DateTime now = DateTime.Now;
            TimeSpan ts = now - lastpasswordchange;            
            return ts.TotalMinutes;
        }
    }

}