using System;
using HslCommunication.LogNet;
using SharpNodeSettings.Core;

namespace SharpNodeSettings.SampleServer {
    internal class Program {
        private static void Main(string[] args) {
            // 创建日志
            ILogNet logNet = new LogNetSingle("log.txt");
            logNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;

            var sharpNodeServer = new SharpNodeServer {LogNet = logNet};
            sharpNodeServer.LoadByXmlFile("settings.xml");
            sharpNodeServer.ServerStart(12345);


            Console.ReadLine();
        }

        private static void LogNet_BeforeSaveToFile(object sender, HslEventArgs e) {
            Console.WriteLine(e.HslMessage.ToString());
        }
    }
}