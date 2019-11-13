using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace SettlementSimulation.Viewer.Helpers
{
    public static class ConfigurationManagerHelper
    {
        public static string GetSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void SetSettings(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = config.AppSettings.Settings[key];
            if (entry == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;

            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
