using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IT2163_Assignment1_202578M
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void OnSubmitForm(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                
                string pwd = passwordTB.Text.ToString().Trim(); ;

                //Generate random "salt" 
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                //Fills array of bytes with a cryptographically strong sequence of random values.
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

                if (checkEmailExist())
                {
                    createAccount();
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    emailError.Visible = true;
                    emailError.Text = "Email already exists!";
                }

            }
            else
            {
                ExtraLabel.Text = "Please check your fields!";
            }

        }

        protected bool checkEmailExist()
        {
            DataSet dset = new DataSet();
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

                            if (userCount > 0)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void createAccount()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Fname, @Lname, @CreditCard, @Email, @Dob, @Photo, @PasswordHash, @PaswwordSalt, @IV, @Key, @Count, @Lock, @Time, @Lockdisabled, @2FA)"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {

                            int length = FileUpload1.PostedFile.ContentLength;
                            byte[] pic = new byte[length];

                            FileUpload1.PostedFile.InputStream.Read(pic, 0, length);

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Fname", HttpUtility.HtmlEncode(FirstNameTB.Text));
                            cmd.Parameters.AddWithValue("@Lname", HttpUtility.HtmlEncode(LastNameTB.Text));
                            cmd.Parameters.AddWithValue("@CreditCard", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(CreditCardTB.Text))));
                            cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(EmailTB.Text));
                            cmd.Parameters.AddWithValue("@Dob", HttpUtility.HtmlEncode(DobTB.Text));
                            cmd.Parameters.AddWithValue("@Photo", pic);
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PaswwordSalt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@Count", 0);
                            cmd.Parameters.AddWithValue("@Lock", 0);
                            cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Lockdisabled", DBNull.Value);
                            cmd.Parameters.AddWithValue("@2FA", 0);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                }
                InsertPassword1();


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void InsertPassword1()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO AccountHistory VALUES(@email, @PassowrdHash1, @PasswordSalt1, @PasswordHash2, @PasswordSalt2)"))
                    //using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @Mobile,@Nric,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@MobileVerified,@EmailVerified)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(EmailTB.Text));
                            cmd.Parameters.AddWithValue("@PassowrdHash1", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt1", salt);
                            cmd.Parameters.AddWithValue("@PasswordHash2", "");
                            cmd.Parameters.AddWithValue("@PasswordSalt2", "");
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
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);


                //Encrypt
                //cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
                //cipherString = Convert.ToBase64String(cipherText);
                //Console.WriteLine("Encrypted Text: " + cipherString);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }
            return cipherText;
        }


        
    }
}