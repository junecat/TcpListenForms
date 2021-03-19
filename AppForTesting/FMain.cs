using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.Json;

namespace TcpListenForms {
    public partial class FMain : Form {
        public FMain() {
            InitializeComponent();
            Shown += Form1_Shown;
            FormClosing += FMain_FormClosing;
        }

        private void FMain_FormClosing(object sender, FormClosingEventArgs e) {
            File.WriteAllText(Jpath(), JsonSerializer.Serialize(pList));

            try {
                using (TcpClient tcpClient = new TcpClient()) {
                    tcpClient.Connect("127.0.0.1", 12496);
                    NetworkStream netStream = tcpClient.GetStream();
                    byte[] buffer = new byte[100];
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
        }

        List<Person> pList;
        Thread srvThread;

        string Jpath() {
            var fldPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var jpath = Path.Combine(fldPath, "mynotebook.json");
            return jpath;
        }

        private void Form1_Shown(object sender, EventArgs e) {
            if (File.Exists(Jpath()))
                pList = JsonSerializer.Deserialize<List<Person>>(File.ReadAllText(Jpath()));
            else
                pList = new List<Person>();

            // start server for 
            srvThread = new Thread( new ThreadStart(TcpServer) );
            srvThread.Start();
        }
        void TcpServer() {
            Debug.Print($"Hi from tcp-server, thread {Thread.CurrentThread.GetHashCode()}");
            TcpListener server = null;
            try {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, 12496);

                // запуск слушателя
                bool stay_connected = true;
                server.Start();
                
                while (stay_connected) {
                    // получаем входящее подключение
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Подключен клиент. Выполнение запроса...");
                    string s = "";
                    NetworkStream stream;
                    do {
                        // получаем сетевой поток для чтения и записи
                        stream = client.GetStream();
                        byte[] buffer = new byte[100];
                        int len = stream.Read(buffer, 0, 100);
                        s = "";
                        for (int i = 0; i < len; ++i) {
                            s += (char)buffer[i];
                        }
                        Debug.Print($"{len}: {s}");
                        stream.Write(buffer, 0, len);
                    } while (s != ".");
                    // Хватит слушать
                    stay_connected = false;
                    byte[] buf1 = new byte[4];
                    buf1[0] = (byte)'b';
                    buf1[1] = (byte)'y';
                    buf1[2] = (byte)'e';
                    buf1[3] = (byte)'!';
                    stream.Write(buf1, 0, 4);
                    stream.Close();
                    client.Close();
                }
            }
            catch(Exception ex) {
                Debug.Print($"Error {ex.Message}");
            }
            Thread.CurrentThread.Abort();
        }
    }
}
