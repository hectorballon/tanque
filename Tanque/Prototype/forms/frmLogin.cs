using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototype.forms
{
    public partial class frmLogin : Form
    {
        #region "Properties"

        /// <summary>
        /// Key for the crypto provider
        /// </summary>
        private static readonly byte[] _key = { 0xA1, 0xF1, 0xA6, 0xBB, 0xA2, 0x5A, 0x37, 0x6F, 0x81, 0x2E, 0x17, 0x41, 0x72, 0x2C, 0x43, 0x27 };
        /// <summary>
        /// Initialization vector for the crypto provider
        /// </summary>
        private static readonly byte[] _initVector = { 0xE1, 0xF1, 0xA6, 0xBB, 0xA9, 0x5B, 0x31, 0x2F, 0x81, 0x2E, 0x17, 0x4C, 0xA2, 0x81, 0x53, 0x61 };

        public bool Authenticated { get; set; }

        #endregion

        public frmLogin()
        {
            InitializeComponent();
        }

        // exit the program if they do not want to login
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // check that username and password are not empty
        // lookup username in database and compare passwords - if ok continue
        // if not ok, retry 2 more times then exit
        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void Login()
        {
            Authenticated = false;

            if (string.IsNullOrEmpty(tbUsername.Text))
            {
                //Focus box before showing a message
                tbUsername.Focus();
                MessageBox.Show("Enter your username", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Focus again afterwards, sometimes people double click message boxes and select another control accidentally
                tbUsername.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(tbPassword.Text))
            {
                tbPassword.Focus();
                MessageBox.Show("Enter your password", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbPassword.Focus();
                return;
            }

            if (UserAuthenticated(tbUsername.Text, tbPassword.Text))
            {
                Authenticated = true;
                this.Close(); // close this form - do not exit the application
            }
            else
            {
                MessageBox.Show("Username or Password not recognised");
            }
        }

        // Does the user exist?
        // if so - is the password correct?
        private bool UserAuthenticated(string user, string password)
        {
            try
            {
                int result = (int)usersTableAdapter.GetUserIDByUsernameAndPassword(user, password);
                if (result > 0) return true;
            }
            catch (Exception) // FIXED: Added Exception catching  which defaults to Not Authenticated
            {
                return false;
            }
            return false;
        }

#if (DEBUG)
        //Only compile this method for local debugging.
        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        private static string Decrypt(string Value)
        {
            SymmetricAlgorithm mCSP;
            ICryptoTransform ct = null;
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] byt;
            byte[] _result;

            mCSP = new RijndaelManaged();

            try
            {
                mCSP.Key = _key;
                mCSP.IV = _initVector;
                ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);


                byt = Convert.FromBase64String(Value);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();

                cs.Close();
                _result = ms.ToArray();
            }
            catch
            {
                _result = null;
            }
            finally
            {
                if (ct != null)
                    ct.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (cs != null)
                    cs.Dispose();
            }

            return ASCIIEncoding.UTF8.GetString(_result);
        }
#endif

        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        private static string Encrypt(string Password)
        {
            if (string.IsNullOrEmpty(Password))
                return string.Empty;

            byte[] Value = Encoding.UTF8.GetBytes(Password);
            SymmetricAlgorithm mCSP = new RijndaelManaged();
            mCSP.Key = _key;
            mCSP.IV = _initVector;
            using (ICryptoTransform ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                    {
                        cs.Write(Value, 0, Value.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
    }
}
