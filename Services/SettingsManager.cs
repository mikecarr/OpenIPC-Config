using System.IO;
using Newtonsoft.Json;
using OpenIPC_Config.Models;


namespace OpenIPC_Config.Services;

public static class SettingsManager
{
    private static string settingsPath = "AppSettings.json";

    public static DeviceConfig LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            var json = File.ReadAllText(settingsPath);
            return JsonConvert.DeserializeObject<DeviceConfig>(json);
        }

        // Default values if no settings file exists
        return new DeviceConfig
        {
            IpAddress = "",
            Username = "",  
            Password = "",
            DeviceType = DeviceType.Camera
        };
    }

    public static void SaveSettings(DeviceConfig settings)
    {
        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(settingsPath, json);
    }
}