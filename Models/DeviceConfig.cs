namespace OpenIPC_Config.Models;

public class DeviceConfig
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }  
    public DeviceType DeviceType { get; set; }
    
}

public enum DeviceType
{
    None,
    Camera,
    Radxa,
    NVR
}