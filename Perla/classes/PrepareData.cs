using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            RefreshData();//Work every 24HR
            timeList.Add(new TimeOnly(10, 20));
            timeList.Add(new TimeOnly(11, 30));
            timeList.Add(new TimeOnly(12, 40));
            timeList.Add(new TimeOnly(13, 50));
        }
        public void RefreshData()
        {
            customerList = DBManager.GetDataFromDB<Customer>("customer", "*", null, "0");
            appoitmentList = DBManager.GetDataFromDB<Appoitment>("appointment", "*", null, "0");
            List<CustomerAppoitments> customerAppoitments =
                        (from cust in customerList
                         join app in appoitmentList
                         on cust.ID equals app.Customer_ID
                         select new CustomerAppoitments
                         {
                             Customer = cust,
                             Appoitment = app
                         }).ToList();
            CustomerAppoitmentsList = new ObservableCollection<CustomerAppoitments>(customerAppoitments);
            List<CustomerAppoitments> TodayApp = CustomerAppoitmentsList.Where(app => app.Appoitment.Appointment_Data.Date == DateTime.Today.Date).ToList();
            todayCustomerAppoiments = new ObservableCollection<CustomerAppoitments>(TodayApp);
        }
    }
}
