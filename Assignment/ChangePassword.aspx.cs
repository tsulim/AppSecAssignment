using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Assignment
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppSecDB"].ConnectionString;
        static string finalHash;
        static string salt;
        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_chk.Text = "";
            if (Session["Email"] != null)
            {
                var email = Session["Email"].ToString();
                lbl_email.Text = email;
                var pwdAge = CheckPwdAge(email);
                var currentTime = DateTime.Now;
                TimeSpan ageDiff = currentTime - pwdAge;
                if (ageDiff.TotalMinutes > 15)
                {
                    lbl_lockdown.Text = "Your password age is 15mins. Please kindly change your password.";
                    lbl_lockdown.ForeColor = Color.Red;
                } else
                {
                    if (Session["AuthToken"] == null && Request.Cookies["AuthToken"] == null)
                    {
                        lbl_lockdown.Text = "Your account is no longer under lockdown. You must change your password before you can access your account!";
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string email = lbl_email.Text.ToString();

            var pwdAge = CheckPwdAge(email);
            var currentTime = DateTime.Now;
            TimeSpan ageDiff = currentTime - pwdAge;

            if (ageDiff.TotalMinutes < 1)
            {
                lbl_chk.Text = "You cannot change password within 5 minutes from the last change of password";
                lbl_chk.ForeColor = Color.Red;
            }
            else
            {
                if (Regex.IsMatch(pwd, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
                {
                    var valid = true;
                    if (pwd != tb_cfmpwd.Text || tb_cfmpwd.Text == "")
                    {
                        lbl_chk.Text = "Those passwords didn't match. Please try again!";
                        lbl_chk.ForeColor = Color.Red;
                        valid = false;
                    }
                    if (valid)
                    {
                        SHA512Managed hashing = new SHA512Managed();
                        // Get password old hash and salt
                        string dbHash = getDBHash(email);
                        string dbSalt = getDBSalt(email);

                        string pwdWithOldSalt = pwd + dbSalt;
                        byte[] hashWithOldSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithOldSalt));
                        string oldFinalHash = Convert.ToBase64String(hashWithOldSalt);

                        var resultPwdHistory = checkOldPasswords(email);
                        // Check if it's previous password
                        if (oldFinalHash.Equals(dbHash))
                        {
                            lbl_chk.Text = "This is your current password.";
                            lbl_chk.ForeColor = Color.Red;
                        } else if (resultPwdHistory == false)
                        {
                            lbl_chk.Text = "You cannot reuse passwords from a maximum of 2 changes";
                            lbl_chk.ForeColor = Color.Red;
                        }
                        else
                        {
                            ResetFail(email);

                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];

                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);

                            string pwdWithSalt = pwd + salt;
                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                            finalHash = Convert.ToBase64String(hashWithSalt);

                            var updateResult = UpdatePassword(email, currentTime);

                            if (updateResult == 1)
                            {
                                Response.Redirect("Login.aspx");
                            }
                        }
                    }
                }
            }
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

        protected bool checkOldPasswords(string email)
        {
            var userId = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string userIdSql = "Select Id FROM Account WHERE Email=@email";
            SqlCommand command = new SqlCommand(userIdSql, connection);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Id"] != null)
                        {
                            if (reader["Id"] != DBNull.Value)
                            {
                                userId = Convert.ToInt32(reader["Id"]);
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

            connection.Open();
            DataSet dset = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql = "SELECT * FROM Password WHERE UserId=@userId";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.CommandType = CommandType.Text;
            adapter.SelectCommand = cmd;
            adapter.Fill(dset);

            string pwd = tb_pwd.Text.ToString().Trim();

            var resultTable = dset.Tables[0].Rows;
            int rec_cnt = resultTable.Count;
            for (int i = rec_cnt-2; i < rec_cnt; i++)
            {
                if (i >=0)
                {
                    SHA512Managed hashing = new SHA512Managed();
                    var pwdHash = resultTable[i]["PasswordHash"];
                    var pwdSalt = resultTable[i]["PasswordSalt"];

                    string pwdWithOldSalt = pwd + pwdSalt;
                    byte[] hashWithOldSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithOldSalt));
                    string oldFinalHash = Convert.ToBase64String(hashWithOldSalt);

                    if (oldFinalHash.Equals(pwdHash))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected DateTime CheckPwdAge(string email)
        {
            DateTime lastTiming = DateTime.MinValue;
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

        protected int UpdatePassword(string email, DateTime timeChange)
        {
            int result;

            SqlConnection conn = new SqlConnection(MYDBConnectionString);

            var userId = 0;
            var oldPwdHash = "";
            var oldPwdSalt = "";
            string userIdSql = "Select Id, PasswordHash, PasswordSalt FROM Account WHERE Email=@email";
            SqlCommand command = new SqlCommand(userIdSql, conn);
            command.Parameters.AddWithValue("@email", email);
            try
            {
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Id"] != null && reader["PasswordHash"] != null && reader["PasswordSalt"] != null)
                        {
                            if (reader["Id"] != DBNull.Value && reader["PasswordHash"] != DBNull.Value && reader["PasswordSalt"] != DBNull.Value)
                            {
                                userId = Convert.ToInt32(reader["Id"]);
                                oldPwdHash = reader["PasswordHash"].ToString();
                                oldPwdSalt = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                conn.Close();
            }

            string sqlstmt = "UPDATE Account SET PasswordHash = @pwdHash, PasswordSalt = @pwdSalt, LastPwdChange = @lastChange WHERE Email = @email";
            SqlCommand sqlCmd = new SqlCommand(sqlstmt, conn);

            sqlCmd.Parameters.AddWithValue("@pwdHash", finalHash);
            sqlCmd.Parameters.AddWithValue("@pwdSalt", salt);
            sqlCmd.Parameters.AddWithValue("@lastChange", timeChange);
            sqlCmd.Parameters.AddWithValue("@email", email);

            try
            { 
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Password VALUES(@PasswordHash,@PasswordSalt,@UserId)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@PasswordHash", oldPwdHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", oldPwdSalt);
                            cmd.Parameters.AddWithValue("@UserId", userId);

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

            conn.Open();
            result = sqlCmd.ExecuteNonQuery();
            conn.Close();

            return result;
        }
    }
}