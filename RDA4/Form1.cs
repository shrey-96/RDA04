/*
* File:         Form1.cs
* Project:      RDA4 - STWally
* By:           Shreyansh Tiwari
* Date:         Janurary 3rd, 2017
* Description:  This file consists of main logic of the progrm. The logic include
*               validating the data, update, add the data to database. Read from the data
*               base and update the data for customer to see.
*/

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

        /* Name:           GetDB()
        *  Parameters:     string query - sql query to be sent to database
        *                  string column - name of column trying to retrieve data from
        *                  bool insert : true if inserting/updating data, false.
        *  Return value:   string - string returned by select statment
        *  Description:    This method is invoked to interact with SQL database, get and set
        *                  data over there.
        */
        static string GetDB(string query, string column, bool insert)
        {
            // set the query to command text
            cmd.CommandText = query;
            MySqlDataReader reader;

            // execute the query
            reader = cmd.ExecuteReader();
            string final = "";

            if (insert == false)
            {
                // read from reader
                while (reader.Read())
                {
                    final = final + " " + reader[column];
                }
            }

            // close reader
            reader.Close();

            return final;
        }

        /*  Name:          ParseInt()
        *  Parameters:     string value - the string that has integer value
        *  Return value:   int - return the parsed int
        *                  -1: if the string is not a valid value
        *  Description:    This method is invoked to parse the integer from string.
        */
        static int ParseInt(string value)
        {
            int result = 0;

            bool flag = int.TryParse(value, out result);
            if (flag == false)
                result = -1;

            return result;
        }

        /*  
        *  Name:           Error()
        *  Parameters:     TextBox box - the textbox near which error is to be shows
        *                  Combobox combo - combo box that needs to be displayed error near.
        *                  bool valid - whether or not validation succeeded or not
        *                  string msg - the message the needs to be shown with error.
        *                  int comboflag - the validation success flag for combo box
        *                  
        *  Return value:   null
        *  Description:    This method is invoked to display error on failed validation.
        *                  And also to remove the error once validated.
        */
        private void Error(TextBox box, ComboBox combo, bool valid, string msg, int ComboFlag)
        {
            try
            {
                // textbox
                if (combo == null)
                {
                    if (!valid)
                        ep.SetError(box, msg);
                    else
                        ep.SetError(box, "");
                }
                // combobox
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

        /* 
        *  Name:           SubmitAddV_Click()
        *  Parameters:     object, Event
        *  Return value:   null
        *  Description:    This method is invoked when add vehicle button is clicked.
        *                  It retrieves the data from textboxes/form, and validate them
        *                  and add them to database once validated
        */
        private void SubmitAddV_Click(object sender, EventArgs e)
        {
            bool flag = true;
            bool tempflag = false;

            // vin
            string vin = vinbox.Text;
            tempflag = IsValid(vin, 11);
            Error(vinbox, null, tempflag, "Please enter valid VIN (11 characters)", 0);
            if (!tempflag) flag = false;

            // year
            int vyear = ParseInt(yearbox.Text);
            Error(null, yearbox, false, "Please choose year from droplist!", vyear);
            if (vyear == -1) flag = false;

            // manufacture
            string make = makebox.Text;
            tempflag = IsValid(make, -1);
            Error(makebox, null, tempflag, "Required Field", 0);
            if (!tempflag) flag = false;

            // model
            string model = modelbox.Text;
            tempflag = IsValid(model, -1);
            Error(modelbox, null, tempflag, "Required Field", 0);
            if (!tempflag) flag = false;

            // colour
            string colour = colourbox.Text;
            tempflag = IsValid(colour, -1);
            Error(colourbox, null, tempflag, "Required Field", 0);
            if (!tempflag) flag = false;

            // kms that car traveled
            int kms = ParseInt(kmsbox.Text);
            if (kms < 0)
            {
                ep.SetError(kmsbox, "Please enter valid positive number.");
                flag = false;
            }
            else ep.SetError(kmsbox, "");

            // wholesale price
            int wprice = ParseInt(pricebox.Text);
            if (wprice < 0)
            {
                ep.SetError(pricebox, "Please enter valid positive number.");
                flag = false;
            }
            else ep.SetError(pricebox, "");

            // instock
            string instock = instockbox.Text;
            if (instock == "")
            {
                ep.SetError(instockbox, "Required Field");
                flag = false;
            }
            else
                ep.SetError(instockbox, "");

            // branch
            string branch = branchbox.Text;
            if (branch == "")
            {
                ep.SetError(branchbox, "Please select a branch");
                flag = false;
            }
            else
                ep.SetError(branchbox, "");

            // validate integer textboxes
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


            // insert into vehicle once validated all the fields
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

        /* 
        *  Name:           BranchId()
        *  Parameters:     string branch - branch string from dropdown
        *  Return value:   int branchid - the dealerid
        *  Description:    This method is invoked to get the branch id of a particular
        *                  branch.
        */
        static int BranchId(string branch)
        {
            int branchid = 0;

            if (branch == "Sportsworld") branchid = 1001;
            else if (branch == "Guelph Auto Mall") branchid = 1002;
            else branchid = 1003;

            return branchid;
        }

        /* 
        *  Name:           IsValid()
        *  Parameters:     string box - value from textbox
        *                  length - length of string
        *  Return value:   bool - true if valid otherwise false
        *  Description:    This method is invoked to validate value from
        *                  textboxes.
        */
        static bool IsValid(string box, int length)
        {
            bool flag = true;

            if (box.Length == 0)
                flag = false;

            // if length is 11, its VIN
            if (length == 11 && box.Length != 11)
                flag = false;

            return flag;
        }

        /* 
        *  Name:           AddVehicle_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method is invoked when add vehice is clicked on the home screen.
        *                  It gets the add vehicle page in the front.
        */
        private void AddVehicle_Click(object sender, EventArgs e)
        {
            PanelAddVehicle.BringToFront();
        }


        /* 
        *  Name:           AddCustomer_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method is invoked when add customer is clicked on the home screen.
        *                  It gets the add customer page to front.
        */
        private void AddCustomer_Click(object sender, EventArgs e)
        {
            PanelAddCustomer.BringToFront();
            firstnamebox.Focus();
        }

        /* 
        *  Name:           AddCustomer_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method is invoked to validate the customer fields. 
        *                  Once validated, they are added to database.
        */
        private void AddCustomerDB_Click(object sender, EventArgs e)
        {
            bool flag = true;
            bool tempflag = false;

            // firstname
            string firstname = firstnamebox.Text;
            tempflag = IsValid(firstname, 0);
            if (!tempflag) flag = false;
            Error(firstnamebox, null, tempflag, "Required Field", 0);

            // lastname
            string lastname = lastnamebox.Text;
            tempflag = IsValid(lastname, 0);
            if (!tempflag) flag = false;
            Error(lastnamebox, null, tempflag, "Required Field", 0);

            // phone
            string phone = phonebox.Text;
            if (phone.Length != 12)
            {
                tempflag = false;
                flag = false;
            }
            else
                tempflag = true;
            Error(phonebox, null, tempflag, "Invalid entry. Supported Format (XXX-XXX-XXXX).", 0);

            // once validated, check if already exist
            if (flag)
            {
                string query = "SELECT customerid FROM customer where phone='" + phone + "';";
                string check = GetDB(query, "customerid", false);

                if (check != "")
                {
                    flag = false;
                    MessageBox.Show("A customer with this phone already exist.", "Error");
                }
            }

            // add if not exist
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

            // go back to home
            if (flag)
            {
                PanelAddCustomer.SendToBack();
                ClearAllTextBoxes();
            }
        }

        /* 
       *  Name:           PlaceOrder_Click()
       *  Parameters:     object, EventArgs
       *  Return value:   null
       *  Description:    This method is invoked when place order is click on home. 
       *                  Brings the placeorder page in front.
       */
        private void PlaceOrder_Click(object sender, EventArgs e)
        {
            PanelPlaceOrder.BringToFront();
        }

        /* 
        *  Name:           FinalOrder_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method is invoked to validate the placeorder fields
        *                  and add to database once validated.
        */
        private void FinalOrder_Click(object sender, EventArgs e)
        {

            bool done = false;

            bool flag = true;
            bool tempflag = true;
            bool exist = false;
            string query = "";
            string check = "";

            // phone
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
            if (tradein < -1)
            {
                flag = false;
                tempflag = false;
            }
            Error(tradeinbox, null, tempflag, "Please enter valid positive number.", 0);


            valid = 0;
            // order status
            string orderstatus = orderstatusbox.Text;
            if (orderstatus == "")
            {
                valid = -1;
                flag = false;
            }
            Error(null, orderstatusbox, false, "Required Field", valid);


            // customer exist
            if (flag)
            {
                query = "SELECT customerid FROM customer where phone='" + phone + "';";
                check = GetDB(query, "customerid", false);
                if (check != "")
                {
                    exist = true;
                }

                // vehicle exist
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

                // check the status and update
                if (done)
                {
                    string status = "";

                    // check whether customer has paid or put on 'Hold'
                    if (orderstatus.ToLower() == "hold")
                    {
                        status = "'HOLD'";
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
                        //  status = status.ToLower();
                        if (status.ToLower().Contains("paid"))
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
                            //  MessageBox.Show("Please enter the details of tradein car.");
                            //  pricebox.Text = tradein.ToString();
                            //  PanelAddVehicle.BringToFront();
                        }
                        // ------------------------

                        // get customer name
                        query = "select concat(firstname, ' ', lastname) as 'Customer' from customer" +
                            " where phone='" + phone + "';";
                        check = GetDB(query, "Customer", false);
                        string cus = check;

                        query = "SELECT kms from vehicle where vin='" + vin + "';";
                        check = GetDB(query, "kms", false);
                        string kms = check;

                        // salutation
                        string x1 = "Thank you for choosing Wally's World of Wheels at " + dealer +
                            " for quality used vehicle\n\n";

                        // date, customer, orderid, orderstatus
                        string date1 = "Date: " + date + "\n";
                        string x2 = "Customer: " + cus + "\n";
                        string x3 = "Order ID: " + orderid + " - " + orderstatus + "\n\n";

                        // vehicle details
                        query = "select concat(v_year, ' ', make, ' ', model, ', ', colour) as vehicles" +
                            " from vehicle where vin='" + vin + "';";
                        check = GetDB(query, "vehicles", false);

                        // vehicle name
                        string x4 = check + "\n";
                        string x5 = "VIN: " + vin.ToUpper() + " " + "KMS: " + kms + "\n\n";

                        // purchase price
                        query = "select wprice from vehicle where vin='" + vin + "';";
                        int wprice = ParseInt(GetDB(query, "wprice", false));
                        string x6 = "Purchase Price: $" + wprice + "\n\n";

                        // trade in
                        string x7 = "Trade In: $" + tradein + "\n\n";

                        // sub total
                        int subtotal = wprice - tradein;
                        string x8 = "Subtotal = $" + subtotal + "\n";

                        // HST
                        double HST = subtotal * 0.13;
                        string x9 = "HST (13%) = $" + HST + "\n";
                        string x10 = "Sale Total = $" + (subtotal + HST).ToString() + "\n";

                        SalesOrderBox.Clear();
                        SalesOrderBox.Text = x1 + date1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10;
                        PanelSalesOrder.BringToFront();




                        //-----------------------
                        PanelPlaceOrder.SendToBack();
                    }

                }
            }

        }

        /* 
        *  Name:           ValidateVIN()
        *  Parameters:     string vin - VIN of vehicle
        *  Return value:   bool
        *  Description:    This method validated the VIN value entered.
        */
        static bool ValidateVIN(string vin)
        {
            bool flag = true;
            // check length
            if (vin.Length != 11)
                flag = false;

            // check characters
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


        /* 
        *  Name:           AddCustomerFromOrder_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method is invoked to add customer from order page.
        *                  Bring add customer page on front.
        */
        private void AddCustomerFromOrder_Click(object sender, EventArgs e)
        {
            PanelAddCustomer.BringToFront();
            firstnamebox.Focus();
        }


        /* 
        *  Name:           ValidatePhone()
        *  Parameters:     object, EventArgs
        *  Return value:   bool
        *  Description:    This method is invoked to validate phone number.
        */
        static bool ValidatePhone(string phone)
        {
            bool flag = true;

            // check length
            if (phone.Length != 12)
                flag = false;

            // check for characters
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

        /* 
        *  Name:           HomeButton()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    Brings home page infront. Clear all textboxes.
        */
        private void HomeButton(object sender, EventArgs e)
        {
            PanelHome.BringToFront();
            ClearAllTextBoxes();
        }

        /* 
        *  Name:           ClearAllTextBoxes()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method clears all the textboxes of all the pages.
        */
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

            // clear inventory listbox
            InvList.Items.Clear();

            // clear modify order textboxes
            Orderidbox.Clear();
            RetrievedDetails.Clear();

            OrderDetails.Clear();

        }

        /* 
        *  Name:           OrderHistory_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method gets the orders from database and display
        *                  in the textbox.
        */
        private void OrderHistory_Click(object sender, EventArgs e)
        {
            // bring orderhistory panel in front
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

            // get details of all orders and update order history
            for (int i = min; i <= max; i++)
            {
                count++;

                // query to get the data from database
                query = "SELECT CONCAT(firstname, ' ', lastname, ' - ',  orderdate, ' - ', orderstatus, ' - ', " +
                    "make, ' ', model, ' -$', sprice) as Details " +
                    "FROM orderline " +
                    "INNER JOIN customer on orderline.customerid=customer.customerid " +
                    "INNER JOIN orders on orderline.orderid=orders.orderid " +
                    "INNER JOIN vehicle on orderline.vin=vehicle.vin "
                    + "where orders.orderid=" + i + ";";

                check = GetDB(query, "Details", false);

                OrderDetails.Text = OrderDetails.Text + count + ". " + check + "\n";

            }
        }

        /* 
        *  Name:           OrderHistory_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method gets the orders from database and display
        *                  in the textbox.
        */
        private void ModifyOrder_Click(object sender, EventArgs e)
        {
            PanelModifyOrder.BringToFront();
        }

        /* 
        *  Name:           SearchCustomer_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This search orderdetails. Get details if found or show error.
        */
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

        /* 
        *  Name:           ChangeStatus_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method gets the value from textbox and changes the status.
        */
        private void ChangeStatus_Click(object sender, EventArgs e)
        {
            bool Changeable = false;
            bool cancel = false;
            bool paid = false;
            bool valid = true;

            string temp = RetrievedDetails.Text;
            if (temp.Length < 2)
            {
                ep.SetError(Orderidbox, "Please enter valid order id.");
                return;
            }
            else
                ep.SetError(Orderidbox, "");



            temp = temp.Substring(0, 5);
           // MessageBox.Show("--" + temp + "--");

            // status is not already paid
            if (temp.ToLower() != " paid" && !temp.ToLower().Contains("cncl"))
            {
                ep.SetError(RetrievedDetails, "");
                Changeable = true;

            }
            else
                ep.SetError(RetrievedDetails, "Status can't be altered if already PAID or CANCELLED!");

            // change status
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
                    MessageBox.Show("PAIDDDDDDDDD");
                    paid = true;
                }

            }

            string query = "";
            string check = "";
            string vin = "";
            string phone = "";
            int branchid = 0;
            int orderid = 0;

            // verify orderid
            check = Orderidbox.Text;
            orderid = ParseInt(check);

            // check if order id changed after detail appeared in text box
            if (orderid != -1)
            {
                query = "SELECT orderdate from orders where orderid =" + orderid + ";";
                check = GetDB(query, "orderdate", false);

                if (check == "")
                {
                    ep.SetError(Orderidbox, "Invalid Orderid");
                    valid = false;
                }
                else
                    ep.SetError(Orderidbox, "");
            }

            // get vin
            query = "SELECT vin FROM orderline where orderid=" + orderid + ";";
            check = GetDB(query, "vin", false);
            vin = check;
            vin = vin.Substring(1);
            // MessageBox.Show("--" + vin + "--");

            // get phone number of customer
            query = "SELECT phone FROM orderline " +
                "INNER JOIN customer on orderline.customerid=customer.customerid " +
                "where orderid=" + orderid + ";";
            phone = GetDB(query, "phone", false);
            phone = phone.Substring(1);
            //MessageBox.Show("--" + phone + "--");

            // get branch id
            query = "SELECT dealerid FROM orderline " +
                 "where orderid=" + orderid + ";";
            branchid = ParseInt(GetDB(query, "dealerid", false));
            //  MessageBox.Show("--" + branchid.ToString() + "--");



            // cancel it
            if (cancel && valid)
            {
                temp = Orderidbox.Text;
                orderid = ParseInt(temp);

                query = "SELECT instock FROM orderline " +
                    "INNER JOIN orders on orders.orderid=orderline.orderid " +
                    "INNER JOIN vehicle on vehicle.vin=orderline.vin " +
                    "where orders.orderid=" + orderid + ";";

                check = GetDB(query, "instock", false);
                if (check.ToLower().Contains("yes"))
                {
                    MessageBox.Show("The order is already cancelled.");
                }
                else
                {
                    InsertOrders(vin, phone, 0, branchid, "CNCL", true);
                    MessageBox.Show("Order Cancelled..");
                }
            }

            else
            if (paid && valid)
            {
                query = "SELECT instock FROM vehicle " +
                    "INNER JOIN orderline ON orderline.vin = vehicle.vin " +
                    "where orderid=" + orderid + ";";
                check = GetDB(query, "instock", false);

                if(check.ToLower().Contains("no"))
                {
                    MessageBox.Show("Order is paid already.");
                }
                else
                {
                    InsertOrders(vin, phone, 0, branchid, "PAID", false);
                    MessageBox.Show("Order Status changed from HOLD to PAID.");
                }

            }
        }



        /* 
        *  Name:           SearchInventory_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method searches the inventory levlel of a specific dealer.
        */
        private void SearchInventory_Click(object sender, EventArgs e)
        {
            string temp = DealershipSelect.Text;
            int selected = 0;
            int branchid = 0;
            string query = "";
            //string check = "";

            if (temp == "")
            {
                selected = -1;
            }

            Error(null, DealershipSelect, false, "Required field", selected);

            // if dealer selected
            if (selected != -1)
            {
                ClearAllTextBoxes();
                branchid = BranchId(temp);
                query = "SELECT concat(instock, ' --', vin, ': ', make, ' ', model) as Details" +
                    " from vehicle where dealerid=" + branchid + ";";
                // check = GetDB(query, "Details", false);

                //  MessageBox.Show(check);

                cmd.CommandText = query;
                MySqlDataReader reader;
                reader = cmd.ExecuteReader();
                string final = "";


                while (reader.Read())
                {
                    final = final + " " + reader["Details"] + "#";
                    //InvList.Items.Add(final);
                }
                reader.Close();

                try
                {
                    string[] items = final.Split('#');
                    int count = 0;
                    for (int i = 0; i < items.Length; i++)
                    {
                        InvList.Items.Add(items[i]);
                        count++;
                    }

                    // MessageBox.Show(count.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
            }
        }

        /* 
        *  Name:           InventoryLevel_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method brings the inventory level page infront.
        */
        private void InventoryLevel_Click(object sender, EventArgs e)
        {
            PanelInventory.BringToFront();
        }


        private void InsertOrders(string vin, string phone, int tradein, int branchid, string orderstatus, bool cncl)
        {
            string status = "";
            string query = "";
            string check = "";


            // check whether customer has paid or put on 'Hold'
            if (orderstatus.ToLower() == "hold")
            {
                status = "'HOLD'";
            }
            else
            if (orderstatus.ToLower().Contains("cncl"))
            {
                status = "'CNCL'";
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

            DialogResult Confirmation = MessageBox.Show("Are you sure to proceed?", "Confirm", MessageBoxButtons.YesNo);

            // Update instock flag
            if (Confirmation == DialogResult.Yes)
            {
                string stock = "";

                //  status = status.ToLower();
                if (status.ToLower().Contains("paid"))
                    stock = "'No'";
                else
                    if (status.ToLower().Contains("cncl"))
                    stock = "'Yes'";
                else
                    stock = status.ToUpper();

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


                string date = DateTime.Now.ToString("yyyy-MM-dd");
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


                // change sale price to 0 if order is cancelled
                if (cncl)
                    sprice = 0;

                // insert into orderline
                query = "INSERT INTO OrderLine (OrderLineID, OrderID, CustomerID, DealerID, VIN, TradeIn, sprice) " +
                    "VALUES " +
                    "(" + orderlineid.ToString() + ", " + orderid.ToString() + ", " + customerid.ToString() + ", "
                    + branchid.ToString() + ", '" + vin + "', " + tradein.ToString() + ", " + sprice.ToString() + ")";

                // execute orderline query
                GetDB(query, "", true);

            }
        }

        /* 
        *  Name:           ExitWally_Click()
        *  Parameters:     object, EventArgs
        *  Return value:   null
        *  Description:    This method exits the closes the connection and exits program.
        */
        private void ExitWally_Click(object sender, EventArgs e)
        {
            DialogResult Confirmation = MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButtons.YesNo);
            if (Confirmation == DialogResult.Yes)
            {
                this.Close();
                cn.Close();
            }
        }
    }


}
