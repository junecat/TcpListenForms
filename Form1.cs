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

namespace TcpListenForms {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e) {
            Thread srvThread = new Thread( new ThreadStart(TcpServer) );
            srvThread.Start();
        }
        void TcpServer() {
            Debug.Print($"Hi from tcp-server, thread {Thread.CurrentThread.GetHashCode()}");
            TcpListener server = null;
            try {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, 12496);

                // запуск слушателя
                server.Start();
                while (true) {
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




        }
    }
}
