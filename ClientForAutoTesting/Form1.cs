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
using System.Net.Sockets;
using System.Diagnostics;
using RestSharp;

namespace ClientForAutoTesting {
    public partial class Form1 : Form {

        int testport = 1234;
        string baseUrl = "http://localhost:1234/";
        RestClient restClient;


        public Form1() {
            InitializeComponent();
            linkLabel1.Click += LinkLabel1_Click;
            linkLabel2.Click += LinkLabel2_Click;
            linkLabel3.Click += LinkLabel3_Click;
            linkLabel4.Click += LinkLabel4_Click;
            Shown += Form1_Shown;
        }
        private void Form1_Shown(object sender, EventArgs e) {
            Location = new Point(-812, -85);
            testContentTb.Text = ClientForAutoTesting.Properties.Settings.Default.TestContent;
        }

        private void LinkLabel4_Click(object sender, EventArgs e) {
            ClientForAutoTesting.Properties.Settings.Default.TestContent = testContentTb.Text;
            ClientForAutoTesting.Properties.Settings.Default.Save();

            // ... to json
            try {
                List<TestStep> testSteps = JsonSerializer.Deserialize<List<TestStep>>(testContentTb.Text);
                var restClient = new RestClient();

                foreach ( TestStep ts in testSteps) {
                    string t = JsonSerializer.Serialize(ts);
                    
                    RestRequest req = new RestRequest("http://localhost:1234/", Method.POST);
                    req.RequestFormat = DataFormat.Json;
                    req.AddBody(ts);
                    var resp = restClient.Execute(req);


                }



            }
            catch(Exception ex) {

            }



        }


        private void LinkLabel3_Click(object sender, EventArgs e) {
            string fp = GetFilePath("case2");
            SendTestCase(fp, 1201);
        }


        private void LinkLabel2_Click(object sender, EventArgs e) {
            string fp = GetFilePath("case2");
            SendTestCase(fp, 1201);
        }

        void SendTestCase(string filePath, int portNumber) {
            try {
                using (TcpClient tcpClient = new TcpClient()) {
                    tcpClient.Connect("127.0.0.1", portNumber);
                    NetworkStream netStream = tcpClient.GetStream();

                    // get file

                    // read file to buffer

                    byte[] jbuff = File.ReadAllBytes(filePath);
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
            File.WriteAllText(GetFilePath("case2"), JsonSerializer.Serialize(tc, joptions));

        }

        string GetFilePath(string case_name) {
            var fldPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var jpath = Path.Combine(fldPath, $"{case_name}.json");
            return jpath;
        }
    }
}
