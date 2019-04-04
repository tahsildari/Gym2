using Gym.Domain;
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
    /// Interaction logic for Sports.xaml
    /// </summary>
    public partial class Sports : Window
    {
        public Sports()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var escTime = (DateTime)(Dynamics.LastEscapeTime ?? DateTime.Now.AddDays(-1));
                if ((DateTime.Now - escTime) > TimeSpan.FromMilliseconds(100))
                    this.Close();
            }
        }

        ObservableCollection<Data.Sport> SportsList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();
        SportVM SportModel = new SportVM();

        Actions action = Actions.Inserting;
        bool IsNew() => action == Actions.Inserting;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            this.DataContext = SportModel;

        }

        private void RefreshGrid()
        {
            var sports = db.Sports.ToList();
            SportsList = new ObservableCollection<Data.Sport>(sports);

            DataGrid.DataContext = SportsList;
        }

        async private void Dialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
            {
                if (!string.IsNullOrEmpty(SportModel.Name))
                {
                    switch (action)
                    {
                        case Actions.Inserting:
                            if (!db.Sports.Any(s => s.Name == SportModel.Name))
                            {
                                db.Sports.InsertOnSubmit(
                                    new Data.Sport
                                    {
                                        Name = SportModel.Name
                                    });
                                db.SubmitChanges();

                                RefreshGrid();
                            }
                            else
                            {
                                Message.Content = "عنوان رشته ورزشی تکراری است، عنوان دیگری انتخاب کنید";
                                DuplicateUsernameSnackBar.IsActive = true;


                                System.Threading.Thread t = new System.Threading.Thread(() =>
                                {
                                    var started = DateTime.Now;
                                    while (DateTime.Now.Subtract(started).TotalSeconds < 3)
                                    { }
                                    this.Dispatcher.Invoke(
                                    new Action(() => { DuplicateUsernameSnackBar.IsActive = false; }));
                                }); t.Start();
                            }
                            break;
                        case Actions.Editing:
                            db.Sports.Where(s => s.Id == SportModel.Id).FirstOrDefault()
                                .Name = SportModel.Name;
                            db.SubmitChanges();
                            RefreshGrid();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Dynamics.LastEscapeTime = DateTime.Now;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var sport = ((FrameworkElement)sender).DataContext as Data.Sport;
            SportModel.Name = sport.Name;
            SportModel.Id = sport.Id;
            action = Actions.Editing;

            DialogHost.IsOpen = true;
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var sport = ((FrameworkElement)sender).DataContext as Data.Sport;
            //SportModel.Name = sport.Name;
            SportModel.Id = sport.Id;
            //action = Actions.Editing;

            Popup1.IsOpen = true;
        }
        private void ConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //var c = db.Sports.FirstOrDefault(s => s.Id == SportModel.Id).Courses.ToList();
                if (!db.Sports.FirstOrDefault(s => s.Id == SportModel.Id).Courses.Any())
                {
                    db.Sports.DeleteOnSubmit(db.Sports.Single(s => s.Id == SportModel.Id));
                    db.SubmitChanges();
                    RefreshGrid();
                }
                else
                {

                    Message.Content = "برای این رشته، دوره ثبت شده است. ابتدا دوره های مرتبط را حذف کنید";
                    DuplicateUsernameSnackBar.IsActive = true;
                }
            }
            catch
            {
                ;
            }
            finally
            {
                Popup1.IsOpen = false;
            }
        }
        private void CancelDelete_Click(object sender, RoutedEventArgs e)
        {
            ;
            Popup1.IsOpen = false;
        }
        private void NewSport_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = SportModel = new SportVM();
            action = Actions.Inserting;
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateUsernameSnackBar.IsActive = false;// = TimeSpan.FromSeconds(3);
        }
    }
}
