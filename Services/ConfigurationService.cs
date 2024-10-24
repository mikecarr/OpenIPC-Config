using System;
using System.Collections.Generic;
using OpenIPC_Config.Models;

namespace OpenIPC_Config;

public class ConfigurationService
{
    private Dictionary<string, DeviceConfig> _deviceConfigs;

    public ConfigurationService()
    {
       
    }

    public DeviceConfig GetConfig(string deviceType)
    {
        // TODO: local config from file
        // if (string.IsNullOrEmpty(deviceType))
        // {
        //     throw new ArgumentNullException(nameof(deviceType), "Device type cannot be null or empty.");
        // }
        //
        // if (!_deviceConfigs.TryGetValue(deviceType, out var config))
        // {
        //     throw new KeyNotFoundException($"No configuration found for device type: {deviceType}");
        // }
        // return config;
        return null;
    }

    public void SaveConfig(DeviceConfig config)
    {
        // TODO: save config from file
        //_deviceConfigs[deviceType] = config;
        // Optionally, save to a file or database for persistence
    }
    
    // Dim fileExists As Boolean = File.Exists(settingsconf)
    // Using sw As New StreamWriter(File.Open(settingsconf, FileMode.OpenOrCreate))
    // sw.WriteLine("openipc:192.168.0.1")
    //     sw.WriteLine("nvr:192.168.0.1")
    // sw.WriteLine("radxa:192.168.0.1")
    //     End Using
    //     MsgBox("File " + settingsconf + " not found and default created!")
}