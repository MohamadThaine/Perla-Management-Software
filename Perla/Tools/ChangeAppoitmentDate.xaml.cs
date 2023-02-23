using Perla.classes;
using System;
using System.Windows;

namespace Perla.Tools
{
    /// <summary>
    /// Interaction logic for DeleyAppoitment.xaml
    /// </summary>
    public partial class ChangeAppoitmentDate : Window
    {
        public CustomerAppoitments customerAppoitments { get; set; }
        public ChangeAppoitmentDate(CustomerAppoitments customerAppoitment)
        {
            customerAppoitments = customerAppoitment;
            InitializeComponent();
        }

        private void ChangeDate(object sender, RoutedEventArgs e)
        {
            if (AppoitmentDate.SelectedDate == null || AppoitmentTime.SelectedDateTime == null)
            {
                MessageBox.Show("لا تترك اي حقل فارغ!");
                return;
            }
            if (AppoitmentDate.SelectedDate < DateTime.Now.Date)
            {
                MessageBox.Show("لا يمكن ان يكون تاريخ قبل اليوم!");
                return;
            }
            if (AppoitmentTime.SelectedDateTime.Value.TimeOfDay < DateTime.Now.TimeOfDay && AppoitmentDate.SelectedDate == DateTime.Today.Date)
            {
                MessageBox.Show("لا يمكن ان يكون الوقت قبل الوقت الحالي!");
                return;
            }
            string NewDate = AppoitmentDate.SelectedDate.Value.ToString("yyyy-MM-dd") + " "
                + Convert.ToDateTime(AppoitmentTime.SelectedDateTime.ToString()).ToString("HH:mm:ss");
            if (NewDate == customerAppoitments.Appoitment.Appointment_Data.ToString("yyyy-MM-dd HH:mm:ss"))
            {
                MessageBox.Show("التاريخ الحالي يشابه التاريخ القديم!");
                return;
            }
            UpdateListsAndDB(NewDate);
        }
        private void UpdateListsAndDB(string NewDate)
        {
            DBManager.UpdateAppoitmentDate(customerAppoitments.Appoitment.ID, NewDate);
            PrepareData.appoitmentList.Remove(customerAppoitments.Appoitment);
            PrepareData.CustomerAppoitmentsList.Remove(customerAppoitments);
            if (Convert.ToDateTime(NewDate).Date == DateTime.Today.Date)
                PrepareData.todayCustomerAppoiments.Remove(customerAppoitments);
            customerAppoitments.Appoitment.Appointment_Data = Convert.ToDateTime(NewDate);
            PrepareData.appoitmentList.Add(customerAppoitments.Appoitment);
            PrepareData.CustomerAppoitmentsList.Add(customerAppoitments);
            if (Convert.ToDateTime(NewDate).Date == DateTime.Today.Date)
                PrepareData.todayCustomerAppoiments.Add(customerAppoitments);
        }
    }
}
