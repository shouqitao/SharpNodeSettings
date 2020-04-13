﻿using System.Collections.Generic;
using System.Xml.Linq;
using SharpNodeSettings.Node.NodeBase;

namespace SharpNodeSettings.Node.Device {
    /// <summary>
    ///     异形ModbusTcp的客户端节点，只能挂在Alien节点下，下面只能挂载异形modbus客户端
    /// </summary>
    public class NodeModbusTcpAline : DeviceNode, IXmlConvert {
        #region Constructor

        /// <summary>
        ///     实例化一个异性的设备对象
        /// </summary>
        public NodeModbusTcpAline() {
            DTU = "12345678901";
            DeviceType = ModbusTcpAlien;

            Name = "异形设备";
            Description = "这是一个异形设备";
            Station = 0x01;
        }

        #endregion

        #region Object Override

        /// <summary>
        ///     返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString() {
            return "[异形设备] " + Name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     设备的唯一号码
        /// </summary>
        public string DTU { get; set; }

        /// <summary>
        ///     设备的站号
        /// </summary>
        public byte Station { get; set; }


        /// <summary>
        ///     起始地址是否从0开始
        /// </summary>
        public bool IsAddressStartWithZero { get; set; } = true;

        /// <summary>
        ///     字节分析时的数据格式
        /// </summary>
        public int DataFormat { get; set; } = (int) HslCommunication.Core.DataFormat.ABCD;

        /// <summary>
        ///     字符串分析是否颠倒
        /// </summary>
        public bool IsStringReverse { get; set; }

        #endregion

        #region Override Method

        /// <summary>
        ///     获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns>键值数据对列表</returns>
        public override List<NodeClassRenderItem> GetNodeClassRenders() {
            var list = base.GetNodeClassRenders();
            list.Add(NodeClassRenderItem.CreateUniqueId(DTU));
            list.Add(NodeClassRenderItem.CreateStation(Station));
            list.Add(NodeClassRenderItem.CreateIsAddressStartWithZero(IsAddressStartWithZero));
            list.Add(NodeClassRenderItem.CreateDataFormat(DataFormat));
            list.Add(NodeClassRenderItem.CreateIsStringReverse(IsStringReverse));
            return list;
        }

        /// <summary>
        ///     对象解析为Xml元素，方便的存储
        /// </summary>
        /// <returns>包含节点信息的Xml元素</returns>
        public override XElement ToXmlElement() {
            var element = base.ToXmlElement();
            element.SetAttributeValue("DTU", DTU);
            element.SetAttributeValue("Station", Station);
            element.SetAttributeValue("IsAddressStartWithZero", IsAddressStartWithZero);
            element.SetAttributeValue("DataFormat", DataFormat);
            element.SetAttributeValue("IsStringReverse", IsStringReverse);
            return element;
        }


        /// <summary>
        ///     对象从xml元素解析，初始化指定的数据
        /// </summary>
        /// <param name="element">包含节点信息的Xml元素</param>
        public override void LoadByXmlElement(XElement element) {
            base.LoadByXmlElement(element);
            DTU = element.Attribute("DTU").Value;
            Station = byte.Parse(element.Attribute("Station").Value);
            IsAddressStartWithZero = bool.Parse(element.Attribute("IsAddressStartWithZero").Value);
            DataFormat = int.Parse(element.Attribute("DataFormat").Value);
            IsStringReverse = bool.Parse(element.Attribute("IsStringReverse").Value);
        }

        #endregion
    }
}