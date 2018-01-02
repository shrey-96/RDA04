using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace RDA4
{
    public partial class Form1 : Form
    {

        MySqlConnection cn;

        static MySqlCommand cmd; 

        public Form1()
        {
            InitializeComponent();

            try
            {
                cn = new MySqlConnection("Server = localhost; Uid = root; Password = Cc7591746;" +
                "Database = stwally; Port = 3306");

                cmd = new MySqlCommand();
            }
            catch(Exception e)
            {
                MessageBox.Show("Error: " + e);
            }
        }

        // first time form loads
        private void Form1_Load(object sender, EventArgs e)
        {
            // open the connection and set the sql command variable to this connection

            try
            {
                cn.Open();
                cmd.Connection = cn;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                cn.Clone();
            }

            // PanelAddCustomer.Visible = false;
            // PanelAddVehicle.Visible = false;          
            // PanelPlaceOrder.Visible = false;
            // PanelHome.Visible = true;

            PanelHome.BringToFront();

            orderdate.MaxDate = DateTime.Now.Date;

        }


        // get or insert into data base
        static string GetDB(string query, string column, bool insert)
        {
            // set the query to command text
            cmd.CommandText = query;
            MySqlDataReader reader;
            reader = cmd.ExecuteReader();
            string final = "";

            if (insert == false)
            {
                while (reader.Read())
                {
                    final = final + " " + reader[column];
                }
            }
            reader.Close();

            return final;
        }

        // validate positive integer
        static int ParseInt(string value)
        {
            int result = 0;

            bool flag = int.TryParse(value, out result);
            if (flag == false)
                result = -1;

            return result;
        }

        // error provider based on conditions
        private void Error(TextBox box, ComboBox combo, bool valid, string msg, int ComboFlag)
        {
            try
            {
                if(combo == null)
                {
                    if (!valid)
                        ep.SetError(box, msg);
                    else
                        ep.SetError(box, "");
                }
                else
                {
                    if (ComboFlag == -1)
                        ep.SetError(combo, msg);
                    else
                        ep.SetError(combo, "");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        // add vehicle page
        private void SubmitAddV_Click(object sender, EventArgs e)
        {
            bool flag = true;
            bool tempflag = false;

            string vin = vinbox.Text;
            tempflag = IsValid(vin, 11);
            Error(vinbox, null, tempflag, "Please enter valid VIN (11 characters)", 0);
            if (!tempflag) flag = false;

            int vyear = ParseInt(yearbox.Text);
            Error(null, yearbox, false, "Please choose year from droplist!", vyear);
            if (vyear == -1) flag = false;


            string make = makebox.Text;
            tempflag = IsValid(make, -1);
            Error(makebox, null, tempflag, "Required Field", 0);
            if (!tempflag) flag = false;

            string model = modelbox.Text;
            tempflag = IsValid(model, -1);
            Error(modelbox, null, tempflag, "Required Field", 0);
            if (!tempflag) flag = false;

            string colour = colourbox.Text;
            tempflag = IsValid(colour, -1);
            Error(colourbox, null, tempflag, "Required Field", 0);
            if (!tempflag) flag = false;


            int kms = ParseInt(kmsbox.Text);
            if (kms < 0)
            {
                ep.SetError(kmsbox, "Please enter valid positive number.");
                flag = false;
            }
            else ep.SetError(kmsbox, "");

            int wprice = ParseInt(pricebox.Text);
            if (wprice < 0) 
            {
                ep.SetError(pricebox, "Please enter valid positive number.");
                flag = false;
            }
            else ep.SetError(pricebox, "");

            string instock = instockbox.Text;
            if (instock == "")
            {
                ep.SetError(instockbox, "Required Field");
                flag = false;
            }
            else
                ep.SetError(instockbox, "");

            string branch = branchbox.Text;
            if (branch == "")
            {
                ep.SetError(branchbox, "Please select a branch");
                flag = false;
            }
            else
                ep.SetError(branchbox, "");
               

            if(vyear == -1 || kms == -1 || wprice == -1)
            {
                flag = false;
            }

            int branchid = 0;

            if (flag)
            {
                if (branch == "Sportsworld") branchid = 1001;
                else if (branch == "Guelph Auto Mall") branchid = 1002;
                else branchid = 1003;
            }

           // MessageBox.Show(branchid.ToString());
            


            if (flag)
            {
                string query =
                    "INSERT INTO Vehicle (VIN, V_Year, Make, Model, Colour, Kms, wPrice, inStock, DealerID) Values " +
                    "('" + vin + "', " + vyear.ToString() + ", '" + make + "', '" + model + "', '" + colour + "', " + kms.ToString() + ", " + wprice.ToString()
                     + ", '" + instock + "', " + branchid.ToString() + ");";

                MessageBox.Show(query);
                GetDB(query, "", true);

                //PanelHome.Visible = true;
                PanelHome.BringToFront();
                
            }
          
            
        }

        // check if string is empty or not
        static bool IsValid(string box, int length)
        {
            bool flag = true;

            if (box.Length == 0)
                flag = false;

            if(length == 11 && box.Length != 11)           
                flag = false;
            
            return flag;
        }

        // add vehicle from home
        private void AddVehicle_Click(object sender, EventArgs e)
        {
            PanelAddVehicle.BringToFront();
        }

        // home button on add customer page
        private void VehicleHome_Click(object sender, EventArgs e)
        {
            PanelHome.BringToFront(); 

           vinbox.Clear();
           makebox.Clear();
           modelbox.Clear();
           colourbox.Clear();
           kmsbox.Clear();
           pricebox.Clear();
           
        }

        // add customer from home
        private void AddCustomer_Click(object sender, EventArgs e)
        {
            PanelAddCustomer.BringToFront();

        }

        // add customer page
        private void AddCustomerDB_Click(object sender, EventArgs e)
        {
            bool flag = true;
            bool tempflag = false;

            string firstname = firstnamebox.Text;
            tempflag = IsValid(firstname, 0);
            if (!tempflag) flag = false;
            Error(firstnamebox, null, tempflag, "Required Field", 0);

            string lastname = lastnamebox.Text;
            tempflag = IsValid(lastname, 0);
            if (!tempflag) flag = false;
            Error(lastnamebox, null, tempflag, "Required Field", 0);

            string phone = phonebox.Text;
            if (phone.Length != 12)
            {
                tempflag = false;
                flag = false;
            }
            else
                tempflag = true;
            Error(phonebox, null, tempflag, "Invalid entry. Supported Format (XXX-XXX-XXXX).", 0);

            if (flag)
            {
                string query = "SELECT max(customerid) as maximum from customer;";
                string result = GetDB(query, "maximum", false);

                int newid = ParseInt(result);
                newid++;

                query = "INSERT INTO customer(CustomerID, FirstName, LastName, Phone) VALUES " +
                   "(" + newid.ToString() + ", '" + firstname + "', '" + lastname + "', '" + phone + "');";

                GetDB(query, "", true);
                MessageBox.Show("Successfully added customer!");
            }

            if (flag)
            {
                PanelHome.BringToFront();
            }


        }

        // place order from home
        private void PlaceOrder_Click(object sender, EventArgs e)
        {
            PanelPlaceOrder.BringToFront();
        }

        // place order click on place order page
        private void FinalOrder_Click(object sender, EventArgs e)
        {
            PanelHome.BringToFront();


            bool flag = true;
            bool tempflag = false;
            bool exist = false;
            string query = "";
            string check = "";



            string phone = phoneid.Text;
            tempflag = ValidatePhone(phone);
            Error(phoneid, null, tempflag, "Please enter a valid phone number (XXX-XXX-XXXX)", 0);
            if (!tempflag) flag = false;



            string vin = vid.Text;
            tempflag = ValidateVIN(vin);
            Error(vid, null, tempflag, "Please enter valid VIN (eg 12345678YEA -- 8 digits + 3 chars", 0);
            if (!tempflag) flag = false;

            int valid = 0;
            string dealer = dealerid.Text;
            if (dealer == "")
            {
                valid = -1;
                flag = false;
            }
            else valid = 0;
            Error(null, dealerid, tempflag, "Required Field", valid);
            

            string date = orderdate.Value.ToString("yyyy-MM-dd");


            int tradein = ParseInt(tradeinbox.Text);
            if(tradein == -1) {
                flag = false;
                tempflag = false;
            }
            Error(tradeinbox, null, tempflag, "Please enter valid positive number.", 0);


            valid = 0;
            string orderstatus = orderstatusbox.Text;
            if(orderstatus == "")
            {
                valid = -1;
                flag = false;
            }
            Error(null, orderstatusbox, false, "Required Field", valid);


            bool instockflag = true;
            bool vehicleflag = true;

            if(flag)
            {
                query = "SELECT customerid FROM customer where phone='" + phone + "';";
                check = GetDB(query, "customerid", false);
                if (check != "")
                {
                    exist = true;
                }

                if (exist)
                {
                    query = "SELECT make from Vehicle where vin='" + vin + "';";
                    check = GetDB(query, "make", false);
                    if(check == "")
                    {
                        MessageBox.Show("Could not find the vehicle you're searching for.", "Error: Not Found");
                    }
                }
                else
                    MessageBox.Show("Could not find customer in database. Please click 'Add Customer' to add them.", "Error: Not Found");

            }


            // MessageBox.Show("Order Placed!");
        }

        // validate VIN (vehicle identification number)
        static bool ValidateVIN(string vin)
        {
            bool flag = true;

            if (vin.Length != 11)
                flag = false;

            if(flag)
            {
                for(int i = 0; i<vin.Length; i++)
                {
                    if(i<8)
                    {
                        if (!(vin[i] >= '0') || !(vin[i] <= '9'))
                            flag = false;
                    }
                    else
                    {
                        if (!char.IsLetter(vin[i]))
                            flag = false;
                    }
                }
            }

            return flag;
        }


        // add customer on place order page
        private void AddCustomerFromOrder_Click(object sender, EventArgs e)
        {
            PanelHome.BringToFront();       
        }

        static bool ValidatePhone(string phone)
        {
            bool flag = true;
            if (phone.Length != 12)
                flag = false;

            if(flag)
            {
                for(int i = 0; i<phone.Length; i++)
                {
                    if(i == 3 || i == 7)
                    {
                        if (phone[i] != '-')
                            flag = false;

                        continue;
                    }

                    if(!(phone[i] >= '0') || !(phone[i] <= '9'))
                    {
                        flag = false;
                    }
                }
            }


            return flag;
        }

        private void HomeOnAddCustomer_Click(object sender, EventArgs e)
        {
            PanelHome.BringToFront();
        }

        private void HomeOnPlaceOrder_Click(object sender, EventArgs e)
        {
            PanelHome.BringToFront();
        }
    }
   
}
