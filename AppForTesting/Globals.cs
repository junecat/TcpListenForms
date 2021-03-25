using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using Nancy.Extensions;

namespace TcpListenForms {
    public static class Globals {
        public static Dictionary<string, TestedForm>  TForms { get; set; }
        static NancyHost nancyHost = null;

        static Globals() {
            // start server for debug 
            try {
                HostConfiguration hostConfigs = new HostConfiguration()
                {
                    UrlReservations = new UrlReservations() { CreateAutomatically = true }
                };
                nancyHost = new NancyHost(hostConfigs, new Uri("http://localhost:1234/"));
                nancyHost.Start();
                TForms = new Dictionary<string, TestedForm>();
                Logger.WriteLog($"Nancy REST TEST Server started(>>>) at port=1234");
            }
            catch (Exception ex) {
                Logger.WriteError(ex);
            }
        }
    }
}
