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
using System.ComponentModel;
using System.Threading;
using PropertyChanged;
using System.Drawing;
using OxoBrowser.Wins;

namespace Base
{
    public enum GameTypeEnum
    {
        KanColle = 0,
        Touken
    }

    [AddINotifyPropertyChangedInterface]
    public class AppConfigData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ProxyIP { get; set; }
        public string ProxyPort { get; set; }
        public bool SoundClose { get; set; }
        public string WebSize { get; set; }

        public GameTypeEnum GameType { get; set; }


        public Size SizeWithGameType() {

            return GameType switch
            {
                GameTypeEnum.KanColle => new Size(1200, 720),
                GameTypeEnum.Touken => new Size(1136, 640),
                _ => new Size(1200, 720),
            };

        }

        public string UrlWithGameType()
        {
            switch (AppConfig.Instance.ConfigData.GameType)
            {
                case GameTypeEnum.KanColle:
                    return "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/";
                case GameTypeEnum.Touken:
                    return "https://pc-play.games.dmm.com/play/tohken/";
            }
            return "";

        }

    }

    /// <summary>
    /// 配置项读取、写入、存储逻辑
    /// </summary>
    public class AppConfig
    {
        private static readonly Lazy<AppConfig> lazy = new Lazy<AppConfig>(() => new AppConfig());

        public static AppConfig Instance => lazy.Value;

        public AppConfigData ConfigData { set; get; } = new AppConfigData();

        private string _configPath = AppContext.BaseDirectory + @"config.json";

        private object _lock = new object();

        private AppConfig()
        {
            Init();
            Debug.WriteLine(_configPath);
        }

        public void InitDefault() //载入默认配置
        {
            Debug.WriteLine($"默认初始化");
        }


        public bool Init()
        {
            try
            {

                Debug.WriteLine($"初始化配置" + Thread.CurrentThread.ManagedThreadId);
                if (File.Exists(_configPath) == false)
                {
                    InitDefault();
                    Save();
                }

                lock (_lock)
                {
                    var fileContent = File.ReadAllText(_configPath);
                    var appData = JsonConvert.DeserializeObject<AppConfigData>(fileContent);
                    ConfigData = appData;
                    ConfigData.PropertyChanged += AppConfigData_PropertyChanged;
                }


                return true;
            }
            catch (Exception ex)
            {
                InitDefault();
                Save();
                Debug.WriteLine(ex);
                return false;
            }
        }
        private void AppConfigData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            try
            {
                lock (_lock)
                {
                    var data = JsonConvert.SerializeObject(ConfigData, Formatting.Indented);
                    Debug.WriteLine($"存储配置{Thread.CurrentThread.ManagedThreadId} {data}");
                    File.WriteAllText(_configPath, data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
