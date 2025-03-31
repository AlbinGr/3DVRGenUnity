using UnityEngine;
using System.IO;
using Renci.SshNet;
using UnityEngine.UI;

public class ServerParams : MonoBehaviour
{
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public string host = "";
    public int port = 22;
    public string username = "";

    public string passwordPath = "";
    public string privateKeyPath = "";
    public string serverDirectory = "/home/albin/Documents/VRPython";

    public string dockerName = "";
    public ConnectionInfo connectionInfo = null;
    
    void Start()
    {
        // Create the connection info object
        if (privateKeyPath != "")
        {
            File.SetAttributes(privateKeyPath, FileAttributes.Normal);
            connectionInfo = new ConnectionInfo(host, port, username, new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile(privateKeyPath)));
        }
        else if (passwordPath != "")
        {
            string password = "";
            UnityEngine.Debug.Log("Loging in with password");
            if (File.Exists(passwordPath))
            {
                password = File.ReadAllText(passwordPath).Trim();
            }
            else
            {
                UnityEngine.Debug.LogError("Password file not found at: " + passwordPath);
                return;
            }
            connectionInfo = new ConnectionInfo(host, port, username, new PasswordAuthenticationMethod(username, password));
            UnityEngine.Debug.Log(connectionInfo);
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
