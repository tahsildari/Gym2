using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gym.Domain;
using static Gym.Domain.Enums;

namespace Gym.Controls
{
    /// <summary>
    /// Interaction logic for TrasnactionsList.xaml
    /// </summary>
    public partial class TrasnactionsList : UserControl
    {
        public TrasnactionsList()
        {
            InitializeComponent();
        }

        DateTime? _fromDate; DateTime? _toDate; int? _memberId; TransactionType? _type;
        public void SetFilters(DateTime? fromDate, DateTime? toDate, int memberId, TransactionType type)
        {
            _fromDate = fromDate;
            _toDate = toDate;
            _memberId = memberId;
            _type = type;
        }

        public void LoadPayments()
        {
            var db = new Data.GymContextDataContext();
            var trans = db.Transactions.AsQueryable();

            if (_fromDate != null)
                trans = trans.Where(t => t.Datetime >= _fromDate);

            if (_toDate != null)
                trans = trans.Where(t => t.Datetime <= _toDate);

            if (_memberId != null)
                trans = trans.Where(t => t.MemberId == _memberId);

            if (_type != null)
                trans = trans.Where(t => t.Type == (byte)_type);

            var list = trans.ToList().Select(t=>new { t.Amount, Datetime = t.Datetime.ToFa(), t.Type, t.Method, Member= t.Member.Fullname(), t.Info });

            DataGrid.DataContext = list;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPayments();
        }
    }
}
