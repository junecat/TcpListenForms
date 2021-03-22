using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace ClientForAutoTesting {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            linkLabel1.Click += LinkLabel1_Click;
            linkLabel2.Click += LinkLabel2_Click;
        }

        int testport = 1201;

        private void LinkLabel2_Click(object sender, EventArgs e) {
            try {
                using (TcpClient tcpClient = new TcpClient()) {
                    tcpClient.Connect("127.0.0.1", testport);
                    NetworkStream netStream = tcpClient.GetStream();

                    // get file

                    // read file to buffer

                    byte[] jbuff = File.ReadAllBytes(GetFilePath());
                    int len = jbuff.Length;
                    byte[] hbuff = BitConverter.GetBytes(len);
                    netStream.Write(hbuff, 0, 4);
                    netStream.Write(jbuff, 0, len);
                    byte[] buffer = new byte[100];
                    netStream.Read(buffer, 0, 100);
                    logTB.Text += Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    netStream.Close();
                    tcpClient.Close();
                }
            }
            catch (Exception ex) {
                Debug.Print($"Exception: {ex.Message}");
            }

        }

        void LinkLabel1_Click(object sender, EventArgs e) {
            TestStep ts0 = new TestStep();
            ts0.Id = 0;
            ts0.ControlName = "textBox1";
            ts0.ControlAction = ControlActions.FillField;
            ts0.Content = "test it!";

            TestStep ts1 = new TestStep();
            ts1.Id = 1;
            ts1.ControlName = "button1";
            ts1.ControlAction = ControlActions.PressButton;

            //TestCase tc = new TestCase();
            //tc.TestSteps = new List<TestStep>();
            //tc.TestSteps.Add(ts0);
            //tc.TestSteps.Add(ts1);

            List<TestStep> tc = new List<TestStep>();
            tc.Add(ts0);
            tc.Add(ts1);

            var joptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            File.WriteAllText(GetFilePath(), JsonSerializer.Serialize(tc, joptions));

        }

        string GetFilePath() {
            var fldPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var jpath = Path.Combine(fldPath, "case0.json");
            return jpath;
        }
    }
}
