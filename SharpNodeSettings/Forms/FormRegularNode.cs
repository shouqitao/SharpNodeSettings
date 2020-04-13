using System;
using System.Windows.Forms;
using SharpNodeSettings.Node.Regular;

namespace SharpNodeSettings.Forms {
    public partial class FormRegularNode : Form {
        public FormRegularNode(RegularNode regularNode = null) {
            InitializeComponent();

            RegularNode = regularNode;
            Icon = Util.GetWinformICon();
        }


        public RegularNode RegularNode { get; set; } // 结果信息

        private void FormRegularNode_Load(object sender, EventArgs e) {
            if (RegularNode != null) {
                textBox1.Text = RegularNode.Name;
                textBox2.Text = RegularNode.Description;
            }
        }

        private void userButton1_Click(object sender, EventArgs e) {
            // 检查数据输入
            if (string.IsNullOrEmpty(textBox1.Text)) {
                MessageBox.Show("名称不能为空！");
                textBox1.Focus();
                return;
            }


            RegularNode = new RegularNode {
                Name = textBox1.Text,
                Description = textBox2.Text
            };


            DialogResult = DialogResult.OK;
        }
    }
}