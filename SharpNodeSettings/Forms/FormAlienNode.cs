﻿using System;
using System.Windows.Forms;
using SharpNodeSettings.Node.Server;

namespace SharpNodeSettings.Forms {
    public partial class FormAlienNode : Form {
        public FormAlienNode(AlienServerNode alienNode = null) {
            InitializeComponent();
            AlienNode = alienNode ?? new AlienServerNode();
            Icon = Util.GetWinformICon();
        }


        /// <summary>
        ///     异形服务器的节点
        /// </summary>
        public AlienServerNode AlienNode { get; set; }

        private void FormAlienNode_Load(object sender, EventArgs e) {
            if (AlienNode != null) {
                textBox1.Text = AlienNode.Name;
                textBox2.Text = AlienNode.Description;
                textBox3.Text = AlienNode.Password;
                textBox4.Text = AlienNode.Port.ToString();
            }
        }

        private void userButton1_Click(object sender, EventArgs e) {
            if (textBox3.Text.Length > 6) {
                MessageBox.Show("密码最大长度为6");
                return;
            }

            if (!int.TryParse(textBox4.Text, out var port)) {
                MessageBox.Show("端口号输入失败！");
                return;
            }


            AlienNode = new AlienServerNode {
                Name = textBox1.Text,
                Description = textBox2.Text,
                Password = textBox3.Text,
                Port = port
            };

            DialogResult = DialogResult.OK;
        }
    }
}