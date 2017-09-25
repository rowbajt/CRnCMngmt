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
            FillDepartmentComboBox();
            FillEnvComboBox();
            FillComponentComboBox();
            // funcction loads data into the Environment dropdown
            FillEnvironmentComboBox();
            // function for Autocomplete dropdown boxes
            AutoCompleteTextIcaoDesignator();
            AutoCompleteTextDepartment();
            AutoCompleteComboEnv();
            AutoCompleteComboComponent();

            // function Load data into Recent Changes grid
            LoadTable();
            // function show/hide edit buttons
            InsertEditUpdateMode();           
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

        // initialize the Insert New button state
        int buttonInsertState = 0;
        // initialize the Update button state
        int buttonUpdateState = 0;

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

        // This code fills the comboboxes
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
        public void FillDepartmentComboBox()
        {
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "SELECT [Department] FROM tbl_Departments order by [Department] ASC";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    string sName = myReader.GetString(0);
                    comboDepartment.Items.Add(sName);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }

            sqlcon.Close();
        }
        public void FillEnvironmentComboBox()
        {
            // clear all data from combobox
            comboEnvironment.Items.Clear();

            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");

            string sql;

            if (comboIcao.SelectedItem != null)
            {
                sql = "SELECT distinct([Environment]) FROM tbl_ChangeTracking WHERE [IcaoDesignator] = '" + comboIcao.Text + "' order by [Environment] ASC";
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
        public void FillEnvComboBox()
        {
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "SELECT [Environment] FROM tbl_Environments order by [Environment] ASC";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    string sName = myReader.GetString(0);
                    comboEnv.Items.Add(sName);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }

            sqlcon.Close();
        }
        public void FillComponentComboBox()
        {
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "SELECT [Component] FROM tbl_Components order by [Component] ASC";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    string sName = myReader.GetString(0);
                    comboComponent.Items.Add(sName);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }

            sqlcon.Close();
        }

        // function for autocomplete comboboxes
        void AutoCompleteTextIcaoDesignator()
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
        void AutoCompleteTextDepartment()
        {
            comboDepartment.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboDepartment.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // coll is the variable defined to hold the collection - this should be the data from the database table
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            // database connection and query
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "SELECT [Department] FROM tbl_Departments order by [Department] ASC";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    string comboDepartment = myReader.GetString(0);
                    coll.Add(comboDepartment);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }
            // load the autocomplete data into combobox
            comboDepartment.AutoCompleteCustomSource = coll;
        }
        void AutoCompleteComboEnv()
        {
            comboEnv.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboEnv.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // coll is the variable defined to hold the collection - this should be the data from the database table
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            // database connection and query
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "SELECT [Environment] FROM tbl_Environments order by [Environment] ASC";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    string comboEnv = myReader.GetString(0);
                    coll.Add(comboEnv);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }
            // load the autocomplete data into combobox
            comboEnv.AutoCompleteCustomSource = coll;
        }
        void AutoCompleteComboComponent()
        {
            comboComponent.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboComponent.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // coll is the variable defined to hold the collection - this should be the data from the database table
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            // database connection and query
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "SELECT [Component] FROM tbl_Components order by [Component] ASC";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader myReader;
            try
            {
                sqlcon.Open();
                myReader = cmd.ExecuteReader();

                while (myReader.Read())
                {
                    string comboComponent = myReader.GetString(0);
                    coll.Add(comboComponent);
                }
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }
            // load the autocomplete data into combobox
            comboComponent.AutoCompleteCustomSource = coll;
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
            txtSearch.Clear();

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

                // clear text searchbox
                txtSearch.Clear();
            }
            else
            {
                //comboIcao.Clear();;
                //comboEnvironment.Clear();;
                //MessageBox.Show("Please select a customer from the ICAO dropdown first!");
            }

            // show refresh label
            labelRefreshIndicator.Visible = true;

            // enable timer
            timerNotification.Enabled = true;
        }
        private void comboSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear(); ;
        }
        
        // RecentChanges datagrid layout
        void DisplayRecentChangesResults()
        {
            // change the dataGrid_RecentChanges table layout
            dataGrid_RecentChanges.Columns[0].Width = 30;
            dataGrid_RecentChanges.Columns[1].Width = 35;       //Icao
            dataGrid_RecentChanges.Columns[2].Width = 170;
            dataGrid_RecentChanges.Columns[2].DefaultCellStyle.Format = "dd-MM-yyyy hh:mm:ss"; // Timestamp
            dataGrid_RecentChanges.Columns[3].Width = 150;      // Department
            dataGrid_RecentChanges.Columns[4].Width = 100;      //Environment
            dataGrid_RecentChanges.Columns[5].Width = 220;      // Component
            dataGrid_RecentChanges.Columns[6].Width = 230;
            dataGrid_RecentChanges.Columns[6].Visible = false;  // ChangedFrom
            dataGrid_RecentChanges.Columns[7].Width = 230;
            dataGrid_RecentChanges.Columns[7].Visible = false;  // [ChangedTo]
            dataGrid_RecentChanges.Columns[8].Width = 150;      // [CreatedBy]
            dataGrid_RecentChanges.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;       // [Purpose]
        }

        // this function holds the assignment to the individual text boxes to display the selected record details
        void DisplayRecentSelectResult()
        {
            // fill the individual Textboxes
            txtId.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[0].Value.ToString(); // Id
            txtIcao.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[1].Value.ToString(); // Id
            txtTimestamp.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[2].Value.ToString(); // Timestamp
            comboDepartment.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[3].Value.ToString(); // Department
            comboEnv.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[4].Value.ToString(); //Environment
            comboComponent.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[5].Value.ToString(); // Component
            txtChangedFrom.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[6].Value.ToString(); // ChangedFrom
            txtChangedTo.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[7].Value.ToString(); // [ChangedTo]
            txtPurpose.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[9].Value.ToString();  // [Purpose]
            txtAuthor.Text = dataGrid_RecentChanges.SelectedRows[0].Cells[8].Value.ToString(); // [CreatedBy]
        }
        
        // this function loads data to the datagridview
        void LoadTable()
        {
            // This code takes the loaded data based on the [IcaoDesignator] chosen in the comboICAO box and fills the remaining textfields
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            // the query
            string sql;

            sql = "SELECT [Id], [IcaoDesignator] as 'Icao', [Timestamp] as 'Changed', [Department], [Environment],  [Component], [ChangedFrom], [ChangedTo], [CreatedBy] as 'Author', [Purpose] FROM tbl_ChangeTracking";

            if (comboEnvironment.ToString().Equals(""))
            {
                sql = sql + " WHERE [IcaoDesignator] like '%" + comboIcao.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
            }
            else
            {
                sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
            }

            if (comboIcao.SelectedItem == null)
            {
                sql = "SELECT [Id], [IcaoDesignator] as 'Icao', [Timestamp] as 'Changed', [Department], [Environment],  [Component], [ChangedFrom], [ChangedTo], [CreatedBy] as 'Author', [Purpose]  FROM tbl_ChangeTracking order by [Id] DESC";
                // empty all Airline Information text boxes
                txtIata.Clear(); ;
                txt3Digit.Clear(); ;
                txtAirlineName.Clear(); ;
                txtHub1.Clear(); ;
                txtHub2.Clear(); ;
                txtCountry.Clear(); ;
                txtAirlineDetails.Clear(); ;
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
        // this function handles the behavior of the textboxes and buttons on Edit
        void InsertEditUpdateMode()
        {
            // INSERT BUTTON
            if (buttonInsertState == 0 && buttonUpdateState == 0)
            {
                // button layout
                buttonInsert.Enabled = true;
                buttonUpdate.Enabled = true;
                buttonDelete.Enabled = true;
                buttonInsert.BackColor = Color.White;
                buttonUpdate.BackColor = Color.White;
                buttonDelete.BackColor = Color.White;
                buttonInsert.FlatAppearance.BorderColor = Color.FromArgb(25, 152, 97);
                buttonUpdate.FlatAppearance.BorderColor = Color.FromArgb(25, 152, 97);
                buttonDelete.FlatAppearance.BorderColor = Color.FromArgb(25, 152, 97);

                buttonInsert.Text = "Insert New";

                // color the individual Textboxes
                txtId.BackColor = Color.FromArgb(224,224,224); // Id
                txtIcao.BackColor = Color.FromArgb(224, 224, 224); // Icao
                txtTimestamp.BackColor = Color.FromArgb(224, 224, 224); // Timestamp
                comboDepartment.BackColor = Color.FromArgb(224, 224, 224); // Department
                comboEnv.BackColor = Color.FromArgb(224, 224, 224); //Environment
                comboComponent.BackColor = Color.FromArgb(224, 224, 224); // Component
                txtChangedFrom.BackColor = Color.FromArgb(224, 224, 224); // ChangedFrom
                txtChangedTo.BackColor = Color.FromArgb(224, 224, 224); // [ChangedTo]
                txtPurpose.BackColor = Color.FromArgb(224, 224, 224);  // [Purpose]
                txtAuthor.BackColor = Color.FromArgb(224, 224, 224); // [CreatedBy]

                // boxes enabled or disabled
                txtId.Enabled = false;
                txtIcao.Enabled = false;
                txtTimestamp.Enabled = false;
                comboDepartment.Enabled = false;
                comboEnv.Enabled = false;
                comboComponent.Enabled = false;
                txtChangedFrom.Enabled = false;
                txtChangedTo.Enabled = false;
                txtPurpose.Enabled = false;
                txtAuthor.Enabled = false;
            }
            else if (buttonInsertState == 1 && buttonUpdateState == 0)
            {
                // clear the individual Textboxes
                txtId.Clear(); // Id
                // txtIcao.Clear(); //Icao
                txtTimestamp.Clear(); // Timestamp
                comboDepartment.Text = ""; // Department
                comboEnv.Text = ""; //Environment
                comboComponent.Text = ""; // Component
                txtChangedFrom.Clear(); // ChangedFrom
                txtChangedTo.Clear(); // [ChangedTo]
                txtPurpose.Clear();  // [Purpose]
                txtAuthor.Clear(); // [CreatedBy]

                // button layout
                buttonInsert.Enabled = true;
                buttonUpdate.Enabled = false;
                buttonDelete.Enabled = false;
                buttonInsert.BackColor = Color.White;
                buttonUpdate.BackColor = Color.FromArgb(224, 224, 224);
                buttonDelete.BackColor = Color.FromArgb(224, 224, 224);
                buttonInsert.FlatAppearance.BorderColor = Color.FromArgb(25, 152, 97);
                buttonUpdate.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                buttonDelete.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);

                buttonInsert.Text = "Save";

                // color the individual Textboxes
                txtId.BackColor = Color.FromArgb(224, 224, 224); // Id
                txtIcao.BackColor = Color.FromArgb(224, 224, 224); // Icao
                txtTimestamp.BackColor = Color.FromArgb(224, 224, 224); // Timestamp
                comboDepartment.BackColor = Color.FromArgb(224, 224, 224); // Department
                comboEnv.BackColor = Color.FromArgb(224, 224, 224); //Environment
                comboComponent.BackColor = Color.FromArgb(224, 224, 224); // Component
                txtChangedFrom.BackColor = Color.FromArgb(224, 224, 224); // ChangedFrom
                txtChangedTo.BackColor = Color.FromArgb(224, 224, 224); // [ChangedTo]
                txtPurpose.BackColor = Color.FromArgb(224, 224, 224);  // [Purpose]
                txtAuthor.BackColor = Color.FromArgb(224, 224, 224); // [CreatedBy]

                // boxes enabled or disabled
                txtId.Enabled = false;
                txtIcao.Enabled = true;
                txtTimestamp.Enabled = false;
                comboDepartment.Enabled = true;
                comboEnv.Enabled = true;
                comboComponent.Enabled = true;
                txtChangedFrom.Enabled = true;
                txtChangedTo.Enabled = true;
                txtPurpose.Enabled = true;
                txtAuthor.Enabled = true;
            }

            // UPDATE BUTTON
            if (buttonUpdateState == 0 && buttonInsertState == 0)
            {
                // button layout
                buttonInsert.Enabled = true;
                buttonUpdate.Enabled = true;
                buttonDelete.Enabled = true;
                buttonInsert.BackColor = Color.White;
                buttonUpdate.BackColor = Color.White;
                buttonDelete.BackColor = Color.White;
                buttonInsert.FlatAppearance.BorderColor = Color.FromArgb(25, 152, 97);
                buttonUpdate.FlatAppearance.BorderColor = Color.FromArgb(25, 152, 97);
                buttonDelete.FlatAppearance.BorderColor = Color.FromArgb(25, 152, 97);
                buttonUpdate.Text = "Edit";
            }
            else if (buttonUpdateState == 1 && buttonInsertState == 0)
            {
                // button layout
                buttonInsert.Enabled = false;
                buttonUpdate.Enabled = true;
                buttonDelete.Enabled = false;
                buttonDelete.BackColor = Color.FromArgb(224, 224, 224);
                buttonUpdate.BackColor = Color.White;
                buttonInsert.BackColor = Color.FromArgb(224, 224, 224);
                buttonInsert.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                buttonUpdate.FlatAppearance.BorderColor = Color.FromArgb(25, 152, 97);
                buttonDelete.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
                buttonUpdate.Text = "Update...";

                // boxes enabled or disabled
                txtId.Enabled = false;
                txtIcao.Enabled = true;
                txtTimestamp.Enabled = false;
                comboDepartment.Enabled = true;
                comboEnv.Enabled = true;
                comboComponent.Enabled = true;
                txtChangedFrom.Enabled = true;
                txtChangedTo.Enabled = true;
                txtPurpose.Enabled = true;
                txtAuthor.Enabled = true;

                //color the individual Textboxes
                txtId.BackColor = Color.FromArgb(224, 224, 224); // Id
                txtIcao.BackColor = Color.FromArgb(224, 224, 224); // Icao
                txtTimestamp.BackColor = Color.FromArgb(224, 224, 224); // Timestamp
                comboDepartment.BackColor = Color.FromArgb(224, 224, 224); // Department
                comboEnv.BackColor = Color.FromArgb(224, 224, 224); //Environment
                comboComponent.BackColor = Color.FromArgb(224, 224, 224); // Component
                txtChangedFrom.BackColor = Color.FromArgb(224, 224, 224); // ChangedFrom
                txtChangedTo.BackColor = Color.FromArgb(224, 224, 224); // [ChangedTo]
                txtPurpose.BackColor = Color.FromArgb(224, 224, 224);  // [Purpose]
                txtAuthor.BackColor = Color.FromArgb(224, 224, 224); // [CreatedBy]
            }
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

                // change the state of the button
                buttonInsertState = 0;
                buttonUpdateState = 0;

                // change label and visible state of Edit buttons, change textbox colors
                InsertEditUpdateMode();

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
            sql = "SELECT [Id], [IcaoDesignator] as 'Icao', [Timestamp] as 'Changed', [Department], [Environment],  [Component], [ChangedFrom], [ChangedTo], [CreatedBy] as 'Author', [Purpose] FROM tbl_ChangeTracking";

            if (comboSearch.Text == "Component")
            {
                // the query
                if (comboEnvironment.ToString().Equals(""))
                {
                    sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [Component] like '%" + txtSearch.Text + "%' AND ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                else
                {
                    sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%' AND [Component] like '%" + txtSearch.Text + "%'ORDER BY [Id] DESC, [Timestamp] DESC;";
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
                    sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [CreatedBy] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                else
                {
                    sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%'AND [CreatedBy] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
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
                    sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [Department] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                else
                {
                    sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%' AND [Department] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
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
                    sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [Purpose] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
                }
                else
                {
                    sql = sql + " WHERE [IcaoDesignator] = '" + comboIcao.Text + "' AND [Environment] like '%" + comboEnvironment.Text + "%' AND [Purpose] like '%" + txtSearch.Text + "%' ORDER BY [Id] DESC, [Timestamp] DESC;";
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
                //comboIcao..Clear();;
                //comboEnvironment.Clear();;
                //MessageBox.Show("Please select a customer from the ICAO dropdown first!");
            }
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            if (buttonInsertState == 0)
            {
                // change the state of the button
                buttonInsertState = 1;
                buttonUpdateState = 0;

                // change label and visible state of Edit buttons, change textbox colors
                InsertEditUpdateMode();

                // put Cursor in first textfield
                //txtDepartment.Focus();
                comboDepartment.Focus();

                // NOW datainput can start
                // data validation before insert is required!
            }
            else
            {
                SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
                string sql = "INSERT INTO tbl_ChangeTracking ([IcaoDesignator],[Timestamp],[Department],[Environment],[Component],[ChangedFrom],[ChangedTo],[Purpose],[CreatedBy]) SELECT '" + txtIcao.Text + "',getUTCdate(), '" + comboDepartment.Text + "','" + comboEnv.Text + "', '" + comboComponent.Text + "', '" + txtChangedFrom.Text + "', '" + txtChangedTo.Text + "', '" + txtPurpose.Text + "', '" + txtAuthor.Text + "'; ";
                SqlCommand cmd = new SqlCommand(sql, sqlcon);
                try
                {
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("New record inserted!");

                    // change the state of the button
                    buttonInsertState = 0;
                    buttonUpdateState = 0;

                    // change label and visible state of Edit buttons, change textbox colors
                    InsertEditUpdateMode();
                    
                    // show new records
                    LoadTable();
                }
                catch (Exception exptn)
                {
                    // should maybe replaced by hidden Text field!!!! 
                    MessageBox.Show(exptn.Message);
                }
            }

        }
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (buttonUpdateState == 0)
            {
                // change the state of the button
                buttonInsertState = 0;
                buttonUpdateState = 1;

                // change label and visible state of Edit buttons, change textbox colors
                InsertEditUpdateMode();

                // put Cursor in first textfield
                //txtDepartment.Focus();
                comboDepartment.Focus();

                // NOW datainput can start
                // data validation before insert is required!
            }
            else
            {
                SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
                string sql = "UPDATE tbl_ChangeTracking SET [IcaoDesignator] = '" + txtIcao.Text + "', [Timestamp] = getUTCdate(), [Department] = '" + comboDepartment.Text + "', [Environment] = '" + comboEnv.Text + "', [Component] = '" + comboComponent.Text + "', [ChangedFrom] = '" + txtChangedFrom.Text + "', [ChangedTo] = '" + txtChangedTo.Text + "', [Purpose] = '" + txtPurpose.Text + "', [CreatedBy] = '" + txtAuthor.Text + "' WHERE [Id] = '" + txtId.Text + "'; ";
                SqlCommand cmd = new SqlCommand(sql, sqlcon);
                try
                {
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Record updated!");

                    // change the state of the button
                    buttonInsertState = 0;
                    buttonUpdateState = 0;
                    // change label and visible state of Edit buttons, change textbox colors
                    InsertEditUpdateMode();
                    // show new records
                    LoadTable();
                }
                catch (Exception exptn)
                {
                    // should maybe replaced by hidden Text field!!!! 
                    MessageBox.Show(exptn.Message);
                }
            }
        }
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            SqlConnection sqlcon = new SqlConnection(@"Data Source=DESKTOP-V2G31EK\CMDATABASE;Initial Catalog=CustomerManagement;Integrated Security=True");
            string sql = "DELETE FROM tbl_ChangeTracking WHERE [Id] = '" + txtId.Text + "'; ";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            try
            {
                sqlcon.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Record deleted!");
                // change label and visible state of Edit buttons, change textbox colors
                InsertEditUpdateMode();
                // show new records
                LoadTable();
            }
            catch (Exception exptn)
            {
                // should maybe replaced by hidden Text field!!!! 
                MessageBox.Show(exptn.Message);
            }
        }
        private void TextboxEdit_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.LightBlue;
        }
        private void TextboxEdit_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.White;
        }

    }
}




