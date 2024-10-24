using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIPC_Config.Models;
using Prism.Events;
using Renci.SshNet;


namespace OpenIPC_Config
{
    public class SshClientService : ISshClientService
    {
        private IEventAggregator _eventAggregator;
        
        // Set up tracing
        
        
        public SshClientService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        
        public async Task ExecuteCommandAsync(DeviceConfig deviceConfig, string command)
        {
            Logger.Instance().Log($"Executing command: '{command}' on {deviceConfig.IpAddress}.");

            using (var client = new SshClient(deviceConfig.IpAddress, deviceConfig.Username, deviceConfig.Password))
            {
                try
                {
                    client.Connect();
                    var result = client.RunCommand(command);
                    Logger.Instance().Log($"Command executed successfully. Result: {result.Result}");
                }
                catch (Exception ex)
                {
                    Logger.Instance().Log($"Error executing command: {ex.Message}");
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        public async Task UploadFileStringAsync(DeviceConfig deviceConfig, string remotePath, string fileContent)
        {
            Logger.Instance().Log($"Uploading content to '{remotePath}' on {deviceConfig.IpAddress}.");

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
                            Logger.Instance().Log("File uploaded successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance().Log($"Error uploading file: {ex.Message}");
                    }
                    finally
                    {
                        client.Disconnect();
                    }
                }
            });
        }

        // Version that uploads a file from a local path
        public async Task UploadFileAsync(DeviceConfig deviceConfig, string localFilePath, string remotePath)
        {
            Logger.Instance().Log($"Uploading file '{localFilePath}' to '{remotePath}' on {deviceConfig.IpAddress}.");

            var traceSource = new TraceSource("Renci.SshNet", SourceLevels.Verbose);
            traceSource.Listeners.Add(new ConsoleTraceListener());
            traceSource.Switch.Level = SourceLevels.All;

            
            await Task.Run(() =>
            {
                using (var client = new SftpClient(deviceConfig.IpAddress, deviceConfig.Username, deviceConfig.Password))
                {
                    try
                    {
                        client.Connect();

                        // Use a FileStream to read the file from disk and upload it
                        using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                        {
                            client.UploadFile(fileStream, remotePath);
                            Logger.Instance().Log("File uploaded successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance().Log($"Error uploading file: {ex.Message}");
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
            Logger.Instance().Log($"Downloading file from '{remotePath}' on {deviceConfig.IpAddress}.");

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
                            Logger.Instance().Log("File downloaded successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance().Log($"Error downloading file: {ex.Message}");
                    }
                    finally
                    {
                        client.Disconnect();
                    }
                }
            });

            return fileContent; // Return the content of the file
        }

    
    
        public async Task UploadBinaryAsync(DeviceConfig deviceConfig, string remoteDirectory, string fileName)
        {
            string binariesFolderPath = Path.Combine(Environment.CurrentDirectory, "binaries");
            string filePath = Path.Combine(binariesFolderPath, fileName);

            if (File.Exists(filePath))
            {
                string remoteFilePath = Path.Combine(remoteDirectory, fileName);
                Console.WriteLine($"Uploading {fileName} to {remoteFilePath}...");
                await UploadFileAsync(deviceConfig, filePath, remoteFilePath);
                Console.WriteLine($"Uploaded {fileName} successfully.");
            }
            else
            {
                Console.WriteLine($"File {fileName} not found in binaries folder.");
            }
        }
}
}
