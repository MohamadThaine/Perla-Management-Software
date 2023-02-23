using Perla.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Perla.Tools
{
    /// <summary>
    /// Interaction logic for Calender.xaml
    /// </summary>
    public partial class Calender : Window
    {
        public List<CustomerAppoitments> dateAppoitments { get; set; }
        public Calender()
        {
            dateAppoitments = PrepareData.CustomerAppoitmentsList.Where(app => app.Appoitment.Appointment_Data.Date == DateTime.Today.Date)
                                                             .ToList();
            InitializeComponent();
        }

        private void AddAppoitment_Click(object sender, RoutedEventArgs e)
        {
            AddAppointment addAppointmentWindow;
            if (AppoitmentCalender.SelectedDate == null)
                addAppointmentWindow = new(DateTime.Today.Date, null);
            else
                addAppointmentWindow = new(AppoitmentCalender.SelectedDate.Value, null);
            addAppointmentWindow.Show();
        }

        private void AppoitmentCalender_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            dateAppoitments = PrepareData.CustomerAppoitmentsList.Where(app => app.Appoitment.Appointment_Data.Date == AppoitmentCalender.SelectedDate.Value)
                                                             .ToList();
            AppoitmentsDataGrid.ItemsSource = dateAppoitments;
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
    }
}
