using Gym.Data;
using Gym.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Gym.Domain.Enums;

namespace Gym.Domain
{
    public static class Extensions
    {
        public static string ToFa(this DateTime date)
        {
            try
            {
                PersianCalendar pc = new PersianCalendar();
                return pc.GetYear(date) + "/" + pc.GetMonth(date) + "/" + pc.GetDayOfMonth(date);
            }
            catch { return ""; }
        }

        public static string ToFaTime(this DateTime date)
        {
            try
            {
                PersianCalendar pc = new PersianCalendar();
                return pc.GetYear(date) + "/" + pc.GetMonth(date) + "/" + pc.GetDayOfMonth(date)+ " " +
                    date.ToString("HH:mm:ss");
            }
            catch { return ""; }
        }
        public static DateTime? ToEn(this string date)
        {
            try
            {
                var parts = date.Split('/').Select(int.Parse).ToList();
                return new DateTime(parts[0], parts[1], parts[2], new PersianCalendar());
            }
            catch { return null; }
        }
        public static void Save(this BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
        public static string Fullname(this Member member)
        {
            return $"{member?.Firstname} {member?.Lastname}";
        }
        public static bool Update(this MemberVM memberVM)
        {
            try
            {
                var db = new Data.GymContextDataContext();
                var obj = db.Members.Where(m => m.Id == memberVM.Id).FirstOrDefault();
                obj.Address = memberVM.Address;
                obj.Birthdate = memberVM.BirthDate?.ToFa();
                obj.Dadsname = memberVM.Fathername;
                obj.Description = memberVM.Description;
                obj.Firstname = memberVM.Firstname;
                obj.Lastname = memberVM.Lastname;
                obj.Mobile = memberVM.Mobile;
                obj.NationalCode = memberVM.NationalCode;
                obj.Referrer = memberVM.Referrer;
                obj.ReferrerMobile = memberVM.ReferrerMobile;
                obj.IsActive = memberVM.IsActive;

                var oldCloset = db.Closets.Where(c => c.RentorId == memberVM.Id).FirstOrDefault();
                if (oldCloset != null)
                {
                    oldCloset.RentorId = null;
                }

                var closet = db.Closets.Where(c => c.Id == memberVM.ClosetId).FirstOrDefault();
                if (closet != null)
                    closet.RentorId = obj.Id;

                db.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static int? Insert(this MemberVM memberVM)
        {
            try
            {
                var db = new Data.GymContextDataContext();
                var obj = new Data.Member(); db.Members.InsertOnSubmit(obj);
                obj.Address = memberVM.Address;
                obj.Birthdate = memberVM.BirthDate?.ToFa();
                obj.Dadsname = memberVM.Fathername;
                obj.Description = memberVM.Description;
                obj.Firstname = memberVM.Firstname;
                obj.Lastname = memberVM.Lastname;
                obj.Mobile = memberVM.Mobile;
                obj.NationalCode = memberVM.NationalCode;
                obj.IsRegular = memberVM.IsRegular;
                obj.UserId = Windows.Main.CurrentUser.Id;
                obj.IsActive = true;
                db.SubmitChanges();

                var closet = db.Closets.Where(c => c.Id == memberVM.ClosetId).FirstOrDefault();
                if (closet != null)
                    closet.RentorId = obj.Id;

                db.SubmitChanges();

                if (memberVM.Image != null)
                    try
                    {
                        memberVM.Image.Save(AppDomain.CurrentDomain.BaseDirectory + $"/Images/{obj.Id}.jpg");
                    }
                    catch
                    {
                    }

                return obj.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static bool UseCloset(this MemberVM memberVM, Data.GymContextDataContext db)
        {
            if (memberVM.ClosetId != null) //If has rented closet
            {
                var freecloset = db.Closets.Where(c => c.Id == memberVM.ClosetId).FirstOrDefault();
                if (freecloset != null)
                {
                    freecloset.IsFree = false;
                    freecloset.UserId = memberVM.Id;

                    db.ClosetUsages.InsertOnSubmit(new ClosetUsage
                    {
                        MemberId = memberVM.Id,
                        ClosetId = freecloset.Id,
                        FromTime = DateTime.Now,
                    });
                    db.SubmitChanges();
                    return true;
                }
                else
                    return false;
            }
            else //give them a proper closet
            {
                var isVIP = (from e in db.Enrolls
                             where e.MemberId == memberVM.Id
                             && e.StartDate <= DateTime.Now
                             && e.ExpireDate >= DateTime.Now
                             select e.EnrollCourses.Any(c => c.Course.IsVIP)).ToList().FirstOrDefault();

                var freecloset = db.Closets.Where(c =>
                    c.IsVip == isVIP
                    && c.IsFree
                    && c.RentorId == null
                    && !c.IsBroken).FirstOrDefault();

                if (freecloset != null)
                {
                    freecloset.IsFree = false;
                    freecloset.UserId = memberVM.Id;

                    db.ClosetUsages.InsertOnSubmit(new ClosetUsage
                    {
                        MemberId = memberVM.Id,
                        ClosetId = freecloset.Id,
                        FromTime = DateTime.Now,
                    });
                    db.SubmitChanges();
                    return true;
                }
                else
                    return false;
            }
        }
        public static bool FreeUpCloset(this MemberVM memberVM, Data.GymContextDataContext db)
        {
            var occupiedclosets = db.ClosetUsages.Where(c => c.ToTime == null && c.MemberId == memberVM.Id);
            foreach (ClosetUsage closet in occupiedclosets)
            {
                closet.Closet.IsFree = true;
                closet.Closet.UserId = null;
                closet.ToTime = DateTime.Now;
            }

            db.SubmitChanges();
            return true;

        }
        public static T Get<T>(this GymContextDataContext db, string key)
        {
            var value = db.Settings.Where(s => s.Key == key).FirstOrDefault()?.Value;
            if (value == null) return default(T);
            return (T)Convert.ChangeType(value, typeof(T));
        }
        public static bool Set(this GymContextDataContext db, string key, string value)
        {
            try
            {
                var setting = db.Settings.Where(s => s.Key == key).FirstOrDefault();
                if (setting != null) setting.Value = value;
                else { setting = new Setting(); db.Settings.InsertOnSubmit(setting); }
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static int GetInsurancePrice(this GymContextDataContext db)
        {
            return db.Get<int>("insurance_price");
        }
        public static int GetSinglePrice(this GymContextDataContext db)
        {
            return db.Get<int>("single_price");
        }
        public static string GetName(this PaymentMethods method)
        {
            switch (method)
            {
                case PaymentMethods.Cash:
                    return "نقد";
                case PaymentMethods.Pos:
                    return "POS";
                case PaymentMethods.Card:
                    return "کارت";
                case PaymentMethods.Cheque:
                    return "چک";
                case PaymentMethods.Credit:
                    return "اعتبار بوفه";
                default:
                    return " - ";
            }
        }
        public static string GetName(this TransactionType type)
        {
            switch (type)
            {
                case TransactionType.Tuition:
                    return "شهریه دوره";
                case TransactionType.Credit:
                    return "شارژ اعتبار";
                case TransactionType.BuyStuff:
                    return "خرید از بوفه";
                case TransactionType.Cost:
                    return "پرداخت هزینه";
                case TransactionType.SingleSession:
                    return "تک جلسه";
                case TransactionType.Withdraw:
                    return "برداشت";
                case TransactionType.All:
                    return "";
                default:
                    return "";
            }
        }
    }
    //public static class Dynamics
    //{
    //    static int? _InsurancePrice = null;
    //    public static int InsurancePrice
    //    {
    //        get
    //        {
    //            if (_InsurancePrice == null)
    //                _InsurancePrice = (new GymContextDataContext()).GetInsurancePrice();
    //            return _InsurancePrice.Value;
    //        }
    //        set
    //        {
    //            _InsurancePrice = value;
    //        }
    //    }

    //    static int? _SingleSessionPrice;
    //    public static int SingleSessionPrice
    //    {
    //        get
    //        {
    //            if (_SingleSessionPrice == null)
    //                _SingleSessionPrice = (new GymContextDataContext()).GetSinglePrice();
    //            return _SingleSessionPrice.Value;
    //        }
    //        set
    //        {
    //            _SingleSessionPrice = value;
    //        }
    //    }
    //}
}
