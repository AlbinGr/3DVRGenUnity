using UnityEngine;
using Renci.SshNet;
using System;
using System.Threading.Tasks;
public class SshServer : MonoBehaviour
{
    private SshClient sshClient = null;
    private ServerParams serverParams;
    
    void Start()
    {   
        serverParams = GetComponent<ServerParams>();
        sshClient = new SshClient(serverParams.connectionInfo);
        sshClient.Connect();
        if (serverParams.dockerName != "");
        {
            // Start the docker container
            using (var client = new SshClient(serverParams.connectionInfo))
            {
                client.Connect();
                var cmd = client.RunCommand("sudo docker-start.sh " + serverParams.dockerName);
                UnityEngine.Debug.Log(cmd.Result);
                client.Disconnect();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteCommand(string commandtxt, bool python = true)
    {   
        
        if (sshClient.IsConnected)
        {
            if (python)
            {
                commandtxt = "cd " + serverParams.serverDirectory + "; .venv/bin/python " + commandtxt;
            }
            else
            {
                commandtxt = "cd " + serverParams.serverDirectory + "; " + commandtxt;
            }
            UnityEngine.Debug.Log("Executing command: " + commandtxt);
            
            SshCommand command = sshClient.RunCommand(commandtxt);
            show_python_error(command);
            UnityEngine.Debug.LogWarning("End of execution");

        }
        else
        {
            UnityEngine.Debug.Log("Tried to execute a command but server not connected");
            return;
        }
    }


    private void show_python_error(SshCommand command)
    {
        if (command.Error != "")
        {
            UnityEngine.Debug.LogError("Python error: " + command.Error);
        }
    }

    void OnApplicationQuit()
    {
        if (sshClient != null)
        {
            UnityEngine.Debug.Log("Disconnecting ssh from the server");
            sshClient.Disconnect();
        }
    }
}

