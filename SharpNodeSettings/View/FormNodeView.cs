﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using HslCommunication.Enthernet;
using Newtonsoft.Json.Linq;
using SharpNodeSettings.Node.Device;
using SharpNodeSettings.Node.NodeBase;
using SharpNodeSettings.Node.Regular;
using SharpNodeSettings.Node.Request;
using SharpNodeSettings.Node.Server;
using SharpNodeSettings.Properties;

namespace SharpNodeSettings.View {
    public partial class FormNodeView : Form {
        public FormNodeView(string ipAddress, int port) {
            InitializeComponent();
            Icon = Util.GetWinformICon();
            this.ipAddress = ipAddress;
            this.port = port;
        }

        private ImageList SharpImageList { get; set; }

        private void FormNodeView_Load(object sender, EventArgs e) {
            ImageResourseLoad();
            treeView1.ImageList = SharpImageList;

            treeView1.Nodes[0].ImageKey = "VirtualMachine_16xLG";
            treeView1.Nodes[0].SelectedImageKey = "VirtualMachine_16xLG";
            treeView1.Nodes[0].Tag = new NodeClass {
                Name = "Devices",
                Description = "所有的设备的集合对象"
            };
            treeView1.Nodes[1].ImageKey = "VirtualMachine_16xLG";
            treeView1.Nodes[1].SelectedImageKey = "VirtualMachine_16xLG";
            treeView1.Nodes[1].Tag = new NodeClass {
                Name = "Server",
                Description = "所有挂载的服务器"
            };
        }

        private void ImageResourseLoad() {
            SharpImageList = new ImageList();
            SharpImageList.Images.Add("abstr1", Resources.abstr1);
            SharpImageList.Images.Add("action_add_16xLG", Resources.action_add_16xLG);
            SharpImageList.Images.Add("action_Cancel_16xLG", Resources.action_Cancel_16xLG);
            SharpImageList.Images.Add("ClassIcon", Resources.ClassIcon);
            SharpImageList.Images.Add("Class_489", Resources.Class_489);
            SharpImageList.Images.Add("Enum_582", Resources.Enum_582);
            SharpImageList.Images.Add("Event_594", Resources.Event_594);
            SharpImageList.Images.Add("Event_594_exp", Resources.Event_594_exp);
            SharpImageList.Images.Add("FieldsHeader_12x", Resources.FieldsHeader_12x);
            SharpImageList.Images.Add("FlagRed_16x", Resources.FlagRed_16x);
            SharpImageList.Images.Add("FlagSpace_16x", Resources.FlagSpace_16x);
            SharpImageList.Images.Add("flag_16xLG", Resources.flag_16xLG);
            SharpImageList.Images.Add("GenericVSEditor_9905", Resources.GenericVSEditor_9905);
            SharpImageList.Images.Add("HotSpot_10548", Resources.HotSpot_10548);
            SharpImageList.Images.Add("ExtensionManager_vsix", Resources.ExtensionManager_vsix);
            SharpImageList.Images.Add("HotSpot_10548_color", Resources.HotSpot_10548_color);
            SharpImageList.Images.Add("library_16xLG", Resources.library_16xLG);
            SharpImageList.Images.Add("Method_636", Resources.Method_636);
            SharpImageList.Images.Add("Module_648", Resources.Module_648);
            SharpImageList.Images.Add("Monitor_Screen_16xLG", Resources.Monitor_Screen_16xLG);
            SharpImageList.Images.Add("Operator_660", Resources.Operator_660);
            SharpImageList.Images.Add("PencilAngled_16xLG", Resources.PencilAngled_16xLG);
            SharpImageList.Images.Add("Property_501", Resources.Property_501);
            SharpImageList.Images.Add("server_Local_16xLG", Resources.server_Local_16xLG);
            SharpImageList.Images.Add("star_16xLG", Resources.star_16xLG);
            SharpImageList.Images.Add("usbcontroller", Resources.usbcontroller);
            SharpImageList.Images.Add("VirtualMachine_16xLG", Resources.VirtualMachine_16xLG);
            SharpImageList.Images.Add("WindowsAzure_16xLG", Resources.WindowsAzure_16xLG);
            SharpImageList.Images.Add("WindowsAzure_16xLG_Cyan", Resources.WindowsAzure_16xLG_Cyan);
            SharpImageList.Images.Add("xbox1Color_16x", Resources.xbox1Color_16x);
        }

