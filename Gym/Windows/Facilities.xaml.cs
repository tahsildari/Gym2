using Gym.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Gym.Domain.Enums;

namespace Gym.Windows
{
    /// <summary>
    /// Interaction logic for Facilities.xaml
    /// </summary>
    public partial class Facilities : Window
    {
        public Facilities()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        ObservableCollection<Data.Facility> FacilitiesList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();
        FacilityVM FacilityModel = new FacilityVM();

        Actions action = Actions.Inserting;
        bool IsNew() => action == Actions.Inserting;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            this.DataContext = FacilityModel;
        }

        private void RefreshGrid()
        {
            var facilities = db.Facilities.ToList();
            FacilitiesList = new ObservableCollection<Data.Facility>(facilities);

            DataGrid.DataContext = FacilitiesList;
        }

        private void MainDialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
                if (!string.IsNullOrEmpty(FacilityModel.Name))
                {
                    switch (action)
                    {
                        case Actions.Inserting:
                            if (!db.Facilities.Any(u => u.Name == FacilityModel.Name))
                            {
                                db.Facilities.InsertOnSubmit(
                                    new Data.Facility
                                    {
                                        Name = FacilityModel.Name,
                                        Price = FacilityModel.Price,
                                        Sessions = FacilityModel.Sessions
                                    });
                                db.SubmitChanges();

                                RefreshGrid();
                            }
                            else
                            {
                                DuplicateSnackBar.IsActive = true;


                                System.Threading.Thread t = new System.Threading.Thread(() =>
                                {
                                    var started = DateTime.Now;
                                    while (DateTime.Now.Subtract(started).TotalSeconds < 3)
                                    { }
                                    this.Dispatcher.Invoke(
                                    new Action(() => { DuplicateSnackBar.IsActive = false; }));
                                }); t.Start();
                            }
                            break;
                        case Actions.Editing:
                            var facilty = db.Facilities.Where(u => u.Name == FacilityModel.Name).FirstOrDefault();
                            facilty.Price = FacilityModel.Price;
                            facilty.Sessions = FacilityModel.Sessions;
                            db.SubmitChanges();
                            RefreshGrid();
                            break;
                        default:
                            break;
                    }
                }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            var facility = ((FrameworkElement)sender).DataContext as Data.Facility;
            FacilityModel.Name = facility.Name;
            FacilityModel.Price = facility.Price;
            action = Actions.Editing;
            NameTextBox.IsReadOnly = true;

            MainDialogHost.IsOpen = true;
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {

            this.DataContext = FacilityModel = new FacilityVM();
            action = Actions.Inserting;
            NameTextBox.IsReadOnly = false;
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateSnackBar.IsActive = false;// = TimeSpan.FromSeconds(3);
        }
    }
}
