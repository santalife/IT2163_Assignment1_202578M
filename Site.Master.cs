using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IT2163_Assignment1_202578M
{
    public partial class SiteMaster : MasterPage
    {
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
    }
}