        private void FormNodeView_Shown(object sender, EventArgs e) {
            simplifyClient = new NetSimplifyClient(ipAddress, port);
            if (!simplifyClient.ConnectServer().IsSuccess) {
                MessageBox.Show("连接服务器失败！请稍候重新连接");
                Close();
                return;
            }


            // 请求配置文件信息
            var resultXml = simplifyClient.ReadFromServer(0, "");
            if (!resultXml.IsSuccess) {
                MessageBox.Show("请求配置文件异常，准备退出！");
                Close();
                return;
            }

            ParseFromXmlResourse(resultXml.Content);
        }


        private void ParseFromXmlResourse(string xmlString) {
            var xElement = XElement.Parse(xmlString);
            regularkeyValuePairs = Util.ParesRegular(xElement);

            TreeNodeRender(treeView1.Nodes[0], xElement.Elements().ToArray()[0]);
            TreeNodeRender(treeView1.Nodes[1], xElement.Elements().ToArray()[1]);

            treeView1.ExpandAll();
        }


        private void TreeNodeRender(TreeNode treeNode, XElement element) {
            foreach (var item in element.Elements())
                if (item.Name == "NodeClass") {
                    var node = new TreeNode(item.Attribute("Name").Value);
                    node.ImageKey = "Class_489";
                    node.SelectedImageKey = "Class_489";

                    var nodeClass = new NodeClass();
                    nodeClass.LoadByXmlElement(item);
                    node.Tag = nodeClass;
                    treeNode.Nodes.Add(node);

                    TreeNodeRender(node, item);
                } else if (item.Name == "DeviceNode") {
                    var type = int.Parse(item.Attribute("DeviceType").Value);

                    var deviceNode = new TreeNode(item.Attribute("Name").Value);
                    if (type == DeviceNode.ModbusTcpClient) {
                        deviceNode.ImageKey = "Module_648";
                        deviceNode.SelectedImageKey = "Module_648";

                        var modbusNode = new NodeModbusTcpClient();
                        modbusNode.LoadByXmlElement(item);
                        deviceNode.Tag = modbusNode;
                    } else if (type == DeviceNode.ModbusTcpAlien) {
                        deviceNode.ImageKey = "Module_648";
                        deviceNode.SelectedImageKey = "Module_648";

                        var modbusAlien = new NodeModbusTcpAline();
                        modbusAlien.LoadByXmlElement(item);
                        deviceNode.Tag = modbusAlien;
                    } else if (type == DeviceNode.MelsecMcQna3E) {
                        deviceNode.ImageKey = "Enum_582";
                        deviceNode.SelectedImageKey = "Enum_582";

                        var node = new NodeMelsecMc();
                        node.LoadByXmlElement(item);
                        deviceNode.Tag = node;
                    } else if (type == DeviceNode.Siemens) {
                        deviceNode.ImageKey = "Event_594";
                        deviceNode.SelectedImageKey = "Event_594";

                        var node = new NodeSiemens();
                        node.LoadByXmlElement(item);
                        deviceNode.Tag = node;
                    } else if (type == DeviceNode.DeviceNone) {
                        deviceNode.ImageKey = "Method_636";
                        deviceNode.SelectedImageKey = "Method_636";

                        var node = new NodeEmpty();
                        node.LoadByXmlElement(item);
                        deviceNode.Tag = node;
                    } else if (type == DeviceNode.Omron) {
                        deviceNode.ImageKey = "HotSpot_10548_color";
                        deviceNode.SelectedImageKey = "HotSpot_10548_color";

                        var node = new NodeOmron();
                        node.LoadByXmlElement(item);
                        deviceNode.Tag = node;
                    }

                    treeNode.Nodes.Add(deviceNode);
                    foreach (var request in item.Elements("DeviceRequest")) {
                        var deviceRequest = new DeviceRequest();
                        deviceRequest.LoadByXmlElement(request);
                        var parseCode = deviceRequest.PraseRegularCode;
                        if (regularkeyValuePairs.ContainsKey(parseCode))
                            foreach (var regularNodeItem in regularkeyValuePairs[parseCode]) {
                                var treeNodeRegular = new TreeNode(regularNodeItem.Name);
                                treeNodeRegular.ImageKey = "Operator_660";
                                treeNodeRegular.SelectedImageKey = "Operator_660";


                                treeNodeRegular.Tag = regularNodeItem;
                                deviceNode.Nodes.Add(treeNodeRegular);
                            }
                    }
                } else if (item.Name == "ServerNode") {
                    var type = int.Parse(item.Attribute("ServerType").Value);

                    if (type == ServerNode.ModbusServer) {
                        var node = new TreeNode(item.Attribute("Name").Value);
                        node.ImageKey = "server_Local_16xLG";
                        node.SelectedImageKey = "server_Local_16xLG";

                        var nodeClass = new NodeModbusServer();
                        nodeClass.LoadByXmlElement(item);
                        node.Tag = nodeClass;
                        treeNode.Nodes.Add(node);
                    } else if (type == ServerNode.AlienServer) {
                        var node = new TreeNode(item.Attribute("Name").Value);
                        node.ImageKey = "server_Local_16xLG";
                        node.SelectedImageKey = "server_Local_16xLG";

                        var nodeClass = new AlienServerNode();
                        nodeClass.LoadByXmlElement(item);
                        node.Tag = nodeClass;
                        treeNode.Nodes.Add(node);

                        TreeNodeRender(node, item);
                    }
                } else if (item.Name == "RegularNode") {
                    var node = new TreeNode(item.Attribute("Name").Value);
                    node.ImageKey = "usbcontroller";
                    node.SelectedImageKey = "usbcontroller";

                    var nodeClass = new RegularNode();
                    nodeClass.LoadByXmlElement(item);
                    node.Tag = nodeClass;
                    treeNode.Nodes.Add(node);

                    foreach (var regularItemXml in item.Elements("RegularItemNode")) {
                        var treeNodeRegular = new TreeNode(regularItemXml.Attribute("Name").Value);
                        treeNodeRegular.ImageKey = "Operator_660";
                        treeNodeRegular.SelectedImageKey = "Operator_660";

                        var regularItemNode = new RegularItemNode();
                        regularItemNode.LoadByXmlElement(regularItemXml);
                        treeNodeRegular.Tag = regularItemNode;
                        node.Nodes.Add(treeNodeRegular);
                    }
                }
        }

