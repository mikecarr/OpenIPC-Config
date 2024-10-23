namespace OpenIPC_Config.Models;

// This class contains strings of commands that can be executed on an OpenIPC camera 
public static class DeviceCommands
{
    public const string WfbStartCommand = "wifibroadcast start";
    public const  string WfbStopCommand = "wifibroadcast stop";
    public const  string WfbRestartCommand = "wifibroadcast stop; sleep 2; wifibroadcast start";
    
    public const string MajesticStartCommand = "majestic start";
    public const  string MajesticStopCommand = "majestic stop";
    
}