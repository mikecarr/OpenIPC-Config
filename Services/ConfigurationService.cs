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
}