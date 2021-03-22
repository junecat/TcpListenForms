using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpListenForms {
    public partial class FMain : TestedForm {
        public FMain() {
            InitializeComponent();
            OnShowingForm();
            FormClosing += FMain_FormClosing;
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e) {
            MessageBox.Show("button pressed!");
        }

        private void FMain_FormClosing(object sender, FormClosingEventArgs e) {
            OnClosingForm();
        }
    }
}
