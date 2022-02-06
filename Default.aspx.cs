using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace IT2163_Assignment1_202578M
{
    public partial class _Default : Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            var emaillist = RetrieveAllLockedAccount();
            foreach ( var email in emaillist)
            {
                var pwd = Membership.GeneratePassword(12, 1);
                UpdatePassword(pwd, email);                              
            }
            UpdateAllLockedAccount();

        }
           
        protected void UpdatePassword( string pwd, string email )
        {
            string finalHash;
            string salt;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];

            //Fills array of bytes with a cryptographically strong sequence of random values.
            rng.GetBytes(saltByte);
            salt = Convert.ToBase64String(saltByte);

            SHA512Managed hashing = new SHA512Managed();

            string pwdWithSalt = pwd + salt;
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

            finalHash = Convert.ToBase64String(hashWithSalt);


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

                            SendEmail(email, "Your new passowrd for your account recovery is " + pwd + "\n You are recommended to change your password after using this to login.", "Recovery Password for SITConnect");

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void UpdateAllLockedAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LockoutEnabled = 0, AccessFailedCount = 0, LockoutDisabled = @disable WHERE LockoutDisabled <= GETDATE()"))
                    //using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LockoutEnabled = 0, AccessFailedCount = 0, LockoutDisabled = @disable WHERE Email = @now"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@disable", DBNull.Value);
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

        protected List<string> RetrieveAllLockedAccount()
        {
            var emaillist = new List<string>();
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT Email FROM Account WHERE LockoutDisabled <= GETDATE()";
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["Email"] != null)
                        {
                            if (reader["Email"] != DBNull.Value)
                            {
                                emaillist.Add(reader["Email"].ToString());
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
            return emaillist;
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