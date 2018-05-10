using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Domain
{
    public class TelegramAPI
    {
        static string token = "505834097:AAG_wv3kKb1erxiU1wCxQ7xQtfF11SGfesE";
        //public static string WeClickAlertsGroupId = "-212399513";
        //public static string WeClickDevelopersGroupId = "-106746860";
        //public static string AliGroupId = "-100021604";
        //public static string LogGroupId = "-129194503";
        //public static string DevLogGroupId = "-162871981";
        //public static string TapLeaderLog = "-180837314";
        public static string FilmsComing = "-1001213179758";

        static string sendMessageUrlTemplate = "https://api.telegram.org/bot{0}/sendMessage";


        public static bool SendMessage(string text, string groupId = "-1001213179758")
        {
            Send(text, groupId);
            return true;
        }

        private static async Task Send(string text, string groupId = "-1001213179758")
        {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string postData = JsonConvert.SerializeObject(new { chat_id = groupId, text = text });
                        HttpResponseMessage resp = await client.PostAsync(string.Format(sendMessageUrlTemplate, token),
                            new StringContent(postData, Encoding.UTF8, "application/json"));

                        string tmp = await resp.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR in Telegram API: " + ex.Message);
                    }
                }
        }
    }
}
