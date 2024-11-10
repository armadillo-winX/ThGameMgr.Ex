﻿using System.Collections.Generic;
using System.Reflection;

namespace ThGameMgr.Ex
{
    internal class PluginHandler
    {
        public static List<dynamic>? GameFilesPlugins { get; set; }

        public static void GetPlugin()
        {
            GameFilesPlugins = [];

            string[] pluginFiles = Directory.GetFiles(
                PathInfo.PluginDirectory, "*.dll", SearchOption.AllDirectories);

            foreach (string pluginFile in pluginFiles)
            {
                try
                {
                    Assembly? dllAssembly = Assembly.LoadFrom(pluginFile);

                    if (dllAssembly != null)
                    {
                        ClassifyPlugin(dllAssembly);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private static void ClassifyPlugin(Assembly dllAssembly)
        {
            //dllAssemblyの中のTypeをすべて取得し、プラグインのタイプがあるかチェックする
            foreach (Type type in dllAssembly.GetTypes())
            {
                //対象のクラスのインスタンスを作成
                dynamic plugin = Activator.CreateInstance(type);

                if (type.BaseType == typeof(GameFilesPluginBase))
                {
                    GameFilesPlugins.Add(plugin);
                }
            }
        }
    }
}
