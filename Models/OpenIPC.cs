namespace OpenIPC_Config.Models;

public class OpenIPC
{
    public const string MAJESTIC_FILE_LOC = "/etc/majestic.yaml";
    public const string WFB_CONF_FILE_LOC = "/etc/wfb.conf";
    public const string TELEMETRY_FILE_LOC = "/etc/telemetry.conf";
    
    public const string LOCAL_BINARIES_FOLDER = "binaries";
    public const string LOCAL_SENSORS_FOLDER = "binaries/sensors";
    public const string LOCAL_FONTS_FOLDER = "binaries/fonts";
    public const string LOCAL_BETAFLIGHT_FONTS_FOLDER = "binaries/fonts/bf";
    public const string LOCAL_INAV_FONTS_FOLDER = "binaries/fonts/inav";
    
    public const string REMOTE_ETC_FOLDER = "/etc";
    public const string REMOTE_BINARIES_FOLDER = "/usr/bin";
    public const string REMOTE_FONTS_FOLDER = "/usr/share/fonts/";
    
    public enum FileType
    {
        Normal,
        BetaFlightFonts,
        iNavFonts
    }
    
}


