using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Assignment
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppSecDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string email = tb_email.Text.ToString().Trim();

            if (!Regex.IsMatch(email, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
            {
                lbl_chk.Text = "Invalid email input!";
                lbl_chk.ForeColor = Color.Red;
            } else
            {
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);

                int loginAttempt = CheckFailAttempts(email);
                try
                {
                    if (loginAttempt == 3)
                    {
                        DateTime lastLogin = CheckLastLogin(email);
                        DateTime currentTime = DateTime.Now;
                        TimeSpan ts = currentTime - lastLogin;
                        if (ts.TotalMinutes >= 5)
                        {
                            //ResetFail(email);
                            // Redirect to change password
                            Session["Email"] = email;
                            Response.Redirect("ChangePassword.aspx", false);
                        } else
                        {
                            lbl_chk.Text = "Email or Password is not valid.";
                            lbl_lockout.Text = "Your account is currently locked. Please ask for assistance to reopen your account.";
                            lbl_chk.ForeColor = Color.Red;
                            lbl_lockout.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        DateTime currentTime = DateTime.Now;
                        UpdateLastLoginAttempt(email, currentTime);
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (userHash.Equals(dbHash))
                            {
                                ResetFail(email);

                                Session["Email"] = email;

                                var pwdAge = CheckPwdAge(email);
                                var current = DateTime.Now;
                                TimeSpan ageDiff = current - pwdAge;
                                if (ageDiff.TotalMinutes > 15)
                                {
                                    Response.Redirect("ChangePassword.aspx", false);
                                } else
                                {
                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;

                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                    Response.Redirect("Success.aspx", false);
                                }
                            }
                            else
                            {
                                loginAttempt += 1;
                                int result = IncrementFail(email, loginAttempt);
                                if (loginAttempt == 2)
                                {
                                    lbl_chk.Text = "Email or Password is not valid.\nYou are at your last attempt before your account is locked";
                                    lbl_chk.ForeColor = Color.Red;
                                }
                                else
                                {
                                    lbl_chk.Text = "Email or Password is not valid. Please try again.";
                                    lbl_chk.ForeColor = Color.Red;
                                }
                            }
                        }
                        else
                        {
                            lbl_chk.Text = "Email or Password is not valid. Please try again.";
                            lbl_chk.ForeColor = Color.Red;
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
        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordHash FROM Account WHERE Email=@email";
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
            string sql = "SELECT PasswordSalt FROM Account WHERE Email=@email";
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
        protected int CheckFailAttempts(string email)
        {
            int attempts = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT LoginFail FROM Account WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LoginFail"] != null)
                        {
                            if (reader["LoginFail"] != DBNull.Value)
                            {
                                attempts = Convert.ToInt32(reader["LoginFail"].ToString());
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
            return attempts;
        }
        protected int IncrementFail(string email, int attempt)
        {
            int result;

            SqlConnection conn = new SqlConnection(MYDBConnectionString);

            string sqlstmt = "UPDATE Account SET LoginFail = @attempt WHERE Email = @email";
            SqlCommand sqlCmd = new SqlCommand(sqlstmt, conn);

            sqlCmd.Parameters.AddWithValue("@attempt", attempt);
            sqlCmd.Parameters.AddWithValue("@email", email);

            conn.Open();
            result = sqlCmd.ExecuteNonQuery();
            conn.Close();

            return result;
        }
        protected void ResetFail(string email)
        {
            //int result;

            SqlConnection conn = new SqlConnection(MYDBConnectionString);

            string sqlstmt = "UPDATE Account SET LoginFail = 0 WHERE Email = @email";
            SqlCommand sqlCmd = new SqlCommand(sqlstmt, conn);

            sqlCmd.Parameters.AddWithValue("@email", email);

            conn.Open();
            //result = 
            sqlCmd.ExecuteNonQuery();
            conn.Close();

            //return result;
        }
        protected int UpdateLastLoginAttempt(string email, DateTime attempt)
        {
            int result;

            SqlConnection conn = new SqlConnection(MYDBConnectionString);

            string sqlstmt = "UPDATE Account SET LastAttempt = @attempt WHERE Email = @email";
            SqlCommand sqlCmd = new SqlCommand(sqlstmt, conn);

            sqlCmd.Parameters.AddWithValue("@attempt", attempt);
            sqlCmd.Parameters.AddWithValue("@email", email);

            conn.Open();
            result = sqlCmd.ExecuteNonQuery();
            conn.Close();

            return result;
        }
        protected DateTime CheckLastLogin(string email)
        {
            DateTime attempts = DateTime.Now;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT LastAttempt FROM Account WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LastAttempt"] != null)
                        {
                            if (reader["LastAttempt"] != DBNull.Value)
                            {
                                attempts = Convert.ToDateTime(reader["LastAttempt"]);
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
            return attempts;
        }

        protected DateTime CheckPwdAge(string email)
        {
            DateTime lastTiming = DateTime.Now;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT LastPwdChange FROM Account WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LastPwdChange"] != null)
                        {
                            if (reader["LastPwdChange"] != DBNull.Value)
                            {
                                lastTiming = Convert.ToDateTime(reader["LastPwdChange"]);
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
            return lastTiming;
        }
    }
}