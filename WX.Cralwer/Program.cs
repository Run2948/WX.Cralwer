using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WX.Cralwer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 基本设置
            var settings = new CefSettings()
            {
                AcceptLanguageList = "zh-CN"
            //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
            //CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
        };
            // 设置是否使用GPU
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            // 设置是否使用代理服务
            settings.CefCommandLineArgs.Add("no-proxy-server", "1");
  
            // 初始化
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);           

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
