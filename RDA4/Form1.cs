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

            PanelAddCustomer.Visible = false;
            PanelAddVehicle.Visible = false;          
            PanelPlaceOrder.Visible = false;
            PanelHome.Visible = true;

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

                PanelHome.Visible = true;
                
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
           
            //   PanelAddVehicle.Dock = DockStyle.Top;
            PanelHome.Visible = false;
            PanelAddVehicle.Visible = true;
            //    PanelAddCustomer.Visible = false;
            //    PanelPlaceOrder.Visible = false;






        }

        // home button on add customer page
        private void VehicleHome_Click(object sender, EventArgs e)
        {
            PanelHome.Visible = true;
            
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
           
            PanelAddVehicle.Visible = false;
            PanelAddCustomer.Visible = true;
            PanelHome.Visible = false;

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
                PanelHome.Visible = true;
                PanelAddCustomer.Visible = false;
            }


        }

        // place order from home
        private void PlaceOrder_Click(object sender, EventArgs e)
        {
            
            PanelHome.Visible = false;
            PanelAddCustomer.Visible = false;
            PanelAddVehicle.Visible = false;
            PanelPlaceOrder.Visible = true;
        }

        // place order page
        private void FinalOrder_Click(object sender, EventArgs e)
        {
            PanelHome.Visible = true;
            PanelAddCustomer.Visible = true;
            PanelAddVehicle.Visible = true;
        }

        // add customer from place order page
        private void AddCustomerFromOrder_Click(object sender, EventArgs e)
        {
            PanelAddCustomer.Visible = true;
        }
    }

   
}
