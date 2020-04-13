using System;
using System.Windows.Forms;
using System.Xml.Linq;
using SharpNodeSettings.Forms;

namespace SharpNodeSettings.View {
    public partial class FormSelectDevice : Form {
        public FormSelectDevice() {
            InitializeComponent();
        }

        public XElement DeviceXml { get; set; }

        private void userButton1_Click(object sender, EventArgs e) {
            // 三菱设备
            using (var form = new FormMelsec3E()) {
                if (form.ShowDialog() == DialogResult.OK) {
                    DeviceXml = form.MelsecMc.ToXmlElement();
                    textBox1.Text = DeviceXml.ToString();
                }
            }
        }

        private void userButton2_Click(object sender, EventArgs e) {
            // 西门子设备
            using (var form = new FormSiemens()) {
                if (form.ShowDialog() == DialogResult.OK) {
                    DeviceXml = form.NodeSiemens.ToXmlElement();
                    textBox1.Text = DeviceXml.ToString();
                }
            }
        }

        private void userButton3_Click(object sender, EventArgs e) {
            // 欧姆龙
            using (var form = new FormOmron()) {
                if (form.ShowDialog() == DialogResult.OK) {
                    DeviceXml = form.NodeOmron.ToXmlElement();
                    textBox1.Text = DeviceXml.ToString();
                }
            }
        }

        private void userButton4_Click(object sender, EventArgs e) {
            // Modbus Tcp
            using (var form = new FormModbusTcp()) {
                if (form.ShowDialog() == DialogResult.OK) {
                    DeviceXml = form.ModbusTcpNode.ToXmlElement();
                    textBox1.Text = DeviceXml.ToString();
                }
            }
        }

        private void userButton5_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
        }
    }
}