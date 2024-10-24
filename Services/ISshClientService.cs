using System.Threading.Tasks;
using OpenIPC_Config.Models;

namespace OpenIPC_Config
{
    public interface ISshClientService
    {
        Task ExecuteCommandAsync(DeviceConfig deviceConfig, string command);

        // Uploads the file from a local path
        Task UploadFileAsync(DeviceConfig deviceConfig, string localFilePath, string remotePath);

        // Uploads a file by providing the content as a string
        Task UploadFileStringAsync(DeviceConfig deviceConfig, string remotePath, string fileContent);

        Task<string> DownloadFileAsync(DeviceConfig deviceConfig, string remotePath);

        Task UploadBinaryAsync(DeviceConfig deviceConfig, string remoteDirectory, string fileName);
        
        Task UploadBinaryAsync(DeviceConfig deviceConfig, string remoteDirectory, OpenIPC.FileType fileType, string fileName);
    }
}