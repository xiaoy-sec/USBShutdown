using System;
using System.Diagnostics;
using System.Management;

namespace USBShutdown
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");
            watcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            watcher.Query = query;
            watcher.Start();

            // 创建一个新的WMI查询，用于检测USB设备的拔出事件
            ManagementEventWatcher removalWatcher = new ManagementEventWatcher();
            WqlEventQuery removalQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");
            removalWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removalWatcher.Query = removalQuery;
            removalWatcher.Start();

            Console.WriteLine("USB监控程序已启动，按任意键停止监控...");
            Console.ReadKey();

            // 停止监控
            watcher.Stop();
            removalWatcher.Stop();
        }
        static void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("检测到USB设备插入");
            Process.Start("shutdown", "/s /t 0");
        }

        static void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("检测到USB设备拔出");
            Process.Start("shutdown", "/s /t 0");
        }
    }
}
