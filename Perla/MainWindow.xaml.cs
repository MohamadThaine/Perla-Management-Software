using Perla.classes;
using Perla.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Perla
{
    public partial class MainWindow : Window
    {
        public CustomerProfile? profile { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            var customer = row.Item as Customer;
        }

        private void OpenAddAppoitmentWindow(object sender, RoutedEventArgs e)
        {
            AddAppointment newWindow = new(DateTime.Today, null);
            newWindow.Closed += NewWindow_Closed;
            newWindow.Show();
        }

        private void NewWindow_Closed(object? sender, EventArgs e)
        {
            CustomersDataGrid.ItemsSource = PrepareData.todayCustomerAppoiments;
        }

        private void ShowCalender(object sender, RoutedEventArgs e)
        {
            Tools.Calender calendar = new();
            calendar.Show();
        }

        private void OpenAddCustomerWindow(object sender, RoutedEventArgs e)
        {
            Tools.AddCustomer addCustomer = new();
            addCustomer.Show();
        }

        private void SearchClick(object sender, MouseButtonEventArgs e)
        {
            if (profile != null)
            {
                profile.Close();
                profile = null;
            }
            ListViewItem item = sender as ListViewItem;
            if (item != null)
            {
                Customer customer = item.Content as Customer;
                profile = new CustomerProfile(customer);
                try
                {
                    profile.Show();

                }
                catch (System.InvalidOperationException Error) { return; };
            }
        }

        private void OpenAddSpending(object sender, RoutedEventArgs e)
        {
            AddSpendning SpendingWindow = new AddSpendning();
            SpendingWindow.Show();
        }

        private void CancelAppoitment(object sender, RoutedEventArgs e)
        {
            MessageBoxResult ButtonClicked = MessageBox.Show("هل انت متاكد من الغاء الموعد!", "تاكيد", MessageBoxButton.YesNo);
            if (ButtonClicked != MessageBoxResult.Yes)
            {
                return;
            }
            CustomerAppoitments customerAppoitments = ((FrameworkElement)sender).DataContext as CustomerAppoitments;
            if (customerAppoitments.Appoitment.Appointment_Data.Date < DateTime.Today.Date)
            {
                MessageBox.Show("لا يمكنك الغاء المواعيد السابقه");
                return;
            }
            DBManager.DeleteRow("appointment", "ID", customerAppoitments.Appoitment.ID.ToString());
            PrepareData.todayCustomerAppoiments.Remove(customerAppoitments);
            PrepareData.CustomerAppoitmentsList.Remove(customerAppoitments);
            PrepareData.appoitmentList.Remove(customerAppoitments.Appoitment);
        }

        private void ChangeAppoitmentDate(object sender, RoutedEventArgs e)
        {
            CustomerAppoitments customerAppoitments = ((FrameworkElement)sender).DataContext as CustomerAppoitments;
            ChangeAppoitmentDate changeAppoitmentDate = new ChangeAppoitmentDate(customerAppoitments);
            changeAppoitmentDate.Show();
        }

        private void Search(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text.Length == 0)
            {
                CustomerSearchView.ItemsSource = PrepareData.customerList;
                return;
            }
            List<Customer> FiltredCustomers = PrepareData.customerList.Where(cust => cust.Name.StartsWith(SearchTextBox.Text)
                                                                            || cust.ID.ToString().StartsWith(SearchTextBox.Text)).ToList();
            CustomerSearchView.ItemsSource = FiltredCustomers;
        }

        private void AddPayment(object sender, RoutedEventArgs e)
        {
            CustomerAppoitments customerAppoitments = ((FrameworkElement)sender).DataContext as CustomerAppoitments;
            string MoneyPaid = Microsoft.VisualBasic.Interaction.InputBox("اضافه المبلغ", customerAppoitments.Customer.Name, "0");
            if (MoneyPaid != "0" && MoneyPaid != "")
            {
                try
                {
                    customerAppoitments.Customer.MoneyPaid += Convert.ToDouble(MoneyPaid);
                    customerAppoitments.Appoitment.MoneyPaid = Convert.ToDouble(MoneyPaid);
                    DBManager.AddPayment(customerAppoitments.Customer.ID, customerAppoitments.Appoitment.ID, Convert.ToDouble(MoneyPaid));
                    MessageBoxResult AddNewAppoitment = MessageBox.Show("اضافه موعد اخر لهذا الزبون؟", "اضافه موعد اخر", MessageBoxButton.YesNo);
                    if (AddNewAppoitment == MessageBoxResult.Yes)
                    {
                        AddAppointment addAppointment = new AddAppointment(DateTime.Today.Date, customerAppoitments.Customer);
                        addAppointment.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حدث خطا يرجى الاضافه مره اخرى");
                    return;
                }
            }

        }

        private void OpenAccounting(object sender, RoutedEventArgs e)
        {
            Accounting AccountingWindow = new Accounting();
            AccountingWindow.ShowDialog();
        }
    }
}
