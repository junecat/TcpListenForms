using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpListenForms {
    public partial class FAdd : Form {
        public FAdd() {
            InitializeComponent();
            cancelBtn.Click += (s, a) => Close();
            saveBtn.Click += SaveBtn_Click;
        }

        Person _p;
        FMain_v0 parentForm;
        public void InitData(Person p, FMain_v0 parent) {
            parentForm = parent;
            if (p!=null) {
                string tmpj = JsonSerializer.Serialize<Person>(p);
                _p = JsonSerializer.Deserialize<Person>(tmpj);
            }
            else
                _p = new Person();

            personBindingSource.DataSource = _p;
        }

        private void SaveBtn_Click(object sender, EventArgs e) {
            parentForm.AddOrUpdate(_p);
            Close();
        }
    }
}
