using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IT2163_Assignment1_202578M
{
    public partial class CheckEmail : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void checkEmail(object sender, EventArgs e)
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
    }
}