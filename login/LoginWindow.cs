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
        ///localDbFile
        ///SqlConnection sqlcon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Databases\LoginDB\LoginDB.mdf;Integrated Security=True;Connect Timeout=30");
        
        ///Mssql database
        SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");

        public LOGIN()
        {
            InitializeComponent();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (txtPassword.Text != "")
            {
                txtMessage.Visible = false;
                txtMessage.Enabled = false;
            }
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
                txtMessage.Visible = false;
                txtMessage.Enabled = false;

                // be aware that the txtUsername in the bracket is used to handover the login name to the MainApplication
                MainApplication objCM_Main = new MainApplication(txtUsername.Text);
                this.Hide();
                objCM_Main.Show();
            }
            else
            {
                /// MessageBox.Show("Check your username and password");
                txtMessage.ForeColor = Color.Red;
                txtMessage.TextAlign = HorizontalAlignment.Center;
                txtMessage.Text = "Check your username and password";
                txtMessage.Enabled = true;
                txtMessage.Visible = true;
                                
                /// clear password field
                txtPassword.Text = "";
            }

            sqlcon.Close();
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            /// hide txtMessage
            txtMessage.Visible = false;
            txtMessage.Enabled = false;
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
    }
}
