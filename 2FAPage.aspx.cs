using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace IT2163_Assignment1_202578M
{
    public partial class _2FAPage : System.Web.UI.Page
    {
        string emailaddress = ConfigurationManager.AppSettings["Email"];
        string emailpassword = ConfigurationManager.AppSettings["EmailPasswrod"];
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        static string verifycode{
            get;
            set;

        }


        static string email
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Session["ToAuthenticate"] == null)
                {
                    Response.Redirect("Login.aspx", false);

                }
                else
                {

                    Random generator = new Random();
                    verifycode = generator.Next(0, 1000000).ToString("D6");
                    email = Session["ToAuthenticate"].ToString();
                    Session.Remove("ToAuthenticate");
                    SendEmail(email, "This is your verification code for you to login: " + verifycode, "SITConnect Two Factor Authentication");
                    

                }
            }

        }

        protected void VerifyLogin (object sender, EventArgs e)
        {
            if (codeTB.Text == verifycode)
            {
                if (CheckPasswordAge(RetrieveLastPasswordChange(email)) >= 2)
                {
                    Session.Remove("ToAuthenticate");
                    verifycode = null;

                    Session["ChangePassword"] = email;
                    Response.Redirect("ChangePassword.aspx", false);
                }
                else
                {
                    Session.Remove("ToAuthenticate");
                    verifycode = null;

                    Session["LoggedIn"] = email;

                    string randomguid = Guid.NewGuid().ToString();
                    Session["AuthToken"] = randomguid;

                    Response.Cookies.Add(new HttpCookie("AuthToken", randomguid));


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

                    Response.Redirect("Profile.aspx", false);
                }

            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Visible = true;
                lblMessage.Text = "The code you entered is not valid, please try again.";
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