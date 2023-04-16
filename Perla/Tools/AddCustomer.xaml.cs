using Perla.classes;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Perla.Tools
{
    /// <summary>
    /// Interaction logic for AddCustomer.xaml
    /// </summary>
    public partial class AddCustomer : Window
    {
        public AddCustomer()
        {
            InitializeComponent();
        }

        private void AddCustomerToDB(object sender, RoutedEventArgs e)
        {
            if (!IsDataValid())
                return;
            string[] Values = new string[4];
            Values[0] = (string.IsNullOrEmpty(CustomerID.Text)) ? "0" : CustomerID.Text;
            Values[1] = Name.Text;
            Values[2] = PhoneNum.Text;
            Values[3] = (string.IsNullOrEmpty(PaidMoney.Text)) ? "0" : PaidMoney.Text;
            try
            {
                string ID = DBManager.InsertToDB("customer", Values).ToString();
                if (ID.Length == 8) ID = "0" + ID;
                PrepareData.customerList.Add(new Customer(ID, Values[1], Values[2],
                    Convert.ToDouble(Values[3])));
                CustomerID.Text = ID;
            }
            catch (MySql.Data.MySqlClient.MySqlException Error)
            {
                if (Error.Message.Contains("Duplicate entry"))
                    MessageBox.Show("هذا الزبون موجود مسبقا");
                else
                    MessageBox.Show(Error.Message);
            }

        }
        private bool IsDataValid()
        {
            if (Name.Text == "" || PhoneNum.Text == "")
            {
                MessageBox.Show("لا تترك اي حقل مطلوب فارغ");
                return false;
            }
            return true;
        }

        private void NumberOnlyTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
