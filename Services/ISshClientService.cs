using System.Threading.Tasks;
using OpenIPC_Config.Models;

namespace OpenIPC_Config;

public interface ISshClientService
{
    Task ExecuteCommandAsync(DeviceConfig deviceConfig, string command);
    Task UploadFileAsync(DeviceConfig deviceConfig, string fileContent, string remotePath);
    Task<string> DownloadFileAsync(DeviceConfig deviceConfig, string remotePath);
}