namespace OpenIPC_Config;

public class DeviceConfig
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }  
    
    public string DeviceType { get; set; }
    
    public DeviceConfig(string username, string password, string ipAddress, DeviceType deviceType)
    {
        Username = username;
        Password = password;
        IpAddress = ipAddress;
        DeviceType = deviceType.ToString();
    }
    
    
}

public enum DeviceType
{
    Camera,
    Radxa,
    NVR
}