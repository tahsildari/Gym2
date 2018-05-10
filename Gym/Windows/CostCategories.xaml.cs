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
    /// Interaction logic for CostCategories.xaml
    /// </summary>
    public partial class CostCategories : Window
    {
        public CostCategories()
        {
            InitializeComponent();
            this.PreviewKeyUp += Window_PreviewKeyUp;
        }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }


        ObservableCollection<Data.Cost> CostsList;
        Data.GymContextDataContext db = new Data.GymContextDataContext();
        CostVM CostModel = new CostVM();

        Actions action = Actions.Inserting;
        bool IsNew() => action == Actions.Inserting;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            this.DataContext = CostModel;

        }

        private void RefreshGrid()
        {
            var costs = db.Costs.ToList();
            CostsList = new ObservableCollection<Data.Cost>(costs);

            DataGrid.DataContext = CostsList;
        }

        private void Dialog_Closing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var confirmed = (bool)eventArgs.Parameter;
            if (confirmed)
                if (!string.IsNullOrEmpty(CostModel.Category))
                {
                    switch (action)
                    {
                        case Actions.Inserting:
                            if (!db.Costs.Any(s => s.Category == CostModel.Category))
                            {
                                db.Costs.InsertOnSubmit(
                                    new Data.Cost
                                    {
                                        Category = CostModel.Category
                                    });
                                db.SubmitChanges();

                                RefreshGrid();
                            }
                            else
                            {
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
                            db.Costs.Where(s => s.Id == CostModel.Id).FirstOrDefault()
                                .Category = CostModel.Category;
                            db.SubmitChanges();
                            RefreshGrid();
                            break;
                        default:
                            break;
                    }
                }

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var cost = ((FrameworkElement)sender).DataContext as Data.Cost;
            CostModel.Id = cost.Id;
            CostModel.Category = cost.Category;
            action = Actions.Editing;

            DialogHost.IsOpen = true;
        }

        private void NewSport_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = CostModel = new CostVM();
            action = Actions.Inserting;
        }

        private void HideSnackbar_Click(object sender, RoutedEventArgs e)
        {
            DuplicateUsernameSnackBar.IsActive = false;// = TimeSpan.FromSeconds(3);
        }
    }
}
