using System;
using System.Net;
using System.Text;

namespace CheckLoginTest
{
    class Program
    {
        // 프로그램 명
        public const string ServiceName = "LoginCheckTest";

        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SessionEnded_EventHandler);
                Console.ReadLine();
                Microsoft.Win32.SystemEvents.SessionSwitch -= SessionEnded_EventHandler;

            }
            else
            {
                using (Service service = new Service())
                {
                    System.ServiceProcess.ServiceBase.Run(service);
                }
            }
        }

        private static void SessionEnded_EventHandler(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            sendWebHook(e.Reason.ToString());
        }

        private static void sendWebHook(string strMessage)
        {
            using (WebClient _objwc = new WebClient())
            {
                string strUrl = "";
                string strBody = "{\"body\" : \"공지\",\"connectColor\" : \"#1B8FFA\",\"connectInfo\" : [";

                strBody += "{ \"title\" : \"*" + Environment.UserName + "님*\",\"description\" : \"" + strMessage + "\"}";

                strBody += "]}";

                _objwc.Headers[HttpRequestHeader.ContentType] = "application/json";
                _objwc.Headers[HttpRequestHeader.Accept] = "application/vnd.tosslab.jandi-v2+json";
                _objwc.Encoding = Encoding.UTF8;

                _objwc.UploadString(strUrl, strBody);
            }
        }

        #region Windows Service
        public class Service : System.ServiceProcess.ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
                this.CanHandleSessionChangeEvent = true;
            }

            protected override void OnSessionChange(System.ServiceProcess.SessionChangeDescription changeDescription)
            {
                //base.OnSessionChange(changeDescription);
                Program.sendWebHook(changeDescription.Reason.ToString());
            }

            //protected override void OnStart(string[] args)
            //{
                //base.OnStart(args);
                //Program.setSessionSwitch(args);
            //}

            //protected override void OnStop()
            //{
                //base.OnStop();
                //Program.Stop();
            //}
        }
        #endregion
    }
}
