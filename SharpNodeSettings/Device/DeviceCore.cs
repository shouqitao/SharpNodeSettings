using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.LogNet;
using Newtonsoft.Json.Linq;
using SharpNodeSettings.Node.Regular;
using SharpNodeSettings.Node.Request;

namespace SharpNodeSettings.Device {
    /// <summary>
    ///     设备交互的核心类对象
    /// </summary>
    public class DeviceCore {
        #region Constructor

        /// <summary>
        ///     使用默认的无参构造方法
        /// </summary>
        public DeviceCore() {
            ActiveTime = DateTime.Now.AddDays(-1);
            autoResetQuit = new AutoResetEvent(false);
            JObjectData = new JObject();
            JsonData = JObjectData.ToString();
            jsonLock = new SimpleHybirdLock();
            dictDynamicValues = new Dictionary<string, dynamic>();
            dictLock = new SimpleHybirdLock();
        }

        #endregion

        #region Protect Method

        /// <summary>
        ///     使用固定的节点加载数据信息
        /// </summary>
        /// <param name="element">数据请求的所有列表信息</param>
        protected void LoadRequest(XElement element) {
            Requests = new List<DeviceRequest>();
            foreach (var item in element.Elements("DeviceRequest")) {
                var request = new DeviceRequest();
                request.LoadByXmlElement(item);
                Requests.Add(request);
            }
        }

        #endregion

        #region Thread Read

        private void ThreadReadBackground() {
            Thread.Sleep(1000); // 默认休息一下下
            BeforStart(); // 需要子类重写

            while (isQuit == 0) {
                Thread.Sleep(100);

                var isDataChange = false; // 数据是否发生了变化
                foreach (var Request in Requests)
                    if ((DateTime.Now - Request.LastActiveTime).TotalMilliseconds > Request.CaptureInterval) {
                        Request.LastActiveTime = DateTime.Now;

                        var read = ReadWriteDevice.Read(Request.Address, Request.Length);
                        if (read.IsSuccess) {
                            IsError = false;
                            isDataChange = true;
                            ParseFromRequest(read.Content, Request);
                            ActiveTime = DateTime.Now;
                            RequestSuccessCount++;
                        } else {
                            IsError = true;
                            RequestFailedCount++;
                        }
                    }

                // 更新Json字符串缓存
                jsonLock.Enter();
                if (isDataChange) JsonData = JObjectData.ToString();
                jsonLock.Leave();
            }


            AfterClose(); // 需要子类重写
            autoResetQuit.Set(); // 通知关闭的线程继续
        }

        #endregion

        #region IDeviceCore Properties

        /// <summary>
        ///     设备分布的信息点
        /// </summary>
        public string[] DeviceNodes { get; set; }

        /// <summary>
        ///     所有的请求列表
        /// </summary>
        public List<DeviceRequest> Requests { get; set; }

        /// <summary>
        ///     数据转换规则
        /// </summary>
        public IByteTransform ByteTransform { get; set; }

        /// <summary>
        ///     当前的数据读写信息
        /// </summary>
        public IReadWriteNet ReadWriteDevice { get; set; }

        /// <summary>
        ///     指示读取到数据后应该如何处理
        /// </summary>
        public Action<DeviceCore, string> WriteCustomerData { get; set; }


        /// <summary>
        ///     设备上次激活的时间节点，用来判断失效状态
        /// </summary>
        public DateTime ActiveTime { get; set; }

        /// <summary>
        ///     唯一的识别码，方便异形客户端寻找对应的处理逻辑
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        ///     设备的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     请求成功的次数统计
        /// </summary>
        public long RequestSuccessCount { get; set; }

        /// <summary>
        ///     请求失败的次数统计
        /// </summary>
        public long RequestFailedCount { get; set; }

        /// <summary>
        ///     类型名称
        /// </summary>
        public string TypeName { get; set; }


        /// <summary>
        ///     指示设备是否正常的状态
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        ///     获取本设备所有的属性数据
        /// </summary>
        public string JsonData { get; private set; } = string.Empty;

        /// <summary>
        ///     获取或设置系统的状态
        /// </summary>
        public ILogNet LogNet { get; set; }

        #endregion

        #region Public Method

        /// <summary>
        ///     启动读取数据
        /// </summary>
        public void StartRead() {
            if (Interlocked.CompareExchange(ref isStarted, 1, 0) == 0) {
                thread = new Thread(ThreadReadBackground);
                thread.IsBackground = true;
                thread.Priority = ThreadPriority.AboveNormal;
                thread.Start();
            }
        }

