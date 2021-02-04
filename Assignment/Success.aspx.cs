using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment
{
    public partial class Success : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                } else
                {
                    LoadAccount();
                }
            } else
            {
                Response.Redirect("Login.aspx", false);
            }
        }
        protected void LoadAccount()
        {
            string email = Session["Email"].ToString();
            DataSet dset = new DataSet();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AppSecDB"].ToString());
            using (conn)
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand("Select * FROM Account WHERE Email = @email", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@email", email);
                adapter.SelectCommand = cmd;
                adapter.Fill(dset);
                gvAccInfo.DataSource = dset;
                gvAccInfo.DataBind();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Fname"] != null)
                        {
                            if (reader["Fname"] != DBNull.Value)
                            {
                                lbl_fname.Text = HttpUtility.HtmlEncode(reader["Fname"].ToString());
                            }
                        }
                    }

                }
            }
        }

        protected void Btn_Logout_Click(object sender, EventArgs e)
        {
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

        protected void Btn_Change_Password(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword.aspx", false);
        }
    }
}