using System;
using System.Collections.Generic;

namespace OpenIPC_Config;

public class ConfigurationService
{
    private Dictionary<string, DeviceConfig> _deviceConfigs;

    public ConfigurationService()
    {
        _deviceConfigs = new Dictionary<string, DeviceConfig>
        {
            { "Camera", new DeviceConfig() },
            { "Radxa", new DeviceConfig() },
            { "NVR", new DeviceConfig() }
        };
    }

    public DeviceConfig GetConfig(string deviceType)
    {
        if (string.IsNullOrEmpty(deviceType))
        {
            throw new ArgumentNullException(nameof(deviceType), "Device type cannot be null or empty.");
        }

        if (!_deviceConfigs.TryGetValue(deviceType, out var config))
        {
            throw new KeyNotFoundException($"No configuration found for device type: {deviceType}");
        }
        return config;
    }

    public void SaveConfig(string deviceType, DeviceConfig config)
    {
        _deviceConfigs[deviceType] = config;
        // Optionally, save to a file or database for persistence
    }
}