        /// <summary>
        ///     退出系统
        /// </summary>
        public void QuitDevice() {
            if (isStarted == 1) {
                isQuit = 1;
                autoResetQuit.WaitOne();
            }
        }

        /// <summary>
        ///     设置为异形客户端对象
        /// </summary>
        /// <param name="alienSession">异形对象</param>
        public virtual void SetAlineSession(AlienSession alienSession) { }

        /// <summary>
        ///     通过节点值名称，获取本设备信息的值
        /// </summary>
        /// <param name="name">节点值名称</param>
        /// <returns>动态值</returns>
        public dynamic GetDynamicValueByName(string name) {
            dynamic value = null;
            dictLock.Enter();
            if (dictDynamicValues.ContainsKey(name)) value = dictDynamicValues[name];
            dictLock.Leave();
            return value;
        }

        /// <summary>
        ///     通过节点值名称，获取本设备信息的值
        /// </summary>
        /// <param name="name">节点值名称</param>
        /// <returns>JSON值</returns>
        public JToken GetJsonValueByName(string name) {
            JToken result = null;
            jsonLock.Enter();
            if (JObjectData.ContainsKey(name)) result = JObjectData[name];
            jsonLock.Leave();
            return result;
        }

        /// <summary>
        ///     通过节点值名称，获取本设备信息的值
        /// </summary>
        /// <param name="name">节点值名称</param>
        /// <returns>动态值</returns>
        public string GetStringValueByName(string name) {
            var jToken = GetJsonValueByName(name);
            if (jToken == null) return string.Empty;
            return jToken.ToString();
        }

        /// <summary>
        ///     判断当前的设备是否是传入的节点参数信息
        /// </summary>
        /// <param name="nodes">传入的节点参数信息</param>
        /// <returns>是否是当前的设备</returns>
        public bool IsCurrentDevice(string[] nodes) {
            if (DeviceNodes != null && nodes != null) {
                if (nodes.Length < DeviceNodes.Length) return false;
                for (var i = 0; i < DeviceNodes.Length; i++)
                    if (DeviceNodes[i] != nodes[i])
                        return false;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     获取本设备对象的值信息
        /// </summary>
        /// <param name="nodes">节点数据</param>
        /// <returns>值信息数据</returns>
        public string GetValueByName(string[] nodes) {
            if (nodes.Length == DeviceNodes.Length)
                return JsonData;
            if (nodes.Length > DeviceNodes.Length)
                return GetStringValueByName(nodes[DeviceNodes.Length]);
            return string.Empty;
        }

        #endregion

        #region Virtual Method

        /// <summary>
        ///     在启动之前进行的操作信息
        /// </summary>
        protected virtual void BeforStart() { }

        /// <summary>
        ///     在关闭的时候需要进行的操作
        /// </summary>
        protected virtual void AfterClose() { }

        #endregion

        #region Private Member

        private Thread thread; // 后台读取的线程
        private int isStarted; // 是否启动了后台数据读取
        private readonly AutoResetEvent autoResetQuit; // 退出系统的时候的同步锁
        private int isQuit; // 是否准备从系统进行退出
        private readonly JObject JObjectData; // JSON数据中心
        private readonly Dictionary<string, dynamic> dictDynamicValues; // 系统缓存的实际数据值
        private readonly SimpleHybirdLock jsonLock; // JSON对象的安全锁
        private readonly SimpleHybirdLock dictLock; // dict词典的数据锁

        #endregion

        #region JSON Object

        private void ParseFromRequest(byte[] data, DeviceRequest request) {
            foreach (var regular in request.RegularNodes) {
                var value = regular.GetValue(data, ByteTransform);

                jsonLock.Enter();
                if (regular.RegularCode != RegularNodeTypeItem.StringAscii.Code &&
                    regular.RegularCode != RegularNodeTypeItem.StringUnicode.Code &&
                    regular.RegularCode != RegularNodeTypeItem.StringUtf8.Code &&
                    regular.TypeLength > 1)
                    // 数组
                    JObjectData[regular.Name] = new JArray(value);
                else
                    // 单个的值
                    JObjectData[regular.Name] = new JValue(value);
                jsonLock.Leave();
                SetDictValue(regular.Name, value);
                WriteCustomerData?.Invoke(this, regular.Name);
            }
        }


        private void SetDictValue(string name, dynamic value) {
            dictLock.Enter();
            if (dictDynamicValues.ContainsKey(name))
                dictDynamicValues[name] = value;
            else
                dictDynamicValues.Add(name, value);
            dictLock.Leave();
        }

        #endregion
    }
}