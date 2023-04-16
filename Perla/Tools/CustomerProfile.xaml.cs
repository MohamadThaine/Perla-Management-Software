using Perla.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Perla.Tools
{
    /// <summary>
    /// Interaction logic for CustomerProfile.xaml
    /// </summary>
    public partial class CustomerProfile : Window
    {
        public Customer Customer { get; set; }
        private Customer OldCustomerData { get; set; }
        private bool EditDBClicked { get; set; } = false;
        public List<CustomerAppoitments> CustomerAppoitmentsList { get; set; }
        public string LastAppoitmentDate { get; set; }
        public CustomerAppoitments NextAppoitment { get; set; }
        public CustomerProfile(Customer customer)
        {
            if (customer == null)
            {
                this.Close();
                return;
            }
            Customer = customer;
            OldCustomerData = new Customer(customer);
            CustomerAppoitmentsList = PrepareData.CustomerAppoitmentsList.Where(cust => cust.Customer.ID == customer.ID).ToList();
            GetCustomerAppoitments();
            InitializeComponent();
        }

        private void GetCustomerAppoitments()
        {
            if (CustomerAppoitmentsList.Count < 0) return;
            try
            {
                NextAppoitment = CustomerAppoitmentsList.Where(app => app.Appoitment.Appointment_Data > DateTime.Now)
                                                        .OrderBy(app => app.Appoitment.Appointment_Data).ToArray()[0];
            }
            catch (System.IndexOutOfRangeException Error) { }
            if (CustomerAppoitmentsList.Count < 0) return;
            try
            {
                CustomerAppoitmentsList = CustomerAppoitmentsList.Where(app => app.Appoitment.Appointment_Data < DateTime.Now).ToList();
                LastAppoitmentDate = CustomerAppoitmentsList.Last().Appoitment.Appointment_Data.ToString("dd/MM/yyyy");
            }
            catch (System.InvalidOperationException Error)
            {
                LastAppoitmentDate = "لا يوجد";
            }

        }

        private void CancelNextAppoitment(object sender, RoutedEventArgs e)
        {
            if (NextAppoitment == null) return;
            MessageBoxResult ButtonClicked = MessageBox.Show("هل انت متاكد من الغاء الموعد!", "تاكيد", MessageBoxButton.YesNo);
            if (ButtonClicked != MessageBoxResult.Yes)
            {
                return;
            }
            DBManager.DeleteRow("appointment", "ID", NextAppoitment.Appoitment.ID.ToString());
            PrepareData.todayCustomerAppoiments.Remove(NextAppoitment);
            PrepareData.CustomerAppoitmentsList.Remove(NextAppoitment);
            PrepareData.appoitmentList.Remove(NextAppoitment.Appoitment);
        }

        private void ChangeAppoitmentDate(object sender, RoutedEventArgs e)
        {
            if (NextAppoitment == null) return;
            ChangeAppoitmentDate changeAppoitmentDate = new ChangeAppoitmentDate(NextAppoitment);
            changeAppoitmentDate.Show();
        }

        private void EditCustomerData(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(CustomerID.Text) || string.IsNullOrEmpty(CustomerName.Text) || string.IsNullOrEmpty(CustomerPhoneNum.Text))
            {
                MessageBox.Show("لا تترك اي حقل فارغ");
                GetOldCustomerData();
                return;
            }
            EditDBClicked = true;
            string[] Values = new string[4] { Customer.ID.ToString(), Customer.Name, Customer.PhoneNumber.ToString(), Customer.MoneyPaid.ToString() };
            try
            {
                DBManager.UpdateCustomerData(OldCustomerData.ID, Values);
            }
            catch (MySql.Data.MySqlClient.MySqlException Error)
            {
                MessageBox.Show("رقم الهويه هذا موجود مسبقا", Error.Message);
                GetOldCustomerData();
            }

            OldCustomerData = new Customer(Customer);
        }

        private void GetOldCustomerData()
        {
            Customer.ID = OldCustomerData.ID;
            Customer.Name = OldCustomerData.Name;
            Customer.PhoneNumber = OldCustomerData.PhoneNumber;
            Customer.MoneyPaid = OldCustomerData.MoneyPaid;
            CustomerID.Text = Customer.ID.ToString();
            CustomerName.Text = Customer.Name;
            CustomerPhoneNum.Text = Customer.PhoneNumber.ToString();
            CustomerPaidMoney.Text = Customer.MoneyPaid.ToString();
        }

        private void Profile_Closed(object sender, EventArgs e)
        {
            if (EditDBClicked != true)
            {
                Customer.ID = OldCustomerData.ID;
                Customer.Name = OldCustomerData.Name;
                Customer.PhoneNumber = OldCustomerData.PhoneNumber;
                CustomerPaidMoney.Text = Customer.MoneyPaid.ToString();
            }
        }

        private void AddAppoitment(object sender, RoutedEventArgs e)
        {
            AddAppointment addAppointment = new(DateTime.Today.Date, Customer);
            addAppointment.Show();
        }

        private void NumbersOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        
    }
}
