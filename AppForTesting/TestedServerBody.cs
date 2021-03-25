using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Nancy;
using Nancy.Extensions;

namespace TcpListenForms {
    public class TestedServerBody : NancyModule {
        public TestedServerBody() {
            Get("/", _ =>
            {
                Console.WriteLine("Hello from get");
                return "Hello World";
            });

            Post("/", _ =>
            {
                
                Console.WriteLine("Hello from post!");
                var jsonStr = this.Request.Body.AsString();
                Logger.WriteLog($"jsonStr={jsonStr}");
                TestStep ts = JsonSerializer.Deserialize<TestStep>(jsonStr);
                if (Globals.TForms != null) {
                    if (Globals.TForms.ContainsKey(ts.FormName)) {
                        TestedForm tf = Globals.TForms[ts.FormName];
                        tf.MakeTestStep(ts);
                    }
                }


                            //var req = Context.Request.Body;
                            //byte[] data = new byte[1000];
                            //Console.WriteLine($"length: {req.Length}");
                            //req.Read(data, 0, data.Length);
                            //Console.WriteLine(System.Text.Encoding.Default.GetString(data));
                return "Hello from POST!";
            });
        }
    }
}
