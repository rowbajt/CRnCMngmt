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
    public partial class MainApplication : Form
    {
        public MainApplication()
        {
            InitializeComponent();

            // function loads data into the Icao dropdown
            FillIcaoComboBox();
            // funcction loads data into the Environment dropdown
            FillEnvironmentComboBox();
            // function Autocomplete for Icao dropdown
            AutoCompleteText();
            // function Load data into Recent Changes grid
            LoadTable();
        }

        bool isTopPanelDragged = false;
        bool isLeftPanelDragged = false;
        bool isRightPanelDragged = false;
        bool isBottomPanelDragged = false;
        bool isTopBorderPanelDragged = false;
        bool isWindowMaximized = false;
        Point offset;
        Size _normalWindowSize;
        Point _normalWindowLocation = Point.Empty;

        // initialize variable
        int LastLoadedItem = 0;

        private void CloserButton_Click(object sender, EventArgs e)
        {
            // The user wants to exit the application. Close everything down.
            Application.Exit();
        }

        // Top Border Panel
        private void TopBorderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isTopBorderPanelDragged = true;
            }
            else
            {
                isTopBorderPanelDragged = false;
            }
        }
        private void TopBorderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Y < this.Location.Y)
            {
                if (isTopBorderPanelDragged)
                {
                    if (this.Height < 50)
                    {
                        this.Height = 50;
                        isTopBorderPanelDragged = false;
                    }
                    else
                    {
                        this.Location = new Point(this.Location.X, this.Location.Y + e.Y);
                        this.Height = this.Height - e.Y;
                    }
                }
            }
        }
        private void TopBorderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isTopBorderPanelDragged = false;
        }
        // Top Panel
        private void TopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isTopPanelDragged = true;
                Point pointStartPosition = this.PointToScreen(new Point(e.X, e.Y));
                offset = new Point();
                offset.X = this.Location.X - (pointStartPosition.X + panelLeftTop.Size.Width);
                offset.Y = this.Location.Y - pointStartPosition.Y;
            }
            else
            {
                isTopPanelDragged = false;
            }
            if (e.Clicks == 2)
            {
                isTopPanelDragged = false;
                MaxButton_Click(sender, e);
            }
        }
        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isTopPanelDragged)
            {
                Point newPoint = panelTop.PointToScreen(new Point(e.X, e.Y));
                newPoint.Offset(offset);
                this.Location = newPoint;

                if (this.Location.X > 2 || this.Location.Y > 2)
                {
                    if (this.WindowState == FormWindowState.Maximized)
                    {
                        this.Location = _normalWindowLocation;
                        this.Size = _normalWindowSize;
                        toolTip1.SetToolTip(MaxButton, "Maximize");
                        MaxButton.CFormState = MinMaxButton.CustomFormState.Normal;
                        isWindowMaximized = false;
                    }
                }
            }
        }
        private void TopPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isTopPanelDragged = false;
            if (this.Location.Y <= 5)
            {
                if (!isWindowMaximized)
                {
                    _normalWindowSize = this.Size;
                    _normalWindowLocation = this.Location;

                    Rectangle rect = Screen.PrimaryScreen.WorkingArea;
                    this.Location = new Point(0, 0);
                    this.Size = new System.Drawing.Size(rect.Width, rect.Height);

                    toolTip1.SetToolTip(MaxButton, "Restore Down");
                    MaxButton.CFormState = MinMaxButton.CustomFormState.Maximize;
                    isWindowMaximized = true;
                }
            }
        }
        // Bottom Panel
        private void BottomPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isBottomPanelDragged = true;
            }
            else
            {
                isBottomPanelDragged = false;
            }
        }
        private void BottomPanel_MouseMove(object sender, MouseEventArgs e)
        {

            if (isBottomPanelDragged)
            {
                if (this.Height < 50)
                {
                    this.Height = 50;
                    isBottomPanelDragged = false;
                }
                else
                {
                    this.Height = this.Height + e.Y;
                }
            }
        }
        private void BottomPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isBottomPanelDragged = false;
        }
        // Left Panel
        private void LeftPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Location.X <= 0 || e.X < 0)
            {
                isLeftPanelDragged = false;
                this.Location = new Point(10, this.Location.Y);
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    isLeftPanelDragged = true;
                }
                else
                {
                    isLeftPanelDragged = false;
                }
            }
        }
        private void LeftPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < this.Location.X)
            {
                if (isLeftPanelDragged)
                {
                    if (this.Width < 100)
                    {
                        this.Width = 100;
                        isLeftPanelDragged = false;
                    }
                    else
                    {
                        this.Location = new Point(this.Location.X + e.X, this.Location.Y);
                        this.Width = this.Width - e.X;
                    }
                }
            }
        }
        private void LeftPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isLeftPanelDragged = false;
        }
        // Right Panel
        private void RightPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isRightPanelDragged = true;
            }
            else
            {
                isRightPanelDragged = false;
            }
        }
        private void RightPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRightPanelDragged)
            {
                if (this.Width < 100)
                {
                    this.Width = 100;
                    isRightPanelDragged = false;
                }
                else
                {
                    this.Width = this.Width + e.X;
                }
            }
        }
        private void RightPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isRightPanelDragged = false;
        }

        // Minimize Button
        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        // Maximize Button
        private void MaxButton_Click(object sender, EventArgs e)
        {
            if (isWindowMaximized)
            {
                this.Location = _normalWindowLocation;
                this.Size = _normalWindowSize;
                toolTip1.SetToolTip(MaxButton, "Maximize");
                MaxButton.CFormState = MinMaxButton.CustomFormState.Normal;
                isWindowMaximized = false;
            }
            else
            {
                _normalWindowSize = this.Size;
                _normalWindowLocation = this.Location;

                Rectangle rect = Screen.PrimaryScreen.WorkingArea;
                this.Location = new Point(0, 0);
                this.Size = new System.Drawing.Size(rect.Width, rect.Height);
                toolTip1.SetToolTip(MaxButton, "Restore Down");
                MaxButton.CFormState = MinMaxButton.CustomFormState.Maximize;
                isWindowMaximized = true;
            }
        }
        // logout current user from MainApplication and show Login Window
        private void panelLogout_Click(object sender, EventArgs e)
        {
            LOGIN objloginWithDatabases = new LOGIN();
            this.Hide();
            objloginWithDatabases.Show();
        }
        // Code to move windows when clicking on WindowTextLabel
        private void WindowTextLabel_MouseDown(object sender, MouseEventArgs e)
        {
            TopPanel_MouseDown(sender, e);
        }
        private void WindowTextLabel_MouseMove(object sender, MouseEventArgs e)
        {
            TopPanel_MouseMove(sender, e);
        }
        private void WindowTextLabel_MouseUp(object sender, MouseEventArgs e)
        {
            TopPanel_MouseUp(sender, e);
        }

        // This code fills the combobox which holds the [IcaoDesignator] data from the table tbl_Airlines
        public void FillIcaoComboBox()
        {
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "SELECT * FROM tbl_Airlines order by [IcaoDesignator] ASC";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    string sName = myReader.GetString(2); // index 2 represents 3rd column in table - [IcaoDesignator]
                    comboIcao.Items.Add(sName);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }

            sqlcon.Close();
        }

        // This code fills the [Environment] combobox
        public void FillEnvironmentComboBox()
        {
            // clear all data from combobox
            comboEnvironment.Items.Clear();

            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");

            string sql;

            if (comboIcao.SelectedItem != null)
            {
                sql = "SELECT distinct([Environment]) FROM tbl_ChangeTracking WHERE [AirlineId] = '" + comboIcao.Text + "' order by [Environment] ASC";
            }
            else
            {
                sql = "SELECT distinct([Environment]) FROM tbl_ChangeTracking order by [Environment] ASC";
            }

            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                // add initial empty record
                comboEnvironment.Items.Add("");

                // add the records from the database result
                while (myReader.Read())
                {
                    string sName = myReader.GetString(0);
                    comboEnvironment.Items.Add(sName);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }

            sqlcon.Close();
        }

        // function for autocomplete IcaoDesignator Dropbdown
        void AutoCompleteText()
        {
            comboIcao.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboIcao.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // coll is the variable defined to hold the collection - this should be the data from the database table
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            // database connection and query
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "SELECT [IcaoDesignator] FROM tbl_Airlines order by [IcaoDesignator] ASC";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    string icao = myReader.GetString(0);
                    coll.Add(icao);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }
            // load the autocomplete data into combobox
            comboIcao.AutoCompleteCustomSource = coll;
        }

        // This code takes the loaded data based on the [IcaoDesignator] chosen in the comboICAO box and fills the remaining textfields
        private void comboIcao_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");

            string sql = "SELECT * FROM tbl_Airlines WHERE [IcaoDesignator] = '" + comboIcao.Text + "';";

            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                // load data int Airline Information text boxes
                while (myReader.Read())
                {
                    string iata = myReader.GetString(1);
                    string icao = myReader.GetString(2);
                    string threeDigit = myReader.GetInt32(3).ToString();
                    string airlineName = myReader.GetString(4);
                    string country = myReader.GetString(5);
                    string hub1 = myReader.GetString(6);
                    string hub2 = myReader.GetString(7);
                    string airlineDetails = myReader.GetString(8);

                    txtIata.Text = iata;
                    txt3Digit.Text = threeDigit;
                    txtAirlineName.Text = airlineName;
                    txtCountry.Text = country;
                    txtHub1.Text = hub1;
                    txtHub2.Text = hub2;
                    txtAirlineDetails.Text = airlineDetails;
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }

            // clear comboEnvironment dropdown to display all changes
            comboEnvironment.Text = "";

            // clear the txtSearch textbox
            txtSearch.Text = "";

            // clear the comboSearch textbox
            comboSearch.Text = "";

            // Load data to Recent Changes datagrid
            LoadTable();

            // load the Environment dropdown
            FillEnvironmentComboBox();

            if (dataGrid_RecentChanges.Rows.Count > 0)
            {
                // fill the individual Textboxes with the latest change
                DisplayRecentSelectResult();
            }
            else
            {
                // do nothing
            }

            // show refresh label
            labelRefreshIndicator.Visible = true;

            // enable timer
            timerNotification.Enabled = true;
        }

        private void comboEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load data to Recent Changes datagrid
            LoadTable();

            if (dataGrid_RecentChanges.Rows.Count > 0)
            {
                // fill the individual Textboxes with the latest change
                DisplayRecentSelectResult();

                // load ID of change as integer into variable
                //LastLoadedItem = Convert.ToInt32(txtId.Text);
                //MessageBox.Show("your item is: " + LastLoadedItem);
            }
            else
            {
                //comboIcao.Text = "";
                //comboEnvironment.Text = "";
                //MessageBox.Show("Please select a customer from the ICAO dropdown first!");
            }

            // show refresh label
            labelRefreshIndicator.Visible = true;

            // enable timer
            timerNotification.Enabled = true;
        }

        // the recentChanges table results layout
        void DisplayRecentChangesResults()
        {
            // change the dataGrid_RecentChanges table layout
            dataGrid_RecentChanges.Columns[0].Width = 30;
            //dataGrid_RecentChanges.Columns[0].Visible = false;  // id
            dataGrid_RecentChanges.Columns[1].Width = 170;
            dataGrid_RecentChanges.Columns[1].DefaultCellStyle.Format = "dd-MM-yyyy hh:mm:ss"; // Timestamp
            dataGrid_RecentChanges.Columns[2].Width = 150;      // Department
            dataGrid_RecentChanges.Columns[3].Width = 100;      //Environment
            dataGrid_RecentChanges.Columns[4].Width = 220;      // Component
            dataGrid_RecentChanges.Columns[5].Width = 230;
            dataGrid_RecentChanges.Columns[5].Visible = false;  // ChangedFrom
            dataGrid_RecentChanges.Columns[6].Width = 230;
            dataGrid_RecentChanges.Columns[6].Visible = false;  // [ChangedTo]
            dataGrid_RecentChanges.Columns[7].Width = 150;      // [CreatedBy]
            dataGrid_RecentChanges.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;       // [Purpose]
        }

        // this function holds the assignment to the individual text boxes to display the selected record details
        void DisplayRecentSelectResult()
        {
            // fill the individual Textboxes
            txtId.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[0].Value.ToString(); // Id
            txtTimestamp.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[1].Value.ToString(); // Timestamp
            txtDepartement.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[2].Value.ToString(); // Department
            txtEnvironment.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[3].Value.ToString(); //Environment
            txtComponent.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[4].Value.ToString(); // Component
            txtChangedFrom.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[5].Value.ToString(); // ChangedFrom
            txtChangedTo.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[6].Value.ToString(); // [ChangedTo]
            txtPurpose.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[8].Value.ToString();  // [Purpose]
            rtfPurpose.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[8].Value.ToString();  // [Purpose RTF]
            txtAuthor.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[7].Value.ToString(); // [CreatedBy]
        }

        // function to automatically load the data on selection of customer
        void LoadTable()
        {
            // This code takes the loaded data based on the [IcaoDesignator] chosen in the comboICAO box and fills the remaining textfields
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            // the query
            string sql;

            sql = "SELECT [Id], [Timestamp] as 'Changed', [Department], [Environment],  [Component], [ChangedFrom], [ChangedTo], [CreatedBy] as 'Author', [Purpose] FROM tbl_ChangeTracking";

            if (comboEnvironment.ToString().Equals(""))
            {
                sql = sql + " WHERE [AirlineId] like '%" + comboIcao.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
            }
            else
            {
                sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
            }

            if (comboIcao.SelectedItem == null)
            {
                sql = "SELECT [Id], [Timestamp] as 'Changed', [Department], [Environment],  [Component], [ChangedFrom], [ChangedTo], [CreatedBy] as 'Author', [Purpose]  FROM tbl_ChangeTracking order by [Id] DESC";
                // empty all Airline Information text boxes
                txtIata.Text = "";
                txt3Digit.Text = "";
                txtAirlineName.Text = "";
                txtHub1.Text = "";
                txtHub2.Text = "";
                txtCountry.Text = "";
                txtAirlineDetails.Text = "";
            }
            SqlCommand cmd = new SqlCommand(sql, sqlcon);

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                DataTable dbdataset = new DataTable();
                sda.Fill(dbdataset);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dbdataset;
                dataGrid_RecentChanges.DataSource = bSource;
                sda.Update(dbdataset);

            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }

            // change the dataGrid_RecentChanges table layout
            DisplayRecentChangesResults();
        }

        private void timerRefreshRecentChanges_Tick(object sender, EventArgs e)
        {
            // only refresh if ther is no text in the search box
            if (String.IsNullOrEmpty(txtSearch.Text) || String.IsNullOrEmpty(comboSearch.Text) || (comboSearch.SelectedIndex == -1))
            {
                // Load data to Recent Changes datagrid
                LoadTable();

                // change the message
                labelRefreshIndicator.ForeColor = Color.FromArgb(25, 152, 97);
                labelRefreshIndicator.Text = "data refresh";

                // show refresh label
                labelRefreshIndicator.Visible = true;

                // enable timer
                timerNotification.Enabled = true;
            }
            else
            {

                // show refresh label
                labelRefreshIndicator.Visible = true;
                // change the message and keep it on the screen
                labelRefreshIndicator.ForeColor = Color.Red;
                labelRefreshIndicator.Text = "refresh suspended - clear 'text search' field to resume refresh";
            }

        }

        private void timerNotification_Tick(object sender, EventArgs e)
        {
            // show refresh label
            labelRefreshIndicator.Visible = false;

            // disable timer
            timerNotification.Enabled = false;

        }

        private void dataGrid_RecentChanges_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // validation
            if (dataGrid_RecentChanges.Rows.Count > 0)
            {
                // fill the individual Textboxes
                DisplayRecentSelectResult();

                // load ID of change as integer into variable
                LastLoadedItem = Convert.ToInt32(txtId.Text);
                //MessageBox.Show("your item is: " + LastLoadedItem);
            }
            else
            {
                MessageBox.Show("Please select a customer from the ICAO dropdown first!");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // This code takes the loaded data based on the [IcaoDesignator] chosen in the comboICAO box and fills the remaining textfields
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");

            string sql;
            sql = "SELECT [Id], [Timestamp] as 'Changed', [Department], [Environment],  [Component], [ChangedFrom], [ChangedTo], [CreatedBy] as 'Author', [Purpose] FROM tbl_ChangeTracking";

            if (comboSearch.Text == "Component")
            {
                // the query
                if (comboEnvironment.ToString().Equals(""))
                {
                    sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [Component] like '%" + txtSearch.Text + "%' AND ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                else
                {
                    sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%' AND [Component] like '%" + txtSearch.Text + "%'ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                SqlCommand cmd = new SqlCommand(sql, sqlcon);
                try
                {
                    SqlDataAdapter sda = new SqlDataAdapter();
                    sda.SelectCommand = cmd;
                    DataTable dbdataset = new DataTable();
                    sda.Fill(dbdataset);
                    BindingSource bSource = new BindingSource();

                    bSource.DataSource = dbdataset;
                    dataGrid_RecentChanges.DataSource = bSource;
                    sda.Update(dbdataset);
                }
                catch (Exception exptn)
                {
                    // should maybe replaced by hidden Text field!!!! 
                    MessageBox.Show(exptn.Message);
                }
            }
            else if (comboSearch.Text == "Created By")
            {
                // the query
                if (comboEnvironment.ToString().Equals(""))
                {
                    sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [CreatedBy] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                else
                {
                    sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%'AND [CreatedBy] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                SqlCommand cmd = new SqlCommand(sql, sqlcon);
                try
                {
                    SqlDataAdapter sda = new SqlDataAdapter();
                    sda.SelectCommand = cmd;
                    DataTable dbdataset = new DataTable();
                    sda.Fill(dbdataset);
                    BindingSource bSource = new BindingSource();

                    bSource.DataSource = dbdataset;
                    dataGrid_RecentChanges.DataSource = bSource;
                    sda.Update(dbdataset);
                }
                catch (Exception exptn)
                {
                    // should maybe replaced by hidden Text field!!!! 
                    MessageBox.Show(exptn.Message);
                }
            }
            else if (comboSearch.Text == "Department")
            {
                // the query
                if (comboEnvironment.ToString().Equals(""))
                {
                    sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [Department] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                else
                {
                    sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%' AND [Department] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                SqlCommand cmd = new SqlCommand(sql, sqlcon);
                try
                {
                    SqlDataAdapter sda = new SqlDataAdapter();
                    sda.SelectCommand = cmd;
                    DataTable dbdataset = new DataTable();
                    sda.Fill(dbdataset);
                    BindingSource bSource = new BindingSource();

                    bSource.DataSource = dbdataset;
                    dataGrid_RecentChanges.DataSource = bSource;
                    sda.Update(dbdataset);
                }
                catch (Exception exptn)
                {
                    // should maybe replaced by hidden Text field!!!! 
                    MessageBox.Show(exptn.Message);
                }
            }
            else if (comboSearch.Text == "Purpose")
            {
                // the query
                if (comboEnvironment.ToString().Equals(""))
                {
                    sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [Purpose] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                else
                {
                    sql = sql + " WHERE [AirlineId] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%' AND [Purpose] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                SqlCommand cmd = new SqlCommand(sql, sqlcon);
                try
                {
                    SqlDataAdapter sda = new SqlDataAdapter();
                    sda.SelectCommand = cmd;
                    DataTable dbdataset = new DataTable();
                    sda.Fill(dbdataset);
                    BindingSource bSource = new BindingSource();

                    bSource.DataSource = dbdataset;
                    dataGrid_RecentChanges.DataSource = bSource;
                    sda.Update(dbdataset);
                }
                catch (Exception exptn)
                {
                    // should maybe replaced by hidden Text field!!!! 
                    MessageBox.Show(exptn.Message);
                }
            }


            // change the dataGrid_RecentChanges table layout
            DisplayRecentChangesResults();

            // validation
            if (dataGrid_RecentChanges.Rows.Count > 0)
            {
                // do nothing at the moment
            }
            else
            {
                //comboIcao.Text = "";
                //comboEnvironment.Text = "";
                //MessageBox.Show("Please select a customer from the ICAO dropdown first!");
            }
        }
        private void comboSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "INSERT INTO tbl_ChangeTracking ([AirlineId],[Timestamp],[Department],[Environment],[Component],[ChangedFrom],[ChangedTo],[Purpose],[CreatedBy]) SELECT '" + comboIcao.Text + "',getUTCdate(), '" + txtDepartement.Text + "','" + txtEnvironment.Text + "', '" + txtComponent.Text + "', '" + txtChangedFrom.Text + "', '" + txtChangedTo.Text + "', '" + txtPurpose.Text + "', '" + txtAuthor.Text + "'; ";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            try
            {
                sqlcon.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("New record inserted!");
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
                // show new records
                LoadTable();
            }
        }
    }
}




