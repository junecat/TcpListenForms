using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenForms {
    public class TestStep {
        public int Id { get; set; }
        public string ControlName { get; set; }
        public ControlActions ControlAction { get; set; }
        public string Content { get; set; }

    }

    public enum ControlActions {
        FillField,
        PressButton,
        InitData,
        PressKeys,
    }

}
