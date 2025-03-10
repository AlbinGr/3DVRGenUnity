using UnityEngine;
using System.IO;
using Renci.SshNet;
using System;

public class Python : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    
    public string message = "Hello World!";
    public string host = "";
    public int port = 22;
    public string username = "";
    public string password = "";
    public string privateKeyPath = "";
    public string serverPythonDirectory = "Documents/VRPython";


    private ConnectionInfo connectionInfo = null;
    private SftpClient sftpClient = null;
    private  SshClient sshClient = null;
    private bool isConnected = false; 
    
    void Start()
    {
        // Create the connection info object
        if (privateKeyPath != "")
        {
            File.SetAttributes(privateKeyPath, FileAttributes.Normal);
            connectionInfo = new ConnectionInfo(host, port, username, new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile(privateKeyPath)));
        }
        else if (password != "")
        {
            connectionInfo = new ConnectionInfo(host, port, username, new PasswordAuthenticationMethod(username, password));
        }
        else
        {
            //connectionInfo = new ConnectionInfo(host, port, username, new PasswordAuthenticationMethod(username, password));
            UnityEngine.Debug.Log("No authentication method provided");
            return;
        }
        sftpClient = new SftpClient(connectionInfo);
        sftpClient.Connect();
        sshClient = new SshClient(connectionInfo);
        sshClient.Connect();
        if (sftpClient.IsConnected && sshClient.IsConnected)
        {
            UnityEngine.Debug.Log("Connected to the server");
            isConnected = true;
        }
        else
        {
            UnityEngine.Debug.Log("Failed to connect to the server");
            isConnected = false;
        }
        UnityEngine.Debug.Log("Is connected: ");
        UnityEngine.Debug.Log(isConnected);
        if (isConnected)
        {
            // Move to the python directory
            sftpClient.ChangeDirectory(serverPythonDirectory);

            UnityEngine.Debug.Log(sshClient.RunCommand("ls").Result);
            UnityEngine.Debug.Log(sshClient.RunCommand("pwd").Result);
            //SshCommand res1 = sshClient.RunCommand("cd Documents");
            UnityEngine.Debug.Log(sshClient.RunCommand("ls").Result);
            SshCommand res2 = sshClient.RunCommand("cd VRPython");
            show_python_error(res2);
            UnityEngine.Debug.Log(sshClient.RunCommand("ls").Result);
            UnityEngine.Debug.Log(sshClient.RunCommand("ls").Result);
        }
        else
        {
            UnityEngine.Debug.Log("Not connected to the server");
        }

        UnityEngine.Debug.Log("Running the python script");
        run_cmd();
    }

    private void run_cmd()
    {
        //string venvDir = @".venv\";
        string ms = "\"" + message + "\"";
        string prefix = "cd Documents/VRPython; ";
        string commandtxt = $"{prefix} .venv/bin/python test.py --message {ms}";
        UnityEngine.Debug.Log("Running command: " + commandtxt);
        
        SshCommand command = sshClient.CreateCommand(commandtxt);
        IAsyncResult asyncProcess = command.BeginExecute();
        string output = command.EndExecute(asyncProcess);
        UnityEngine.Debug.Log("Shell output: " + output);
        show_python_error(command);
        UnityEngine.Debug.Log("Command exit status: " + command.ExitStatus);
        UnityEngine.Debug.Log("Command output: " + command.Result);
        command.Dispose();
    }

    private void show_python_error(SshCommand command)
    {
        if (command.Error != "")
        {
            UnityEngine.Debug.Log("Python error: " + command.Error);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        
        if (sftpClient != null)
        {
            UnityEngine.Debug.Log("Disconnecting sftp from the server");
            sftpClient.Disconnect();
        }
        if (sshClient != null)
        {
            UnityEngine.Debug.Log("Disconnecting ssh from the server");
            sshClient.Disconnect();
        }

    }
}
