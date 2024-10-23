using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OpenIPC_Config.Models;
using Renci.SshNet;

namespace OpenIPC_Config
{
    public class SshClientService : ISshClientService
    {
        public  async Task ExecuteCommandAsync(DeviceConfig deviceConfig, string command)
        {
            Logger.Instance.Log($"Executing command: '{command}' on {deviceConfig.IpAddress}.");

            using (var client = new SshClient(deviceConfig.IpAddress, deviceConfig.Username, deviceConfig.Password))
            {
                try
                {
                    client.Connect();
                    var result = client.RunCommand(command);
                    //Logger.Instance.Log($"Command executed successfully. Result: {result.Result}");
                }
                catch (Exception ex)
                {
                    //Logger.Instance.Log($"Error executing command: {ex.Message}");
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        public async Task UploadFileAsync(DeviceConfig deviceConfig, string remotePath, string fileContent)
        {
            Logger.Instance.Log($"Uploading content to '{remotePath}' on {deviceConfig.IpAddress}.");

            await Task.Run(() =>
            {
                using (var client = new SftpClient(deviceConfig.IpAddress, deviceConfig.Username, deviceConfig.Password))
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


        public async Task<string> DownloadFileAsync(DeviceConfig deviceConfig, string remotePath)
        {
            Logger.Instance.Log($"Downloading file from '{remotePath}' on {deviceConfig.IpAddress}.");

            string fileContent = string.Empty;

            await Task.Run(() =>
            {
                using (var client = new SftpClient(deviceConfig.IpAddress, deviceConfig.Username, deviceConfig.Password))
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
