using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;

namespace Gym.Domain
{
    [ServiceContract]
    public interface IGymService
    {
        [OperationContract]
        bool PassMember(string code, DateTime date);
    }
    
    public class GymService : IGymService
    {
        public bool PassMember(string code, DateTime date)
        {
            var main = Application.Current.MainWindow as Windows.Main;
            var allowed= main.PassingMember(code, date);
            return allowed;
        }

    }

    public class GymHost
    {
        public static ServiceHost host;
        //netsh http add urlacl url=http://+:2344/gym user=everyone
        public static void Run()
        {
            try
            {

                // Create the ServiceHost.
                Uri baseAddress = new Uri("http://localhost:2344/gym");

                //Create ServiceHost
                ServiceHost host = new ServiceHost(typeof(GymService), baseAddress);

                //Add a service endpoint
                host.AddServiceEndpoint(typeof(IGymService), new WSHttpBinding(), "");

                //Enable metadata exchange
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);

                //Start the Service
                host.Open();
                //Console.WriteLine("Service is host at " + DateTime.Now.ToString());
                //Console.WriteLine("Host is running... Press  key to stop");
                //Console.ReadLine();
                //System.Windows.Forms.MessageBox.Show("سرویس اتصال به دستگاه اثر انگشت فعال می باشد");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("به علت Run as Admin نبودن برنامه، فعلا سرویس اثرانگشت و گیت فعال نمی باشد");
            }
        }
        public static void Stop() {

            // Close the ServiceHost.
            host.Close();
        }
    }
}
