using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain
{
    public class Dynamics
    {
        static int _InsuranceFee;
        public static int InsuranceFee
        {
            get
            {
                return _InsuranceFee;
            }
        }

        static int _SingleSessionPrice;
        public static int SingleSessionPrice
        {
            get
            {
                return _SingleSessionPrice;
            }
        }

        static int _ExtendDaysInterval;
        public static int ExtendDaysInterval
        {
            get
            {
                return _ExtendDaysInterval;
            }
        }

        static int _TransitFarthestHour;
        public static int TransitFarthestHour
        {
            get
            {
                return _TransitFarthestHour;
            }
        }


        static string _AppTitle;
        public static string AppTitle
        {
            get
            {
                return _AppTitle;
            }
        }

        static double _ClosetsWidth;
        public static double ClosetsWidth { get { return _ClosetsWidth; } }

        static public void Refresh()
        {
            var db = new Data.GymContextDataContext();
            _InsuranceFee = int.Parse(db.Settings.Where(s => s.Key == "insurance_price").FirstOrDefault().Value);
            _SingleSessionPrice = int.Parse(db.Settings.Where(s => s.Key == "single_price").FirstOrDefault().Value);
            _ExtendDaysInterval = int.Parse(db.Settings.Where(s => s.Key == "extend_days").FirstOrDefault().Value);
            _TransitFarthestHour = int.Parse(db.Settings.Where(s => s.Key == "transit_hours").FirstOrDefault().Value);
            _AppTitle = db.Settings.Where(s => s.Key == "app_title").FirstOrDefault().Value;
            _ClosetsWidth = int.Parse(db.Settings.Where(s => s.Key == "closets_width").FirstOrDefault().Value);

        }
    }
}
