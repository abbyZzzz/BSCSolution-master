using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Advantech.AppCommon
{
    public class HardwareInfoHelper
    {
        /// <summary>
        /// 获取本机屏幕数量
        /// </summary>
        /// <returns></returns>
        public static int GetScreenCount()
        {
            //ScreenOrientation screenOrientation= ScreenOrientation.Angle0;
           return Screen.AllScreens.Count();
        }
        /// <summary>
        /// 获取本地ip
        /// </summary>
        /// <returns></returns>
        public static string GetMachineIp()
        {
            string AddressIP = string.Empty;
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if(adapter.Speed>0)
                {
                    System.Console.WriteLine("Id .................. : {0}", adapter.Id); // 获取网络适配器的标识符 
                    System.Console.WriteLine("Name ................ : {0}", adapter.Name); // 获取网络适配器的名称 
                    System.Console.WriteLine("Description ......... : {0}", adapter.Description); // 获取接口的描述 
                    System.Console.WriteLine("Interface type ...... : {0}", adapter.NetworkInterfaceType); // 获取接口类型 
                    System.Console.WriteLine("Is receive only...... : {0}", adapter.IsReceiveOnly); // 获取 Boolean 值，该值指示网络接口是否设置为仅接收数据包。 
                    System.Console.WriteLine("Multicast............ : {0}", adapter.SupportsMulticast); // 获取 Boolean 值，该值指示是否启用网络接口以接收多路广播数据包。 
                    System.Console.WriteLine("Speed ............... : {0}", adapter.Speed); // 网络接口的速度 
                    System.Console.WriteLine("Physical Address .... : {0}", adapter.GetPhysicalAddress().ToString()); // MAC 地址 

                    IPInterfaceProperties fIPInterfaceProperties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = fIPInterfaceProperties.UnicastAddresses;
                    foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                    {
                        if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            System.Console.WriteLine("Ip Address .......... : {0}", UnicastIPAddressInformation.Address); // Ip 地址 
                            AddressIP = UnicastIPAddressInformation.Address.ToString();
                        }
                    }
                }
                
            }
            if(string.IsNullOrEmpty(AddressIP))//未找到有效的网卡信息
            {
                var list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                    {
                        AddressIP = _IPAddress.ToString();
                    }
                }
            }
            return AddressIP;
        }

        /// <summary>
        /// 获取本机CPU序列号
        /// </summary>
        /// <returns></returns>
        public static string GetCPUSN()
        {
            string cpuInfo = string.Empty;//保存CPU 序列号            
            List<string> result = new List<string>();
            ManagementClass cimobject = new ManagementClass("Win32_Processor");  //通过WMI 中的"Win32_Processor类获取CPU ID
            ManagementObjectCollection moc = cimobject.GetInstances();  //返回所有类实例集合

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();  //取得CPU ID
                return cpuInfo;
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取本机CPU信息
        /// </summary>
        /// <returns></returns>
        public static string GetCPUInfo()
        {
            string cpuInfo = string.Empty;//保存CPU信息       
            List<string> result = new List<string>();
            ManagementClass cimobject = new ManagementClass("Win32_Processor");  //通过WMI 中的"Win32_Processor类获取CPU ID
            ManagementObjectCollection moc = cimobject.GetInstances();  //返回所有类实例集合

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["Manufacturer"].Value.ToString();  //
                cpuInfo += mo.Properties["Version"].Value.ToString();      //
                cpuInfo += mo.Properties["Name"].Value.ToString();     //
                return cpuInfo;
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取主板信息
        /// </summary>
        /// <returns></returns>
        public static string GetBoardInfo()
        {
            string boardInfo = string.Empty;//保存CPU信息       
            ManagementClass mc = new ManagementClass("Win32_BaseBoard");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                boardInfo = mo.Properties["SerialNumber"].Value.ToString();
                break;
            }
            return boardInfo;
        }
        /// <summary>
        /// 获取Bios信息
        /// </summary>
        /// <returns></returns>
        public static string GetBiosInfo()
        {
            string biosInfo = string.Empty;      
            
            ManagementClass mc = new ManagementClass("Win32_BIOS");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                biosInfo = mo.Properties["SerialNumber"].Value.ToString();
                break;
            }
            return biosInfo;
        }

        /// <summary>
        /// 获取系统内存大小
        /// </summary>
        /// <returns>内存大小（单位M）</returns>
        public static string GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();   //用于查询一些如系统信息的管理对象 
            searcher.Query = new SelectQuery(WindowsAPIType.Win32_PhysicalMemory.ToString(), "",
                                             new string[] { WindowsAPIKeys.Capacity.ToString() });//设置查询条件 
            ManagementObjectCollection collection = searcher.Get();   //获取内存容量 
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties[WindowsAPIKeys.Capacity.ToString()].Value != null)
                {
                    try
                    {
                        capacity += long.Parse(baseObj.Properties[WindowsAPIKeys.Capacity.ToString()].Value.ToString());
                    }
                    catch
                    {
                        return "查询失败";
                    }
                }
            }

            return ToGB((double)capacity, 1024.0);
        }
        /// <summary>
        /// 操作系统版本
        /// </summary>
        public static string GetOS_Version()
        {
            string str = "Windows 10";
            try
            {
                string hdId = string.Empty;
                ManagementClass hardDisk = new ManagementClass(WindowsAPIType.Win32_OperatingSystem.ToString());
                ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                foreach (ManagementObject m in hardDiskC)
                {
                    str = m[WindowsAPIKeys.Name.ToString()].ToString().Split('|')[0].Replace("Microsoft", "");
                    break;
                }
            }
            catch
            {

            }
            return str;
        }
        /// <summary>
        /// 获取分辨率
        /// </summary>
        public string GetFenbianlv()
        {
            string result = "1920*1080";
            try
            {
                string hdId = string.Empty;
                ManagementClass hardDisk = new ManagementClass(WindowsAPIType.Win32_DesktopMonitor.ToString());
                ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                foreach (ManagementObject m in hardDiskC)
                {
                    result = m[WindowsAPIKeys.ScreenWidth.ToString()].ToString() + "*" +
                             m[WindowsAPIKeys.ScreenHeight.ToString()].ToString();
                    break;
                }
            }
            catch
            {

            }
            return result;
        }
        /// <summary>
        /// 获取硬盘容量
        /// </summary>
        public string GetDiskSize()
        {
            string result = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                string hdId = string.Empty;
                ManagementClass hardDisk = new ManagementClass(WindowsAPIType.win32_DiskDrive.ToString());
                ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
                foreach (ManagementObject m in hardDiskC)
                {
                    long capacity = Convert.ToInt64(m[WindowsAPIKeys.Size.ToString()].ToString());
                    sb.Append(ToGB(capacity, 1000.0) + "+");
                }
                result = sb.ToString().TrimEnd('+');
            }
            catch
            {

            }
            return result;
        }
        /// <summary>  
        /// 将字节转换为GB
        /// </summary>  
        /// <param name="size">字节值</param>  
        /// <param name="mod">除数，硬盘除以1000，内存除以1024</param>  
        /// <returns></returns>  
        public static string ToGB(double size, double mod)
        {
            String[] units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return Math.Round(size) + units[i];
        }
    }

    public enum WindowsAPIKeys
    {
        /// <summary>
        /// 名称
        /// </summary>
        Name,
        /// <summary>
        /// 显卡芯片
        /// </summary>
        VideoProcessor,
        /// <summary>
        /// 显存大小
        /// </summary>
        AdapterRAM,
        /// <summary>
        /// 分辨率宽
        /// </summary>
        ScreenWidth,
        /// <summary>
        /// 分辨率高
        /// </summary>
        ScreenHeight,
        /// <summary>
        /// 电脑型号
        /// </summary>
        Version,
        /// <summary>
        /// 硬盘容量
        /// </summary>
        Size,
        /// <summary>
        /// 内存容量
        /// </summary>
        Capacity,
        /// <summary>
        /// cpu核心数
        /// </summary>
        NumberOfCores
    }
    /// <summary>
    /// windows api 名称
    /// </summary>
    public enum WindowsAPIType
    {
        /// <summary>
        /// 内存
        /// </summary>
        Win32_PhysicalMemory,
        /// <summary>
        /// cpu
        /// </summary>
        Win32_Processor,
        /// <summary>
        /// 硬盘
        /// </summary>
        win32_DiskDrive,
        /// <summary>
        /// 电脑型号
        /// </summary>
        Win32_ComputerSystemProduct,
        /// <summary>
        /// 分辨率
        /// </summary>
        Win32_DesktopMonitor,
        /// <summary>
        /// 显卡
        /// </summary>
        Win32_VideoController,
        /// <summary>
        /// 操作系统
        /// </summary>
        Win32_OperatingSystem

    }
}
