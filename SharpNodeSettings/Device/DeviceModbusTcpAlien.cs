using System.Xml.Linq;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.ModBus;
using SharpNodeSettings.Node.Device;

namespace SharpNodeSettings.Device {
    /// <summary>
    ///     异形的Modbus-tcp的设备信息，包含了核心的连接对象
    /// </summary>
    public class DeviceModbusTcpAlien : DeviceCore {
        #region Private

        private readonly ModbusTcpNet modbusTcp; // 核心交互对象

        #endregion

        #region Constructor

        /// <summary>
        ///     实例化一个异形的Modbus-tcp的设备对象，从配置信息创建
        /// </summary>
        /// <param name="element">配置信息</param>
        public DeviceModbusTcpAlien(XElement element) {
            var modbusTcpAline = new NodeModbusTcpAline();
            modbusTcpAline.LoadByXmlElement(element);

            LoadRequest(element);

            modbusTcp = new ModbusTcpNet(string.Empty, 502, modbusTcpAline.Station);
            modbusTcp.AddressStartWithZero = modbusTcpAline.IsAddressStartWithZero;
            modbusTcp.ConnectionId = modbusTcpAline.DTU;
            modbusTcp.DataFormat = (DataFormat) modbusTcpAline.DataFormat;
            modbusTcp.IsStringReverse = modbusTcpAline.IsStringReverse;


            ByteTransform = modbusTcp.ByteTransform;
            ReadWriteDevice = modbusTcp;
            UniqueId = modbusTcp.ConnectionId;

            TypeName = "Modbus-Tcp异形设备";
        }

        #endregion


        #region Public Override

        /// <summary>
        ///     设置为异形客户端对象
        /// </summary>
        /// <param name="alienSession">异形对象</param>
        public override void SetAlineSession(AlienSession alienSession) {
            modbusTcp.ConnectServer(alienSession);
        }

        #endregion

        #region Protect Override

        /// <summary>
        ///     在启动之前进行的操作信息
        /// </summary>
        protected override void BeforStart() {
            modbusTcp.ConnectServer(null);
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