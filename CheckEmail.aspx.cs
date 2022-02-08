using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IT2163_Assignment1_202578M
{
    public partial class CheckEmail : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }



        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void checkEmail(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (ValidateCaptcha())
                {
                    if (CheckPasswordAge(RetrieveLastPasswordChange(EmailTB.Text)) < 1)
                    {
                        ErrorMessage.Visible = true;
                        ErrorMessage.ForeColor = System.Drawing.Color.Red;
                        ErrorMessage.Text = "Please try again later";
                    }
                    else
                    {
                        if (checkEmailExist() == true)
                        {
                            Session["ToAuthenticate"] = EmailTB.Text;
                            Response.Redirect("ForgetPassword2FA", false);
                        }
                        else
                        {
                            ErrorMessage.Visible = true;
                            ErrorMessage.ForeColor = System.Drawing.Color.Red;
                            ErrorMessage.Text = "The Email you entered does not exist";
                        }
                    }


                }
                else
                {
                    ErrorMessage.Visible = true;
                    ErrorMessage.ForeColor = System.Drawing.Color.Red;
                    ErrorMessage.Text = "Cacptcha Failed";
                }
            }
            else
            {
                ErrorMessage.Visible = true;
                ErrorMessage.ForeColor = System.Drawing.Color.Red;
                ErrorMessage.Text = "Check your fields again";
            }            

        }

        protected double CheckPasswordAge(string date)
        {
            DateTime lastpasswordchange = Convert.ToDateTime(date);
            DateTime now = DateTime.Now;
            TimeSpan ts = now - lastpasswordchange;
            return ts.TotalMinutes;
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


        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LdF7lkeAAAAAEgNn8cqcJAc5XuryxUCljv9qrjt&response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();


                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}