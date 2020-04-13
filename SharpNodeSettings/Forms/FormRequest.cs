using System;
using System.Windows.Forms;
using SharpNodeSettings.Node.Request;

namespace SharpNodeSettings.Forms {
    public partial class FormRequest : Form {
        public FormRequest(DeviceRequest deviceRequest, string[] regularsArray) {
            InitializeComponent();
            DeviceRequest = deviceRequest ?? new DeviceRequest();
            Icon = Util.GetWinformICon();
            RegularsArray = regularsArray;
        }

        private string[] RegularsArray { get; }

        /// <summary>
        ///     单次的设备请求信息
        /// </summary>
        public DeviceRequest DeviceRequest { get; set; }

        private void FormRequest_Load(object sender, EventArgs e) {
            comboBox1.DataSource = RegularsArray;

            textBox1.Text = DeviceRequest.Name;
            textBox2.Text = DeviceRequest.Description;
            textBox3.Text = DeviceRequest.Address;
            textBox4.Text = DeviceRequest.Length.ToString();
            textBox6.Text = DeviceRequest.CaptureInterval.ToString();
            comboBox1.SelectedItem = DeviceRequest.PraseRegularCode;
        }


        private void userButton1_Click(object sender, EventArgs e) {
            var praseRegularCode = "";
            if (comboBox1.SelectedItem != null) praseRegularCode = comboBox1.SelectedItem.ToString();


            try {
                DeviceRequest = new DeviceRequest {
                    Name = textBox1.Text,
                    Description = textBox2.Text,
                    Address = textBox3.Text,
                    Length = ushort.Parse(textBox4.Text),
                    CaptureInterval = int.Parse(textBox6.Text),
                    PraseRegularCode = praseRegularCode
                };
            } catch (Exception ex) {
                MessageBox.Show("数据填入失败！" + ex.Message);
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}