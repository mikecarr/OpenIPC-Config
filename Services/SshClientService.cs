using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OpenIPC_Config.ViewModels;
using Renci.SshNet;

namespace OpenIPC_Config
{
    public class SshClientService : ISshClientService
    {
        public async Task ExecuteCommandAsync(string host, string username, string password, string command)
        {
            Logger.Instance.Log($"Executing command: '{command}' on {host}.");

            using (var client = new SshClient(host, username, password))
            {
                try
                {
                    client.Connect();
                    var result = client.RunCommand(command);
                    Logger.Instance.Log($"Command executed successfully. Result: {result.Result}");
                    MainWindowViewModel.Instance?.AddLogMessage($"Command executed successfully. Result: {result.Result}");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log($"Error executing command: {ex.Message}");
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        public async Task UploadFileAsync(string host, string username, string password,  string remotePath, string fileContent)
        {
            Logger.Instance.Log($"Uploading content to '{remotePath}' on {host}.");
            

            await Task.Run(() =>
            {
                using (var client = new SftpClient(host, username, password))
                {
                    try
                    {
                        client.Connect();
                
                        // Convert the string content to a byte stream
                        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
                        {
                            client.UploadFile(memoryStream, remotePath);
                            Logger.Instance.Log("File uploaded successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Log($"Error uploading file: {ex.Message}");
                    }
                    finally
                    {
                        client.Disconnect();
                    }
                }
            });
        }


        public async Task<string> DownloadFileAsync(string host, string username, string password, string remotePath)
        {
            Logger.Instance.Log($"Downloading file from '{remotePath}' on {host}.");

            string fileContent = string.Empty;

            await Task.Run(() =>
            {
                using (var client = new SftpClient(host, username, password))
                {
                    try
                    {
                        client.Connect();
                        using (var memoryStream = new MemoryStream())
                        {
                            // Download the file content into a MemoryStream
                            client.DownloadFile(remotePath, memoryStream);
                            fileContent = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                            Logger.Instance.Log("File downloaded successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Log($"Error downloading file: {ex.Message}");
                    }
                    finally
                    {
                        client.Disconnect();
                    }
                }
            });

            return fileContent; // Return the content of the file
        }

    }
}
