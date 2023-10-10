using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using Rust;
using Facepunch;
using Network;

#region Changelogs and ToDo
/**********************************************************************
* 
* 1.0.0 :       -   Release
* 1.0.1 :       -   Added checks to see if server is fully started (THX @BMGJet for the tips)
*                              
**********************************************************************/
#endregion

namespace Oxide.Plugins
{
    [Info("ReloadAfterStartup", "Krungh Crow", "1.0.1")]
    [Description("Reload listed plugins after a restart")]

    class ReloadAfterStartup : RustPlugin
    {
        #region Variables
        Timer reloadtimer;
        #endregion

        #region Configuration
        void Init()
        {
            if (!LoadConfigVariables())
            {
            Puts("Config file issue detected. Please delete file, or check syntax and fix.");
            return;
            }
        }

        private ConfigData configData;

        class ConfigData
        {
            [JsonProperty(PropertyName = "Plugin settings")]
            public Settings PlugSettings = new Settings();
        }

        class Settings
        {
            [JsonProperty(PropertyName = "Time After Startup (seconds)")]
            public float StartR = 20f;
            [JsonProperty(PropertyName = "Time between Reloads (seconds)")]
            public float StartBR = 2f;
            [JsonProperty(PropertyName = "Plugins to Reload")]
            public List<string> ReloadList = new List<string>();
        }

        private bool LoadConfigVariables()
        {
            try
            {
            configData = Config.ReadObject<ConfigData>();
            }
            catch
            {
            return false;
            }
            SaveConf();
            return true;
        }

        protected override void LoadDefaultConfig()
        {
            Puts("Fresh install detected Creating a new config file.");
            configData = new ConfigData();
            SaveConf();
        }

        void SaveConf() => Config.WriteObject(configData, true);
        #endregion

        #region Hooks

        void OnServerInitialized(bool initial)
        {
            if(initial)
            {
                CheckInit();
                return;
            }
            Puts($"[Debugging] Server is already initialized skipping new reload sequence");
        }

        #endregion

        #region Core

        void CheckInit()
        {
            timer.Once(10f, () =>
            {
                if (Rust.Application.isLoading)
                {
                    Puts($"[Debugging] Server still loading....");
                    CheckInit();
                    return;
                }
                ReloadStart();
            });
        }

        void ReloadStart()
        {
            float TimeBetween = configData.PlugSettings.StartBR;
            float TimeAfter = configData.PlugSettings.StartR;
            List<string> plugs = configData.PlugSettings.ReloadList.ToList();
            timer.Once(TimeAfter, () =>
            {

                Puts($"Starting Reload sequence....");
                reloadtimer = timer.Repeat(TimeBetween, 0, () =>
                {
                    if (plugs.Count() > 0)
                    {
                        Puts($"Reloading {configData.PlugSettings.ReloadList.Count() - plugs.Count() + 1}/{configData.PlugSettings.ReloadList.Count()}");
                        Interface.Oxide.ReloadPlugin(plugs[0]);
                        plugs.RemoveAt(0);
                    }
                    else
                    {
                        reloadtimer.Destroy();
                        Puts($"Reload Checks Finished for {configData.PlugSettings.ReloadList.Count()} Plugin(s)....");
                        return;
                    }
                });
            });
        }
        #endregion
    }
}