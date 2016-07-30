using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Diagnostics;
using OxoBrowser;

namespace Base
{
    class AppConfigData
    {
        public string ProxyIP { get; set; }
        public string ProxyPort { get; set; }
    }

    /// <summary>
    /// elf配置项读取、写入、存储逻辑
    /// </summary>
    class AppConfig
    {
        public static AppConfigData m_config = new AppConfigData();
        public static string m_config_path = App.AppPath + @"\\config.json";
        const int def_save_config = 1;

        public static void InitDef() //载入默认配置
        {
            m_config.ProxyIP = "";
            m_config.ProxyPort = "";
        }

        public static bool Init()
        {
            try
            {
                if (File.Exists(m_config_path) == false)
                {
                    InitDef();
                    Save();
                    return false;
                }
                m_config = JsonConvert.DeserializeObject<AppConfigData>(System.IO.File.ReadAllText(m_config_path));
                return true;
            }
            catch (System.Exception ex)
            {
                InitDef();
                Save();
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static void Save()
        {
            try
            {
                System.IO.File.WriteAllText(m_config_path, JsonConvert.SerializeObject(m_config));
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }


    }
}
