using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Nancy.Hosting.Self;

namespace TcpListenForms {
    public class TestedForm : Form{
        
        int testport;
        NancyHost nancyHost=null;

        public void OnShowingForm() {
            // start server for debug 

            try {
                HostConfiguration hostConfigs = new HostConfiguration()
                {
                    UrlReservations = new UrlReservations() { CreateAutomatically = true }
                };
                nancyHost = new NancyHost(hostConfigs, new Uri($"http://localhost:{Globals.TestPort}"));
                nancyHost.Start();
                Logger.WriteLog($"Nancy REST TEST Server started(>>>) at port={Globals.TestPort}");

                Globals.TForm = this;
                Globals.TestPort++;
            }
            catch(Exception ex) {
                Logger.WriteError(ex);
            }
        }

        public void OnClosingForm() {
            if (nancyHost == null)
                return;
            try {
                nancyHost.Stop();
                Logger.WriteLog($"Nancy REST TEST Server stopped(<<<) at port={Globals.TestPort-1}");
            }
            catch (Exception ex) {
                Logger.WriteError(ex);
            }
            finally {
                Globals.TForm = null;
                Globals.TestPort--;
            }
        }

        //void TestRestServer() {
            
        //    Debug.Print($"Hi from tcp-server, thread {Thread.CurrentThread.GetHashCode()}");
        //    TcpListener server = null;
        //    try {
        //        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        //        server = new TcpListener(localAddr, testport);

        //        // запуск слушателя
        //        bool stay_connected = true;
        //        byte[] retbuf;
        //        server.Start();


        //        while (stay_connected) {
        //            // получаем входящее подключение
        //            Logger.WriteLog("Wait for accept");
        //            TcpClient client = server.AcceptTcpClient();
        //            Logger.WriteLog("Подключен клиент. Выполнение запроса...");
        //            string s = "";
        //            NetworkStream stream;
        //            s = "";
        //            // получаем сетевой поток для чтения и записи
        //            stream = client.GetStream();
        //            int len;
        //            byte[] buffer0 = new byte[4];
        //            len = stream.Read(buffer0, 0, 4);
        //            if (len == 4) {
        //                int jlen = buffer0[0] + buffer0[1] * 256 + buffer0[2] * 256 * 256;
        //                byte[] buff = new byte[jlen];
        //                len = stream.Read(buff, 0, jlen);
        //                if (len == jlen) {
        //                    MakeTests(buff);
        //                    retbuf = Encoding.UTF8.GetBytes("test completed!");
        //                    stream.Write(retbuf, 0, retbuf.Length);
        //                    stream.Close();
        //                    client.Close();
        //                }
        //                else {
        //                    MessageBox.Show($"Несоответствие длинны буфера: {len}!={jlen}");
        //                }
        //            }
        //            else {
        //                if (len > 0) {
        //                    // ожидаем "точку" - признак закрытия сокета
        //                    string str = Encoding.UTF8.GetString(buffer0, 0, len);
        //                    if (str == ".") {
        //                        // Хватит слушать
        //                        stay_connected = false;
        //                        retbuf = Encoding.UTF8.GetBytes("bye!");
        //                        stream.Write(retbuf, 0, retbuf.Length);
        //                        stream.Close();
        //                        client.Close();
        //                    }
        //                }
        //            }
        //            Thread.Sleep(50);
        //            Logger.WriteLog($"stream closed, stay_connected={stay_connected}");
        //        }
        //    }
        //    catch (Exception ex) {
        //        Debug.Print($"Error {ex.Message}");
        //    }
        //    Thread.CurrentThread.Abort();
        //}

        void MakeTests(byte[] buff) {
            // десериализуем буфер
            string str = Encoding.UTF8.GetString(buff, 0, buff.Length);
            List<TestStep> testSteps = JsonSerializer.Deserialize<List<TestStep>>(str);
            foreach(TestStep ts in testSteps) {
                Thread.Sleep(100);
                MakeTestStep(ts);
            }
        }

        public void MakeTestStep(TestStep ts) {
            Logger.WriteLog($"id={ts.Id}, action={ts.ControlAction}, control={ts.ControlName}, content={ts.Content}");
            bool founded = false;
            foreach ( Control c in Controls) {
                if ( c.Name==ts.ControlName) {
                    founded = true;
                    switch (ts.ControlAction) {
                        case ControlActions.FillField:
                            TextBox tBox = c as TextBox;
                            SetTextBox(tBox, ts.Content);
                            break;
                        
                        case ControlActions.PressButton:
                            PressButton(c as Button);
                            break;

                        case ControlActions.PressKeys:
                            MySendKeys(c as TextBox, ts.Content);
                            break;

                        default:
                            MessageBox.Show($"Нет поддержки ControlAction={ts.ControlAction}");
                            break;
                    }
                }
            }
            if (!founded)
                Logger.WriteLog($"control {ts.ControlName} not founded");
        }

        //public class SetTextBoxData {
        //    public TextBox TextBox { get; set; }
        //    public string Value { get; set; }
        //}

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
        
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);


        public void MySendKeys(TextBox textBox, string str) {
            if (InvokeRequired) {
                this.Invoke(new Action<TextBox, string>(MySendKeys), new object[] { textBox, str });
                return;
            }

            IntPtr hwnd = textBox.Handle;
            foreach(char ch in str) {
                uint chcode = (uint)ch;
                SendMessage(hwnd, 258, chcode, 0);
                Thread.Sleep(10);
            }
            // uint msg =
            //SendMessage(hwnd, 258, 48, 0);
            //Thread.Sleep(10);

        }

        public void SetTextBox(TextBox textBox, string value) {
            if (InvokeRequired) {
                this.Invoke(new Action<TextBox, string>(SetTextBox), new object[] { textBox, value });
                return;
            }
            textBox.Focus();
            textBox.Text = value;
        }

        public void PressButton(Button btn) {
            if (InvokeRequired) {
                this.Invoke(new Action<Button>(PressButton), new object[] { btn });
                return;
            }
            btn.Focus();
            btn.PerformClick();
        }
    }


}
