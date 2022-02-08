using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IT2163_Assignment1_202578M
{
    public partial class SiteMaster : MasterPage
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                }
                else
                {
                    login.Visible = false;
                    register.Visible = false;
                    username.Visible = true;
                    username.Text = Session["LoggedIn"].ToString();
                    logout.Visible = true;
                }

            }
            else
            {
            }
        }


        public Label username
        {
            get
            {
                return Username;
            }
        }

        public Label logout
        {
            get
            {
                return this.Logout;
            }
        }
        public Label login
        {
            get
            {
                return this.Login;
            }
        }

        public Label register
        {
            get
            {
                return this.Register;
            }
        }

        protected void LogoutMe(object sender, EventArgs e)
        {
            string email = Session["LoggedIn"].ToString();
            createAuditLog(email, "Logout");

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);

            }
        }

        protected void createAuditLog(string email, string action)
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
                            cmd.Parameters.AddWithValue("@Action", action);
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
    }
}