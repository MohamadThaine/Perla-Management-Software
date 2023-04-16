using Perla.classes;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Perla.Tools
{
    /// <summary>
    /// Interaction logic for AddAppointment.xaml
    /// </summary>
    public partial class AddAppointment : Window
    {
        public Customer AppointmentCustomer { get; set; }
        public bool ExistingCustomer { get; set; }
        public AddAppointment(DateTime AppoitmentDate, Customer customer)
        {
            InitializeComponent();
            if (AppoitmentDate != null)
            {
                AppointmentDate.SelectedDate = AppoitmentDate;
            }
            if (customer != null)
            {
                AppointmentCustomer = customer;
                CustomerID.Text = AppointmentCustomer.ID.ToString();
                Name.Text = AppointmentCustomer.Name.ToString();
                PhoneNum.Text = AppointmentCustomer.PhoneNumber.ToString();
                Name.IsReadOnly = true;
                PhoneNum.IsReadOnly = true;
                ExistingCustomer = true;
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)

        {
            Expander expander = sender as Expander;
            expander.Height = 190;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            expander.Height = 45;
        }

        private void CustomerIDOutOfFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                AppointmentCustomer = PrepareData.customerList.Single(cust => cust.ID == CustomerID.Text);
                ExistingCustomer = true;
                Name.Text = AppointmentCustomer.Name;
                PhoneNum.Text = AppointmentCustomer.PhoneNumber.ToString();
                Name.IsReadOnly = true;
                PhoneNum.IsReadOnly = true;
            }
            catch (Exception Error)
            {
                ExistingCustomer = false;
                Name.Text = "";
                PhoneNum.Text = "";
                Name.IsReadOnly = false;
                PhoneNum.IsReadOnly = false;
            }
        }

        private void AddAppoitmentToDB(object sender, RoutedEventArgs e)
        {
            if (!IsDataValid())
                return;
            if (!ExistingCustomer)
            {
                AddCustomerToDB();
            }
            string[] Values = new string[5];
            Values[0] = "0";
            Values[1] = AppointmentDate.SelectedDate.Value.ToString("yyyy-MM-dd") + " "
                + Convert.ToDateTime(AppointmentTime.SelectedDateTime.ToString()).ToString("HH:mm:ss");
            Values[2] = CustomerID.Text;
            Values[3] = Treatment.Text;
            Values[4] = "0";
            if (AppoitmentAlreadyExistInSameTime(Convert.ToDateTime(Values[1])))
            {
                return;
            }
            try
            {
                Appoitment appoitment = new Appoitment(0, Convert.ToDateTime(Values[1]), Values[2], Values[3], 0);
                if (!ExistingCustomer)
                {
                    long AddedID = DBManager.InsertToDB("appointment", Values);
                    appoitment.ID = Convert.ToInt32(AddedID);
                    CustomerAppoitments customerAppoitments = new CustomerAppoitments(AppointmentCustomer, appoitment);
                    PrepareData.CustomerAppoitmentsList.Add(customerAppoitments);
                    if (appoitment.Appointment_Data.Date == DateTime.Today.Date)
                       PrepareData.todayCustomerAppoiments.Add(customerAppoitments);
                    return;
                }
                if (AddCustomerToApp(appoitment))
                {
                    long AddedID = DBManager.InsertToDB("appointment", Values);
                    appoitment.ID = Convert.ToInt32(AddedID);
                    CustomerAppoitments customerAppoitments = new CustomerAppoitments(AppointmentCustomer, appoitment);
                    PrepareData.CustomerAppoitmentsList.Add(customerAppoitments);
                    if (appoitment.Appointment_Data.Date == DateTime.Today.Date)
                        PrepareData.todayCustomerAppoiments.Add(customerAppoitments);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException Error)
            {
                MessageBox.Show(Error.Message);
            }
        }

        private bool AppoitmentAlreadyExistInSameTime(DateTime AppDate)
        {
            int result = DBManager.GetAppoitmentID("0", AppDate);
            if (result == 0)
            {
                return false;
            }
            MessageBox.Show("هناك جلسه في نفس هذا الوقت واليوم!");
            return true;
        }

        private void AddCustomerToDB()
        {
            string[] Values = new string[4];
            Values[0] = (string.IsNullOrEmpty(CustomerID.Text)) ? "0" : CustomerID.Text;
            Values[1] = Name.Text;
            Values[2] = PhoneNum.Text;
            Values[3] = "0";
            try
            {
                string ID = DBManager.InsertToDB("customer", Values).ToString();
                if (ID.Length == 8) ID = "0" + ID;
                PrepareData.customerList.Add(new Customer(ID, Values[1], Values[2], 0));
                AppointmentCustomer = new Customer(ID, Values[1], Values[2], 0);
                CustomerID.Text = ID;
                ExistingCustomer = true;
            }
            catch (MySql.Data.MySqlClient.MySqlException Error)
            {
                if (Error.Message.Contains("Duplicate entry"))
                    MessageBox.Show("هذا الزبون موجود مسبقا");
                else
                    MessageBox.Show(Error.Message);
            }

        }

        private bool AddCustomerToApp(Appoitment appoitment)
        {
            int CheckAppoitment = 0;
            try
            {
                CheckAppoitment = PrepareData.CustomerAppoitmentsList.Where(cust => cust.Customer.ID == AppointmentCustomer.ID)
                                                                     .Count(app => app.Appoitment.Appointment_Data.Date == appoitment.Appointment_Data.Date);
            }catch(System.NullReferenceException Error)
            {
                return true;
            }
            
            if (CheckAppoitment > 0)
            {
                MessageBoxResult Result = MessageBox.Show("يوجد لدى هذا الزبون موعد بهذا اليوم هل تريد اضافه الموعد جديد مكان القديم", "خطا",
                    MessageBoxButton.YesNo);
                if (Result == MessageBoxResult.Yes)
                {
                    int OldAppID = DBManager.GetAppoitmentID(AppointmentCustomer.ID, appoitment.Appointment_Data);
                    CustomerAppoitments customerAppoitments = PrepareData.CustomerAppoitmentsList
                                                              .Where(app => app.Appoitment.Appointment_Data.Date == appoitment.Appointment_Data.Date)
                                                              .Single(cust => cust.Customer.ID == AppointmentCustomer.ID);
                    DBManager.DeleteRow("appointment", "ID", OldAppID.ToString());
                    PrepareData.CustomerAppoitmentsList.Remove(customerAppoitments);
                    if (appoitment.Appointment_Data.Date == DateTime.Today.Date)
                    {
                        PrepareData.todayCustomerAppoiments.Remove(customerAppoitments);
                    }
                }
                else
                    return false;
            }
            return true;
        }

        private bool IsDataValid()
        {
            if (Name.Text == "" || PhoneNum.Text == "" || Treatment.Text == "" || AppointmentDate.SelectedDate.Value == null
                || AppointmentTime.SelectedDateTime == null)
            {
                MessageBox.Show("لا تترك اي حقل مطلوب فارغ");
                return false;
            }
            return true;
        }

        private void NumbersOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
