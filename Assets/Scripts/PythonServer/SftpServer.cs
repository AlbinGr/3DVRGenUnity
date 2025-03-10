using UnityEngine;
using Renci.SshNet;
using System.IO;
using Renci.SshNet.Sftp;
using System.Collections.Generic;
using System;
public class SftpServer : MonoBehaviour
{
    private SftpClient sftpClient = null;
    private ServerParams serverParams;
    void Start()
    {
        serverParams = GetComponent<ServerParams>();
        sftpClient = new SftpClient(serverParams.connectionInfo);
        sftpClient.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DownloadFile(string serverFilename, string localFilename)
    {
        if (sftpClient.IsConnected)
        {
            using (var fileStream = new FileStream(localFilename, FileMode.Create))
            {
                UnityEngine.Debug.Log("Downloading file " + serverFilename);
                sftpClient.DownloadFile(serverFilename, fileStream);  
            }
        }
        else
        {
            UnityEngine.Debug.Log("Tried to get a file but server not connected");
        }
    }

    public void DownloadDirectory(string sourceRemotePath, string destLocalPath)
    {
        Directory.CreateDirectory(destLocalPath);
        IEnumerable<ISftpFile> files = sftpClient.ListDirectory(sourceRemotePath);
        foreach (ISftpFile file in files)
        {
            if ((file.Name != ".") && (file.Name != ".."))
            {
                string sourceFilePath = sourceRemotePath + "/" + file.Name;
                string destFilePath = Path.Combine(destLocalPath, file.Name);
                if (file.IsDirectory)
                {
                    DownloadDirectory(sourceFilePath, destFilePath);
                }
                else
                {
                    DownloadFile(sourceFilePath, destFilePath);
                }
            }
        }
    }

    public void UploadFile(string localFilename, string serverFilename)
    {
        if (sftpClient.IsConnected)
        {
            using (var fileStream = new FileStream(localFilename, FileMode.Open))
            {
                UnityEngine.Debug.Log("Uploading file " + serverFilename);
                sftpClient.UploadFile(fileStream, serverFilename);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Tried to upload a file but server not connected");
        }
    }
    
    public void DeleteDirectory(string path)
    {
        if (sftpClient.IsConnected)
        {
           
            if (path.EndsWith("..") || path.EndsWith("."))
            {
                return;
            }
            UnityEngine.Debug.Log("Deleting directory at " + path);
            sftpClient.DeleteDirectory(path);
        }
        else
        {
            UnityEngine.Debug.Log("Tried to delete a directory but server not connected");
        }
    }

    public void DeleteFile(string path)
    {
        if (sftpClient.IsConnected)
        {
            
            // Remove the case path end with .. and . 
            if (path.EndsWith("..") || path.EndsWith("."))
            {
                return;
            }
            UnityEngine.Debug.Log("Deleting file at " + path);
            sftpClient.DeleteFile(path);
        }
        else
        {
            UnityEngine.Debug.Log("Tried to delete a file but server not connected");
        }
    }
    
    public IEnumerable<ISftpFile> SftpListDirectory(string path)
    {
        if (sftpClient.IsConnected)
        {
            UnityEngine.Debug.Log("Listing directory at " + serverParams.serverDirectory + $"/{path}");
            IEnumerable<ISftpFile> files = sftpClient.ListDirectory(serverParams.serverDirectory + $"/{path}");
            return files;
        }
        else
        {
            return null;
        }
    }
    void OnApplicationQuit()
    {
        if (sftpClient != null)
        {
            UnityEngine.Debug.Log("Disconnecting sftp from the server");
            sftpClient.Disconnect();
        }
    }

    
}
