using System.Xml.Linq;
using HslCommunication.Core;
using HslCommunication.ModBus;
using SharpNodeSettings.Node.Device;

namespace SharpNodeSettings.Device {
    /// <summary>
    ///     Modbus-Tcp的设备信息，包含了核心的连接对象
    /// </summary>
    public class DeviceModbusTcp : DeviceCore {
        #region Private

        private readonly ModbusTcpNet modbusTcp; // 核心交互对象

        #endregion

        #region Constructor

        /// <summary>
        ///     实例化一个Modbus-Tcp的设备对象，从配置信息创建
        /// </summary>
        /// <param name="element">配置信息</param>
        public DeviceModbusTcp(XElement element) {
            var nodeModbus = new NodeModbusTcpClient();
            nodeModbus.LoadByXmlElement(element);
            LoadRequest(element);

            modbusTcp = new ModbusTcpNet(nodeModbus.IpAddress, nodeModbus.Port, nodeModbus.Station);
            modbusTcp.AddressStartWithZero = nodeModbus.IsAddressStartWithZero;
            modbusTcp.ConnectTimeOut = nodeModbus.ConnectTimeOut;
            modbusTcp.DataFormat = (DataFormat) nodeModbus.DataFormat;
            modbusTcp.IsStringReverse = nodeModbus.IsStringReverse;

            ByteTransform = modbusTcp.ByteTransform;
            UniqueId = modbusTcp.ConnectionId;
            ReadWriteDevice = modbusTcp;

            TypeName = "Modbus-Tcp设备";
        }

        #endregion

        #region Protect Override

        /// <summary>
        ///     在启动之前进行的操作信息
        /// </summary>
        protected override void BeforStart() {
            modbusTcp.SetPersistentConnection();
        }


        /// <summary>
        ///     在关闭的时候需要进行的操作
        /// </summary>
        protected override void AfterClose() {
            modbusTcp.ConnectClose();
        }

        #endregion
    }
}