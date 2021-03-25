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

namespace TcpListenForms {
    public class TestedForm : Form{
        
        public void OnShowingForm() {
            if (Globals.TForms != null) {
                if (!Globals.TForms.ContainsKey(Name))
                    Globals.TForms.Add(Name, this);
            }
        }

        public void OnClosingForm() {
            if (Globals.TForms == null)
                return;
            try {
                if (Globals.TForms.ContainsKey(Name))
                    Globals.TForms.Remove(Name);
            }
            catch (Exception ex) {
                Logger.WriteError(ex);
            }
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
