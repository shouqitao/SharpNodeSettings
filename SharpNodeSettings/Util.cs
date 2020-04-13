using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using SharpNodeSettings.Device;
using SharpNodeSettings.Node.Device;
using SharpNodeSettings.Node.Regular;

namespace SharpNodeSettings {
    /// <summary>
    ///     节点配置类的工具辅助类
    /// </summary>
    public class Util {
        #region Static Method

        /// <summary>
        ///     子窗口的图标显示信息
        /// </summary>
        /// <returns></returns>
        public static Icon GetWinformICon() {
            return Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }


        /// <summary>
        ///     解析一个配置文件中的所有的规则解析，并返回一个词典信息
        /// </summary>
        /// <param name="nodeClass">配置文件的根信息</param>
        /// <returns>词典</returns>
        public static Dictionary<string, List<RegularItemNode>> ParesRegular(XElement nodeClass) {
            var regularkeyValuePairs = new Dictionary<string, List<RegularItemNode>>();
            foreach (var xmlNode in nodeClass.Elements())
                if (xmlNode.Attribute("Name").Value == "Regular")
                    foreach (var element in xmlNode.Elements("RegularNode")) {
                        var itemNodes = new List<RegularItemNode>();
                        foreach (var xmlItemNode in element.Elements("RegularItemNode")) {
                            var regularItemNode = new RegularItemNode();
                            regularItemNode.LoadByXmlElement(xmlItemNode);
                            itemNodes.Add(regularItemNode);
                        }

                        if (regularkeyValuePairs.ContainsKey(element.Attribute("Name").Value))
                            regularkeyValuePairs[element.Attribute("Name").Value] = itemNodes;
                        else
                            regularkeyValuePairs.Add(element.Attribute("Name").Value, itemNodes);
                    }

            return regularkeyValuePairs;
        }


        /// <summary>
        ///     通过真实配置的设备信息，来创建一个真实的设备，如果类型不存在，将返回null
        /// </summary>
        /// <param name="device">设备的配置信息</param>
        /// <returns>真实的设备对象</returns>
        public static DeviceCore CreateFromXElement(XElement device) {
            var deviceType = int.Parse(device.Attribute("DeviceType").Value);

            if (deviceType == DeviceNode.ModbusTcpAlien)
                return new DeviceModbusTcpAlien(device);
            if (deviceType == DeviceNode.ModbusTcpClient)
                return new DeviceModbusTcp(device);
            if (deviceType == DeviceNode.MelsecMcQna3E)
                return new DeviceMelsecMc(device);
            if (deviceType == DeviceNode.Omron)
                return new DeviceOmron(device);
            if (deviceType == DeviceNode.Siemens)
                return new DeviceSiemens(device);
            return null;
        }

        #endregion
    }
}