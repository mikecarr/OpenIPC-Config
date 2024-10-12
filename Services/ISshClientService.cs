using System.Threading.Tasks;

namespace OpenIPC_Config;

public interface ISshClientService
{
    Task ExecuteCommandAsync(string host, string username, string password, string command);
    Task UploadFileAsync(string host, string username, string password, string fileContent, string remotePath);
    Task<string> DownloadFileAsync(string host, string username, string password, string remotePath);
}