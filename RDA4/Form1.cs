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
            catch (Exception e)
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
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                cn.Close();
            }

 
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
                if (combo == null)
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
            catch (Exception ex)
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


            if (vyear == -1 || kms == -1 || wprice == -1)
            {
                flag = false;
            }

            int branchid = 0;

            if (flag)
            {
                branchid = BranchId(branch);
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

        // Return the branch id for selected branch
        static int BranchId(string branch)
        {
            int branchid = 0;

            if (branch == "Sportsworld") branchid = 1001;
            else if (branch == "Guelph Auto Mall") branchid = 1002;
            else branchid = 1003;

            return branchid;
        }

        // check if string is empty or not
        static bool IsValid(string box, int length)
        {
            bool flag = true;

            if (box.Length == 0)
                flag = false;

            if (length == 11 && box.Length != 11)
                flag = false;

            return flag;
        }

        // add vehicle from home
        private void AddVehicle_Click(object sender, EventArgs e)
        {
            PanelAddVehicle.BringToFront();
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
                PanelAddCustomer.SendToBack();
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

            bool done = false;

            bool flag = true;
            bool tempflag = true;
            bool exist = false;
            string query = "";
            string check = "";


            string phone = phoneid.Text;
            tempflag = ValidatePhone(phone);
            Error(phoneid, null, tempflag, "Please enter a valid phone number (XXX-XXX-XXXX)", 0);
            if (!tempflag) flag = false;


            // get VIN and validate
            string vin = vid.Text;
            tempflag = ValidateVIN(vin);
            Error(vid, null, tempflag, "Please enter valid VIN (eg 12345678YEA -- 8 digits + 3 chars", 0);
            if (!tempflag) flag = false;

            // Get dealer and validate
            int branchid = 0;
            int valid = 0;
            string dealer = dealerid.Text;
            if (dealer == "")
            {
                valid = -1;
                flag = false;
            }
            else
            {
                valid = 0;
                branchid = BranchId(dealer);
            }
            Error(null, dealerid, tempflag, "Required Field", valid);

            // get date
            string date = orderdate.Value.ToString("yyyy-MM-dd");

            // get tradein
            tempflag = true;
            int tradein = ParseInt(tradeinbox.Text);
            if (tradein < -1) {
                flag = false;
                tempflag = false;
            }
            Error(tradeinbox, null, tempflag, "Please enter valid positive number.", 0);


            valid = 0;
            string orderstatus = orderstatusbox.Text;
            if (orderstatus == "")
            {
                valid = -1;
                flag = false;
            }
            Error(null, orderstatusbox, false, "Required Field", valid);



            if (flag)
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
                    if (check == "")
                    {
                        MessageBox.Show("Could not find the vehicle you're searching for.", "Error: Not Found");
                    }
                    else
                    {
                        query = "SELECT make FROM vehicle where dealerid=" + branchid.ToString() +
                            " AND vin='" + vin + "';";
                        check = GetDB(query, "make", false);
                        //MessageBox.Show("-----" + check + "-------");
                        if (check.ToLower() == "")
                            MessageBox.Show("Vehicles is not at this dealership.");
                        else
                        {
                            query = "SELECT instock FROM vehicle where vin='" + vin + "';";
                            check = GetDB(query, "instock", false);
                            if (check.ToLower() == " no")
                            {
                                MessageBox.Show("Vehicle is currenlty not in stock.");
                            }
                            else
                                if (check.ToLower() == " hold")
                            {
                                MessageBox.Show("Vehicles is currently on hold by a customer.");
                            }
                            else
                            {
                                done = true;
                            }

                        }
                    }
                }
                else
                    MessageBox.Show("Could not find customer in database. Please click 'Add Customer' to add them.", "Error: Not Found");

                if (done)
                {
                    string status = "";

                    // check whether customer has paid or put on 'Hold'
                    if (orderstatus.ToLower() == "hold")
                    {
                        status = "'Hold'";
                    }
                    else
                    {
                        status = "'PAID'";
                    }

                    // get the price of the car and calculate sale price
                    query = "SELECT wprice FROM vehicle where vin='" + vin + "';";
                    check = GetDB(query, "wprice", false);
                    double sprice;
                    double.TryParse(check, out sprice);
                    double diff = sprice - tradein;
                    sprice = diff + (diff * 0.13);
                    DialogResult Confirmation = MessageBox.Show("Are you sure to proceed?", "Confirm Purchase", MessageBoxButtons.YesNo);

                    // Update instock flag
                    if (Confirmation == DialogResult.Yes)
                    {
                        string stock = "";
                        if (status.Contains("Paid"))
                            stock = "'No'";
                        else
                            stock = status;
                        query = "UPDATE Vehicle " +
                            "SET instock=" + stock + " where vin='" + vin + "';";

                        GetDB(query, "", true);

                        // get the latest orderid
                        query = "SELECT max(orderid) AS maximum from orders;";
                        check = GetDB(query, "maximum", false);
                        int orderid = ParseInt(check);
                        if (orderid <= 5000)
                        {
                            orderid = 5000;
                        }
                        orderid++;

                        // insert order into Ordertable
                        query = "INSERT INTO Orders (OrderID, OrderDate, OrderStatus) " +
                            "VALUES " +
                            "(" + orderid.ToString() + ", '" + date + "', " + status + ");";
                        GetDB(query, "", true);

                        // get customer id for phonenumber from database
                        query = "SELECT customerid FROM customer where phone='" + phone + "';";
                        check = GetDB(query, "customerid", false);
                        int customerid = ParseInt(check);


                        // get latest orderlineid
                        int orderlineid = 0;
                        query = "SELECT max(orderlineid) AS maximum FROM orderline;";
                        check = GetDB(query, "maximum", false);
                        orderlineid = ParseInt(check);
                        if (orderlineid <= 200)
                        {
                            orderlineid = 200;
                        }
                        orderlineid++;




                        // insert into orderline
                        query = "INSERT INTO OrderLine (OrderLineID, OrderID, CustomerID, DealerID, VIN, TradeIn, sprice) " +
                            "VALUES " +
                            "(" + orderlineid.ToString() + ", " + orderid.ToString() + ", " + customerid.ToString() + ", "
                            + branchid.ToString() + ", '" + vin + "', " + tradein.ToString() + ", " + sprice.ToString() + ")";

                        // execute orderline query
                        GetDB(query, "", true);

                        MessageBox.Show("Order Successfully placed!");

                        if (tradein != 0)
                        {
                            MessageBox.Show("Please enter the details of tradein car.");
                            pricebox.Text = tradein.ToString();
                            PanelAddVehicle.BringToFront();
                        }

                        PanelPlaceOrder.SendToBack();
                    }

                }
            }

        }

        // validate VIN (vehicle identification number)
        static bool ValidateVIN(string vin)
        {
            bool flag = true;

            if (vin.Length != 11)
                flag = false;

            if (flag)
            {
                for (int i = 0; i < vin.Length; i++)
                {
                    if (i < 8)
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
            PanelAddCustomer.BringToFront();
        }


        // validate phone number
        static bool ValidatePhone(string phone)
        {
            bool flag = true;
            if (phone.Length != 12)
                flag = false;

            if (flag)
            {
                for (int i = 0; i < phone.Length; i++)
                {
                    if (i == 3 || i == 7)
                    {
                        if (phone[i] != '-')
                            flag = false;

                        continue;
                    }

                    if (!(phone[i] >= '0') || !(phone[i] <= '9'))
                    {
                        flag = false;
                    }
                }
            }


            return flag;
        }


        private void HomeButton(object sender, EventArgs e)
        {
            PanelHome.BringToFront();
            ClearAllTextBoxes();
        }

        private void ClearAllTextBoxes()
        {
            // clear all textboxes of add vehicle page
            vinbox.Clear();
            makebox.Clear();
            modelbox.Clear();
            colourbox.Clear();
            kmsbox.Clear();
            pricebox.Clear();


            // clear all textboxes of add customer
            firstnamebox.Clear();
            lastnamebox.Clear();
            phonebox.Clear();
            

            // clear all textboxes of place order
            phoneid.Clear();
            vid.Clear();
            tradeinbox.Text = "0";

            OrderDetails.Clear();

        }

        private void OrderHistory_Click(object sender, EventArgs e)
        {
            PanelOrderHistory.BringToFront();

            string query = "";
            string check = "";
            int count = 0;

            int min = 0;
            int max = 0;
            query = "SELECT max(orderid) as maximum from orders;";
            check = GetDB(query, "maximum", false);
            max = ParseInt(check);

            query = "Select min(orderid) as minimum from orders;";
            check = GetDB(query, "minimum", false);
            min = ParseInt(check);

            for (int i = min; i <= max; i++)
            {
                count++;

                query = "SELECT CONCAT(firstname, ' ', lastname, ' - ',  orderdate, ' - ', orderstatus, ' - ', " +
                    "make, ' ', model) as Details " +
                    "FROM orderline " +
                    "INNER JOIN customer on orderline.customerid=customer.customerid " +
                    "INNER JOIN orders on orderline.orderid=orders.orderid " +
                    "INNER JOIN vehicle on orderline.vin=vehicle.vin "
                    + "where orders.orderid=" + i + ";";

                check = GetDB(query, "Details", false);

                OrderDetails.Text = OrderDetails.Text + count + ". " + check + "\n";

            }
        }

        private void ModifyOrder_Click(object sender, EventArgs e)
        {
            PanelModifyOrder.BringToFront();
        }

        private void SearchCustomer_Click(object sender, EventArgs e)
        {
            bool validformat = true;
            string temp = Orderidbox.Text;
            string query = "";
            string check = "";
            bool exist = true;

            int orderid = ParseInt(temp);

            

            if (orderid < 1000 || orderid > 9999)
                validformat = false;

            Error(Orderidbox, null, validformat, "Please enter valid 4 digit id.", 0);

            if (validformat)
            {
                query = "SELECT orderdate from orders where orderid=" + orderid + ";";
                check = GetDB(query, "orderdate", false);

                if (check != "")
                {

                    query = "SELECT CONCAT(orderstatus, ' -- ', firstname, ' ', lastname, ' - ',  orderdate, ' - ', " +
                    "make, ' ', model) as Details " +
                    "FROM orderline " +
                    "INNER JOIN customer on orderline.customerid=customer.customerid " +
                    "INNER JOIN orders on orderline.orderid=orders.orderid " +
                    "INNER JOIN vehicle on orderline.vin=vehicle.vin "
                    + "where orders.orderid=" + orderid + ";";

                    check = GetDB(query, "Details", false);

                    RetrievedDetails.Text = check;


                   
                }
                else
                {
                    exist = false;
                }
                Error(Orderidbox, null, exist, "The Order id does not exist in database", 0);
            }



        }

        private void ChangeStatus_Click(object sender, EventArgs e)
        {
            bool Changeable = false;
            bool cancel = false;
            bool paid = false;

            string temp = RetrievedDetails.Text;
            temp = temp.Substring(0, 5);

            if (temp.ToLower() != " paid")
            {
                ep.SetError(RetrievedDetails, "");
                Changeable = true;

            }
            else
                ep.SetError(RetrievedDetails, "Status is already PAID. Can't be altered now!");


            if (Changeable)
            {
                string status = ModifyStatus.Text;
                if (status.ToLower() == "cncl")
                {
                    cancel = true;

                }
                else
                    if (status.ToLower() == "paid")
                    {
                        paid = true;
                    }

            }

            string query = "";
            string check = "";

            if(cancel)
            {
                temp = Orderidbox.Text;
                int orderid = ParseInt(temp);

                query = "SELECT instock FROM orderline " +
                    "INNER JOIN orders on orders.orderid=orderline.orderid " +
                    "INNER JOIN vehicle on vehicle.vin=orderline.vin " +
                    "where orders.orderid=" + orderid + ";";

                check = GetDB(query, "instock", false);
                if(check.ToLower().Contains("yes"))
                {
                    MessageBox.Show("The order is already cancelled.");
                }
                else
                {
                    query = "SELECT vehicle.vin from orderline " +
                        "INNER JOIN orders on orderline.orderid=orders.orderid " +
                        "INNER JOIN vehicle on orderline.vin=vehicle.vin " +
                        "where orders.orderid=" + orderid + ";";

                    check = GetDB(query, "vin", false);
                    string vin = check;
                    MessageBox.Show(vin);

                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    

                    // get the latest id
                    query = "SELECT max(orderid) as maximum from orders";
                    int newid = ParseInt(GetDB(query, "maximum", false)) + 1;

                    // insert into order table
                    query = "INSERT INTO Orders (OrderID, OrderDate, OrderStatus) " +
                        "VALUES " +
                        "(" + newid + ", '" + date + "', 'CNCL');";

                    GetDB(query, "", true);

                    // update instock flag
                    query = "UPDATE vehicle SET instock='Yes' where vin='" + vin + "';";
                    GetDB(query, "", true);

                }
                
            }
        }
    } 
    
   
}
