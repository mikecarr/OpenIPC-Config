namespace OpenIPC_Config.Models;

// This class contains strings of commands that can be executed on an OpenIPC camera 
public static class DeviceCommands
{
    public const string WfbStartCommand = "wifibroadcast start";
    public const  string WfbStopCommand = "wifibroadcast stop";
    public const  string WfbRestartCommand = "wifibroadcast stop; sleep 2; wifibroadcast start";
    
    public const string MajesticStartCommand = "majestic start";
    public const  string MajesticStopCommand = "majestic stop";
    
    public const string TelemetryStartCommand = "telemetry start";
    public const string TelemetryStopCommand = "telemetry stop";
    public const  string TelemetryRestartCommand = "telemetry stop; sleep 2; telemetry start";

    public const string UART0OnCommand =
        "sed -i 's/console::respawn:\\/sbin\\/getty -L console 0 vt100/#console::respawn:\\/sbin\\/getty -L console 0 vt100/' /etc/inittab";

    public const string UART0OffCommand =
        "sed -i 's/#console::respawn:\\/sbin\\/getty -L console 0 vt100/console::respawn:\\/sbin\\/getty -L console 0 vt100/' /etc/inittab";

    public const string RebootCommand = "reboot";
}