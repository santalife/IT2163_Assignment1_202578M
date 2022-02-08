using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IT2163_Assignment1_202578M
{
    public partial class ForgetPassword2FA : System.Web.UI.Page
    {

        string emailaddress = ConfigurationManager.AppSettings["Email"];
        string emailpassword = ConfigurationManager.AppSettings["EmailPassword"];
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        static string verifycode
        {
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

        protected void VerifyLogin(object sender, EventArgs e)
        {
            if (codeTB.Text == verifycode)
            {
                Session.Remove("ToAuthenticate");
                Session["ToChangePassword"] = email;
                Response.Redirect("ForgetPassword", false);
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
    }
}