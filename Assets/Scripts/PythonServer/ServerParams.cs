using UnityEngine;
using System.IO;
using Renci.SshNet;

public class ServerParams : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public string host = "";
    public int port = 22;
    public string username = "";
    public string password = "";
    public string privateKeyPath = "";
    public string serverDirectory = "/home/albin/Documents/VRPython";


    public ConnectionInfo connectionInfo = null;
    
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
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
