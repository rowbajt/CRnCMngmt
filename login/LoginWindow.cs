using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace login
{
    public partial class LOGIN : Form
    {
        ///Mssql database
        SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");

        public LOGIN()
        {
            InitializeComponent();
            
            // set the initial ForeColor of the button
            LoginBtnState();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            sqlcon.Open();

            SqlCommand cmd = sqlcon.CreateCommand();
            cmd.CommandType = CommandType.Text;

            /// define and execute the query
            cmd.CommandText = "SELECT * FROM tbl_Login WHERE userName = '" + txtUsername.Text.Trim() + "' AND password = '" + txtPassword.Text.Trim() + "'";
            cmd.ExecuteNonQuery();

            /// retrieve the data
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dtbl = new DataTable();
            sda.Fill(dtbl);

            /// check if login is successfull
            if (dtbl.Rows.Count == 1)
            {
                txtError.Visible = false;
                txtError.Enabled = false;

                // be aware that the txtUsername in the bracket is used to handover the login name to the MainApplication
                MainApplication objCM_Main = new MainApplication(txtUsername.Text);
                this.Hide();
                objCM_Main.Show();
            }
            else
            {
                /// MessageBox.Show("Check your username and password");
                txtError.ForeColor = Color.Red;
                txtError.TextAlign = HorizontalAlignment.Center;
                txtError.Text = "Check your username and password";
                txtError.Enabled = true;
                txtError.Visible = true;
                                
                /// clear password field
                txtPassword.Text = "";
            }

            sqlcon.Close();
        }

        private void txtError_TextChanged(object sender, EventArgs e)
        {
            /// hide txtMessage
            txtError.Visible = false;
            txtError.Enabled = false;
        }

        private void btnLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
                // these last two lines will stop the beep sound
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
                // these last two lines will stop the beep sound
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void CloserButton_Click(object sender, EventArgs e)
        {
            // The user wants to exit the application. Close everything down.
            Application.Exit();
        }

        private void LOGIN_Load(object sender, EventArgs e)
        {

        }
        private void LoginBtnState()
        {
            if (String.IsNullOrEmpty(txtUsername.Text))
            {
                btnLogin.ForeColor = Color.Red;
            }
            else if (String.IsNullOrEmpty(txtPassword.Text))
            {
                btnLogin.ForeColor = Color.Red;
            } else
            {
                btnLogin.ForeColor = Color.FromArgb(25,152,97);
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            LoginBtnState();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            LoginBtnState();
        }
    }
}
