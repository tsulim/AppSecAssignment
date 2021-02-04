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
    public partial class RegistrationForm : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppSecDB"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string email = tb_email.Text.ToString().Trim();
            
            bool emailValid = checkValidEmail();
            if (emailValid) {
                if (Regex.IsMatch(pwd, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
                {
                    var valid = true;
                    if (!Regex.IsMatch(email, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
                    {
                        lbl_emailchk.Text = "Invalid email input!";
                        valid = false;
                    }

                    if (pwd != tb_cfpwd.Text || tb_cfpwd.Text == "")
                    {
                        lbl_chk.Text = "Those passwords didn't match. Please try again!";
                        lbl_chk.ForeColor = Color.Red;
                        valid = false;
                    }
                    if (valid)
                    {
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] saltByte = new byte[8];

                        rng.GetBytes(saltByte);
                        salt = Convert.ToBase64String(saltByte);

                        SHA512Managed hashing = new SHA512Managed();

                        string pwdWithSalt = pwd + salt;
                        byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                        finalHash = Convert.ToBase64String(hashWithSalt);

                        RijndaelManaged cipher = new RijndaelManaged();
                        cipher.GenerateKey();
                        Key = cipher.Key;
                        IV = cipher.IV;

                        createAccount();
                        Response.Redirect("Login.aspx");
                    }
                }
            } else
            {
                lbl_chk.Text = "This email already has an account";
                lbl_chk.ForeColor = Color.Red;
            }
        }
        public bool checkValidEmail()
        {
            bool valid = true;
            DataSet dset = new DataSet();
            SqlConnection conn = new SqlConnection(MYDBConnectionString);
            
            using (conn)
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();
                string sqlQuery = string.Format("SELECT * FROM Account WHERE Email = @email");
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@email", tb_email.Text.Trim());
                adapter.SelectCommand = cmd;
                adapter.Fill(dset);

                int rec_cnt = dset.Tables[0].Rows.Count;

                if (rec_cnt == 1)
                {
                    valid = false;
                }
            }
            return valid;
        }
        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Fname,@Lname,@CardNo,@CardName,@ExpDate,@Cvv,@Email,@Dob,@PasswordHash,@PasswordSalt,@IV,@Key, 0, null, null)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Fname", tb_fname.Text.Trim());
                            cmd.Parameters.AddWithValue("@Lname", tb_lname.Text.Trim());
                            cmd.Parameters.AddWithValue("@CardNo", Convert.ToBase64String(encryptData(tb_cardno.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CardName", Convert.ToBase64String(encryptData(tb_cardname.Text.Trim())));
                            cmd.Parameters.AddWithValue("@ExpDate", Convert.ToBase64String(encryptData(tb_expDate.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Cvv", Convert.ToBase64String(encryptData(tb_cvv.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@Dob", tb_dob.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));

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
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            } finally { }
            return cipherText;
        }
    }
}