        private string[] GetTreePath(TreeNode treeNode) {
            var paths = new List<string>();
            while (true)
                if (treeNode != null) {
                    paths.Add(treeNode.Text);
                    treeNode = treeNode.Parent;
                } else {
                    break;
                }

            paths.Reverse();
            return paths.ToArray();
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            // 节点被选择的时候
            var node = treeView1.SelectedNode;
            if (node.Tag is NodeClass nodeClass) {
                textBox1.Text = string.Join("/", GetTreePath(node));
                if (nodeClass is DeviceNode) {
                    // 显示设备的所有数据
                    var result = simplifyClient.ReadFromServer(1, textBox1.Text);
                    if (result.IsSuccess) {
                        if (!string.IsNullOrEmpty(result.Content))
                            DataGridViewRenderNodeClass(JObject.Parse(result.Content));
                    } else {
                        MessageBox.Show("Wrong");
                    }
                } else if (nodeClass.GetType() == typeof(RegularItemNode)) {
                    var result = simplifyClient.ReadFromServer(1, textBox1.Text);
                    if (result.IsSuccess) {
                        DataGridSpecifyRowCount(1);
                        dataGridView1.Rows[0].Cells[0].Value = node.Text;
                        dataGridView1.Rows[0].Cells[1].Value = result.Content;
                    } else {
                        MessageBox.Show("Wrong");
                    }
                } else {
                    // 显示选择的节点信息
                    DataGridViewRenderNodeClass(nodeClass);
                }
            }
        }

        private void DataGridViewRenderNodeClass(NodeClass nodeClass) {
            var renders = nodeClass.GetNodeClassRenders();
            DataGridSpecifyRowCount(renders.Count);
            for (var i = 0; i < renders.Count; i++) {
                dataGridView1.Rows[i].Cells[0].Value = renders[i].ValueName;
                dataGridView1.Rows[i].Cells[1].Value = renders[i].Value;
            }
        }

        private void DataGridViewRenderNodeClass(JObject json) {
            DataGridSpecifyRowCount(json.Count);
            var i = 0;
            foreach (var item in json.Properties()) {
                dataGridView1.Rows[i].Cells[0].Value = item.Name;
                dataGridView1.Rows[i].Cells[1].Value = item.Value;
                i++;
            }
        }


        private void DataGridSpecifyRowCount(int row) {
            if (dataGridView1.RowCount < row) {
                // 扩充
                dataGridView1.Rows.Add(row - dataGridView1.RowCount);
            } else if (dataGridView1.RowCount > row) {
                var deleteRows = dataGridView1.RowCount - row;
                for (var i = 0; i < deleteRows; i++) dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
            }

            if (row > 0) dataGridView1.Rows[0].Cells[0].Selected = false;
        }


        private void FormNodeView_FormClosing(object sender, FormClosingEventArgs e) {
            simplifyClient?.ConnectClose();
        }

        #region Private Member

        private readonly string ipAddress = "127.0.0.1"; // 服务器的Ip地址
        private readonly int port = 12345; // 服务器的端口号
        private NetSimplifyClient simplifyClient; // 网络请求服务
        private Dictionary<string, List<RegularItemNode>> regularkeyValuePairs; // 所有的解析规则列表的对象

        #endregion
    }
}