using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Diagnostics;

namespace TcpListenForms {
    public class TestedForm : Form{
        
        int testport;
        Thread srvThread;

        public void OnShowingForm() {
            // start server for debug 
            Globals.BaseTestPort++;
            testport = Globals.BaseTestPort;
            srvThread = new Thread(new ThreadStart(TcpServer));
            srvThread.Start();
        }

        public void OnClosingForm() {
            try {
                using (TcpClient tcpClient = new TcpClient()) {
                    tcpClient.Connect("127.0.0.1", testport);
                    NetworkStream netStream = tcpClient.GetStream();
                    byte[] buffer = new byte[100];
                    Logger.WriteLog($"OnClosingForm(), send '.' for form closing");
                    buffer[0] = (byte)'.';
                    netStream.Write(buffer, 0, 1);
                    netStream.Read(buffer, 0, 100);
                    netStream.Close();
                    tcpClient.Close();
                }
            }
            catch (Exception ex) {
                Debug.Print($"Exception: {ex.Message}");
            }
            finally {
                Globals.BaseTestPort--;
            }
        }

        void TcpServer() {
            Debug.Print($"Hi from tcp-server, thread {Thread.CurrentThread.GetHashCode()}");
            TcpListener server = null;
            try {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, testport);

                // запуск слушателя
                bool stay_connected = true;
                byte[] retbuf;
                server.Start();


                while (stay_connected) {
                    // получаем входящее подключение
                    Logger.WriteLog("Wait for accept");
                    TcpClient client = server.AcceptTcpClient();
                    Logger.WriteLog("Подключен клиент. Выполнение запроса...");
                    string s = "";
                    NetworkStream stream;
                    s = "";
                    // получаем сетевой поток для чтения и записи
                    stream = client.GetStream();
                    int len;
                    byte[] buffer0 = new byte[4];
                    len = stream.Read(buffer0, 0, 4);
                    if (len == 4) {
                        int jlen = buffer0[0] + buffer0[1] * 256 + buffer0[2] * 256 * 256;
                        byte[] buff = new byte[jlen];
                        len = stream.Read(buff, 0, jlen);
                        if (len == jlen) {
                            MakeTests(buff);
                            retbuf = Encoding.UTF8.GetBytes("test completed!");
                            stream.Write(retbuf, 0, retbuf.Length);
                            stream.Close();
                            client.Close();
                        }
                        else {
                            MessageBox.Show($"Несоответствие длинны буфера: {len}!={jlen}");
                        }
                    }
                    else {
                        if (len > 0) {
                            // ожидаем "точку" - признак закрытия сокета
                            string str = Encoding.UTF8.GetString(buffer0, 0, len);
                            if (str == ".") {
                                // Хватит слушать
                                stay_connected = false;
                                retbuf = Encoding.UTF8.GetBytes("bye!");
                                stream.Write(retbuf, 0, retbuf.Length);
                                stream.Close();
                                client.Close();
                            }
                        }
                    }
                    Thread.Sleep(50);
                    Logger.WriteLog($"stream closed, stay_connected={stay_connected}");
                }
            }
            catch (Exception ex) {
                Debug.Print($"Error {ex.Message}");
            }
            Thread.CurrentThread.Abort();
        }

        void MakeTests(byte[] buff) {
            // десериализуем буфер
            string str = Encoding.UTF8.GetString(buff, 0, buff.Length);
            List<TestStep> testSteps = JsonSerializer.Deserialize<List<TestStep>>(str);
            foreach(TestStep ts in testSteps) {
                Thread.Sleep(100);
                MakeTestStep(ts);
            }
        }

        void MakeTestStep(TestStep ts) {
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

    // data classes for testing
    public enum ControlActions {
        FillField,
        PressButton
    }

    public class TestStep {
        public int Id { get; set; }
        public string ControlName { get; set; }
        public ControlActions ControlAction { get; set; }
        public string Content { get; set; }
    }

}
