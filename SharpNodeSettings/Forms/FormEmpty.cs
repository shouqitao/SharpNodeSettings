using System;
using System.Windows.Forms;
using SharpNodeSettings.Node.Device;

namespace SharpNodeSettings.Forms {
    public partial class FormEmpty : Form {
        public FormEmpty(NodeEmpty nodeEmpty = null) {
            InitializeComponent();
            NodeEmpty = nodeEmpty ?? new NodeEmpty();
            Icon = Util.GetWinformICon();
        }


        public NodeEmpty NodeEmpty { get; set; }

        private void FormEmpty_Load(object sender, EventArgs e) {
            if (NodeEmpty != null) {
                textBox1.Text = NodeEmpty.Name;
                textBox2.Text = NodeEmpty.Description;
            }
        }


        private void userButton1_Click(object sender, EventArgs e) {
            NodeEmpty = new NodeEmpty {
                Name = textBox1.Text,
                Description = textBox2.Text
            };
            DialogResult = DialogResult.OK;
        }
    }
}