using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Management;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace BatterySaverAppKiller
{
    class Program
    {
        public static string path;
        public static Dictionary<string, string> ClosedApps;
        public static bool isDying = false;
        public static List<string> killMes;
        public static int batteryPercentage;

        static void Main(string[] args)
        {
            //INIT
            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            CreateProcessesListFile();
            JObject jObject = JObject.Parse(File.ReadAllText(path + "/AppConfig.json"));
            batteryPercentage = (int)jObject["batteryPercentage"];
            killMes = new List<string>();
            foreach (object name in jObject["processNames"].ToList())
                killMes.Add(name.ToString());
            ClosedApps = new Dictionary<string, string>();

            //Keep running
            while (true)
            {
                if (SystemInformation.PowerStatus.BatteryLifePercent * 100 > batteryPercentage || SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.Charging && isDying)
                {
                    //Restore apps
                    isDying = false;
                    RestoreClosedApps(ClosedApps);
                }
                else if(SystemInformation.PowerStatus.BatteryLifePercent * 100 <= batteryPercentage && !isDying)
                {
                    //Kill apps
                    isDying = true;
                    foreach (string process in killMes)
                    {
                        CloseApp(process);
                    }
                }
            }
        }

        static void CloseApp(string appName)
        {
            Process[] processes = Process.GetProcessesByName(appName);
            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    if (!ClosedApps.ContainsKey(processes[0].ProcessName))
                    {
                        ClosedApps.Add(processes[0].ProcessName, GetMainModuleFilepath(processes[0].Id));
                        Console.WriteLine("Kill: " + processes[0].ProcessName);
                    }
                    process.Kill();
                }
            }
        }

        static void RestoreClosedApps(Dictionary<string, string> dict)
        {
            for (int i = 0; i < dict.Count; i++)
            {
                if (Process.GetProcessesByName(dict.Keys.ElementAt(i)).Length == 0)
                {
                    Console.WriteLine("Start: " + dict.Keys.ElementAt(i));
                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo(dict.Values.ElementAt(i));
                    process.StartInfo.WorkingDirectory = Path.GetDirectoryName(dict.Values.ElementAt(i));
                    process.Start();
                }
            }
        }

        static string GetMainModuleFilepath(int processId)
        {
            string wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            {
                using (var results = searcher.Get())
                {
                    ManagementObject mo = results.Cast<ManagementObject>().FirstOrDefault();
                    if (mo != null)
                    {
                        return (string)mo["ExecutablePath"];
                    }
                }
            }
            return null;
        }

        static void CreateProcessesListFile()
        {
            Process[] list = Process.GetProcesses();
            string text = "";
            foreach (Process process in list)
                text = text + process.ProcessName + "\n";
            File.WriteAllText(path + "/RunningProcesses.txt", text);
        }
    }
}
