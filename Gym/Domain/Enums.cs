using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain
{
    public class Enums
    {
        public enum Actions
        {
            Inserting,
            Editing,
            Selected,
            None
        }

        public enum MemberSelectionCategory
        {
            SelectionOnly,
            MembersTransit,
            IrregularTransit,
            DeleteMembers,
            PersonnelTransit,
            TutitionDebtorsList,
            ShoppingDebtorsList,
            NearExpiryList,
            None
        }

        public enum ClosetFormType
        {
            Selection,
            Registration
        }

        public enum Frequencies
        {
            Everyday = 0,
            EveryOtherDay = 1,
            SingleSessions = 2
        }
        public enum PaymentMethods
        {
            Cash = 0,
            Pos = 1,
            Card = 2,
            Cheque = 3,
            Credit = 4,
        }

        public enum TransactionType
        {
            Tuition,
            Credit,
            BuyStuff,
            Cost,
            SingleSession,
            Withdraw,
            All = 1000
        }

        public enum PayingItems
        {
            CourseTuition,
            ChargeCredit,
            GrantDiscount,
            None
        }

        public enum PassageReportType
        {
            MembersReport,
            PersonnelReport,
            SingleSessionsReport
        }

    }
}
