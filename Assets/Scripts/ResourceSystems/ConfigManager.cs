using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

using UnityEngine;

namespace ResourceSystems
{
    public static class ConfigManager
    {
        /// <summary>
        /// Корневая папка с ресурсами Disk://...//RootFolder//
        /// </summary>
        public static string RootFolder { get; private set; }

        /// <summary>
        /// Папка с ассетбандлами (оканчивается на //)
        /// </summary>
        public static string AssetBundlesFolder { get; private set; }

        public static void Setup(ConfigSetup configSetup)
        {
            if (configSetup.ExecutionRoot == true)
            {
                RootFolder = Application.dataPath+"//";
                Debug.Log(RootFolder);
            }
            else
            {
                RootFolder = "D://Development//Syntech//";
            }

            var configPath = RootFolder + "config.txt";
            if (File.Exists(configPath))
            {
                // Загрузка конфига
                FileStream fs = new FileStream(configPath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);

                var content = sr.ReadToEnd();

                var reg = new Regex("assetbundlesfolder: \"(\\S+)\"",RegexOptions.IgnorePatternWhitespace);
                AssetBundlesFolder = reg.Match(content).Groups[1].Value;
            }
            else
            {
                throw new System.Exception(configPath + " не существует");
            }
        }
    }

    public class ConfigSetup
    {
        /// <summary>
        /// Корневая папка будет папкой запуска игры 
        /// </summary>
        public bool ExecutionRoot;
    }
}

