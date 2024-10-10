using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;

namespace OpenIPC_Config;

public class SshClientService : ISshClientService
{
    public async Task ExecuteCommandAsync(string host, string username, string password, string command)
    {
        using (var client = new SshClient(host, username, password))
        {
            client.Connect();
            var result = client.RunCommand(command);
            client.Disconnect();
            // Process result or return output
        }
    }

    public async Task UploadFileAsync(string host, string username, string password, string localPath, string remotePath)
    {
        using (var client = new SftpClient(host, username, password))
        {
            client.Connect();
            using (var fileStream = new FileStream(localPath, FileMode.Open))
            {
                client.UploadFile(fileStream, remotePath);
            }
            client.Disconnect();
        }
    }

    public async Task DownloadFileAsync(string host, string username, string password, string remotePath, string localPath)
    {
        using (var client = new SftpClient(host, username, password))
        {
            client.Connect();
            using (var fileStream = new FileStream(localPath, FileMode.Create))
            {
                client.DownloadFile(remotePath, fileStream);
            }
            client.Disconnect();
        }
    }
}