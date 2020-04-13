﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SharpNodeSettings.Node.NodeBase;

namespace SharpNodeSettings.Node.Device {
    /// <summary>
    ///     常规的Modbus-Tcp的客户端
    /// </summary>
    public class NodeModbusTcpClient : DeviceNode, IXmlConvert {
        #region Constructor

        /// <summary>
        ///     实例化一个默认参数的对象
        /// </summary>
        public NodeModbusTcpClient() {
            CreateTime = DateTime.Now;
            DeviceType = ModbusTcpClient;

            Name = "ModbusTcp客户端";
            Description = "这是描述";
            IpAddress = "127.0.0.1";
            Port = 502;
            Station = 1;
        }

        #endregion

        #region Overide Method

        /// <summary>
        ///     获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns>键值数据对列表</returns>
        public override List<NodeClassRenderItem> GetNodeClassRenders() {
            var list = base.GetNodeClassRenders();
            list.Add(NodeClassRenderItem.CreateIpAddress(IpAddress));
            list.Add(NodeClassRenderItem.CreateIpPort(Port));
            list.Add(NodeClassRenderItem.CreateStation(Station));
            list.Add(NodeClassRenderItem.CreateIsAddressStartWithZero(IsAddressStartWithZero));
            list.Add(NodeClassRenderItem.CreateDataFormat(DataFormat));
            list.Add(NodeClassRenderItem.CreateIsStringReverse(IsStringReverse));
            return list;
        }

        #endregion

        #region Object Override

        /// <summary>
        ///     返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString() {
            return "[Modbus设备] " + Name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     设备的Ip地址
        /// </summary>
        public string IpAddress { get; set; }


        /// <summary>
        ///     设备的端口号
        /// </summary>
        public int Port { get; set; }


        /// <summary>
        ///     客户端的站号
        /// </summary>
        public byte Station { get; set; }


        /// <summary>
        ///     起始地址是否从0开始
        /// </summary>
        public bool IsAddressStartWithZero { get; set; } = true;

        /// <summary>
        ///     字节分析是否颠倒
        /// </summary>
        public int DataFormat { get; set; } = (int) HslCommunication.Core.DataFormat.DCBA;

        /// <summary>
        ///     字符串分析是否颠倒
        /// </summary>
        public bool IsStringReverse { get; set; }

        #endregion

        #region Xml Interface

        /// <summary>
        ///     对象从xml元素解析，初始化指定的数据
        /// </summary>
        /// <param name="element">包含节点信息的Xml元素</param>
        public override void LoadByXmlElement(XElement element) {
            base.LoadByXmlElement(element);
            IpAddress = element.Attribute("IpAddress").Value;
            Port = int.Parse(element.Attribute("Port").Value);
            Station = byte.Parse(element.Attribute("Station").Value);
            IsAddressStartWithZero = bool.Parse(element.Attribute("IsAddressStartWithZero").Value);
            DataFormat = int.Parse(element.Attribute("DataFormat").Value);
            IsStringReverse = bool.Parse(element.Attribute("IsStringReverse").Value);
        }

        /// <summary>
        ///     对象解析为Xml元素，方便的存储
        /// </summary>
        /// <returns>包含节点信息的Xml元素</returns>
        public override XElement ToXmlElement() {
            var element = base.ToXmlElement();
            element.SetAttributeValue("IpAddress", IpAddress);
            element.SetAttributeValue("Port", Port);
            element.SetAttributeValue("Station", Station);
            element.SetAttributeValue("IsAddressStartWithZero", IsAddressStartWithZero);
            element.SetAttributeValue("DataFormat", DataFormat);
            element.SetAttributeValue("IsStringReverse", IsStringReverse);
            return element;
        }

        #endregion
    }
}