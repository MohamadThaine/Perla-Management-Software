using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Perla.classes
{
    internal class PrepareData
    {
        static public ObservableCollection<Customer> customerList { get; set; }
        static public ObservableCollection<Appoitment> appoitmentList { get; set; }
        static public ObservableCollection<CustomerAppoitments> CustomerAppoitmentsList { get; set; }
        static public ObservableCollection<CustomerAppoitments> todayCustomerAppoiments { get; set; }
        static public ObservableCollection<TimeOnly> timeList { get; set; } = new ObservableCollection<TimeOnly>();
        public PrepareData()
        {
            customerList = new ObservableCollection<Customer>();
            appoitmentList = new ObservableCollection<Appoitment>();
            CustomerAppoitmentsList = new ObservableCollection<CustomerAppoitments>();
            todayCustomerAppoiments = new ObservableCollection<CustomerAppoitments>();
            DispatcherTimer RefreshDataTimer = new DispatcherTimer();
            RefreshDataTimer.Interval = TimeSpan.FromMinutes(10);
            RefreshDataTimer.Tick += RefreshData;
            RefreshData(null , null);//Work every 10 Min
            RefreshDataTimer.Start();
        }

        private void RefreshData(object? sender, EventArgs e)
        {
            try
            {
                customerList.Clear();
                foreach(Customer customer in DBManager.GetDataFromDB<Customer>("customer", "*", null, "0"))
                {
                    if (customer.ID.Length == 8)
                        customer.ID = "0" + customer.ID;
                     customerList.Add(customer);
                    
                }
                appoitmentList.Clear();
                foreach(Appoitment appoitment in DBManager.GetDataFromDB<Appoitment>("appointment", "*", null, "0"))
                {
                    if (appoitment.Customer_ID.Length == 8)
                        appoitment.Customer_ID = "0" + appoitment.Customer_ID;
                    appoitmentList.Add(appoitment);
                }
                List<CustomerAppoitments> customerAppoitments =
                            (from cust in customerList
                             join app in appoitmentList
                             on cust.ID equals app.Customer_ID
                             select new CustomerAppoitments
                             {
                                 Customer = cust,
                                 Appoitment = app
                             }).ToList();
                CustomerAppoitmentsList.Clear();
                foreach (CustomerAppoitments customerAppoitments1 in customerAppoitments)
                {
                    CustomerAppoitmentsList.Add(customerAppoitments1);
                }
                DateTime todayData = DateTime.Today.Date;
                List<CustomerAppoitments> TodayApp = CustomerAppoitmentsList.Where(app => app.Appoitment.Appointment_Data.Date == todayData).ToList();
                foreach(CustomerAppoitments customerAppoitments2 in TodayApp)
                {
                    todayCustomerAppoiments.Add(customerAppoitments2);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                DBManager.server = Microsoft.VisualBasic.Interaction.InputBox("Add radmin server ip", "cant connetect to db", "");
                if (DBManager.server == null)
                    return;
                File.WriteAllText(@"C:\\Program Files\\Perla\\Server.txt", DBManager.server);
                RefreshData(null, null);

            }
            
        }
    }
}
