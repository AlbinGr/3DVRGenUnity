using System.Collections.Generic;
using System.IO;
using Renci.SshNet.Sftp;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class Interactor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public string config = "InstantMesh/configs/instant-mesh-base.yaml";
    public int diffusionSteps = 50;

    public float scale = 1.0f;
    public bool removeBackground = true;

    public List<int> gpuIds = new List<int> { 3 };

    private string serverDirectory;
    private SshServer sshServer;
    private SftpServer sftpServer;

    public GameObject ObjectGenerator;
    private Load3DModel ObjectLoader;
    private bool isRunning = false;

    // Create Get isRunning
    public bool GetIsRunning()
    {
        return isRunning;
    }
    // TODO Setup event system that passed the current image on click. 
    // TODO Include events that show the progress of the process to the user or at least notify the user of the state of computation.
    // TODO Include a button to cancel the process

    void Start()
    {
        sshServer = GetComponent<SshServer>();
        sftpServer = GetComponent<SftpServer>();
        serverDirectory = GetComponent<ServerParams>().serverDirectory;
        ObjectLoader = ObjectGenerator.GetComponent<Load3DModel>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DownloadListObject(string directory, string name = null)
    {
        
        if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        foreach (var file in sftpServer.SftpListDirectory(directory))
        {   
            if (file.IsDirectory)
            {
                continue;
            }
            if (name != null) 
            {
                if (!file.Name.Contains(name))
                {
                    continue;
                }
            }
            UnityEngine.Debug.Log("Downloading file " + file.FullName + " to " + directory + file.Name);
            sftpServer.DownloadFile(file.FullName, directory + file.Name);
        }
    }
    public async void Get3DObjectAsync(string imageFilePath)
    {
        UnityEngine.Debug.Log("Get3DObjectAsync called");
        if (!isRunning)
        {
            isRunning = true;
            string imageFilename = Path.GetFileName(imageFilePath);
            Task uploadTask = Task.Run(() => sftpServer.UploadFile(imageFilePath, serverDirectory + $"/Image/{imageFilename}")); 
            string cmd = null;
            if (removeBackground)
            {
                cmd = $"convert.py --input_path Image/{imageFilename} --output_path 3DObject/{Path.GetFileNameWithoutExtension(imageFilename)}/ --config {config} --diffusion_steps {diffusionSteps} --scale {scale} --gpus {string.Join(" ", gpuIds)}";
            }
            else
            {
                cmd = $"convert.py --input_path Image/{imageFilename} --output_path 3DObject/{Path.GetFileNameWithoutExtension(imageFilename)}/ --config {config} --diffusion_steps {diffusionSteps} --scale {scale} --no_rembg  --gpus {string.Join(" ", gpuIds)}";
            }
            Task executeTask = uploadTask.ContinueWith( obj => sshServer.ExecuteCommand(cmd));
            Task downloadObjTask = executeTask.ContinueWith( obj => DownloadListObject($"3DObject/{Path.GetFileNameWithoutExtension(imageFilename)}/"));
            await Task.WhenAll(downloadObjTask);
            
            string modelPath = $"3DObject/{Path.GetFileNameWithoutExtension(imageFilename)}/";
            string objectName = Path.GetDirectoryName(modelPath);
            objectName = objectName.Substring(objectName.LastIndexOf("\\") + 1);
            // Check if the object already exists and if so add a suffixe
            int i = 0;
            while (GameObject.Find(objectName) != null)
            {
                i++;
                objectName += $"_{i}";
            }

            ObjectLoader.Create3DModel(modelPath, objectName);
            // Asynchronously wait for the object to be created
            while (GameObject.Find(objectName) == null)
            {
                await Task.Delay(100);
            }
            GameObject obj = GameObject.Find(objectName);
            Debug.LogWarning(obj);
            MeshRenderer meshRenderer =  obj.transform.Find("default").GetComponent<MeshRenderer>();
            ObjectLoader.AddTexture(modelPath, obj, meshRenderer);
            isRunning = false;
        }
        else
        {
            UnityEngine.Debug.Log("Process already running");
            await Task.Delay(1);
        }
    }

    void OnApplicationQuit()
    {
        IEnumerable<ISftpFile> files = sftpServer.SftpListDirectory("3DObject/");
        // Delete the files in the server and the directories inside the 3DObject folder
        if (!(files == null))
        {
            foreach (var dir in files)
            {
                if (dir.IsDirectory)
                {
                    // Skip if directory ends with . or ..
                    if (dir.FullName.EndsWith(".") || dir.FullName.EndsWith(".."))
                    {
                        continue;
                    }
                    // Delete the directory and everything inside it
                    sshServer.ExecuteCommand($" rm -r {dir.FullName}", false);
                }
                else
                {
                    // Delete the file
                    sftpServer.DeleteFile(dir.FullName);
                }
            }
        }
        files = sftpServer.SftpListDirectory("Image/");
        if (!(files == null))
        {
            foreach (var image in sftpServer.SftpListDirectory("Image/"))
            {   
                if (image.IsDirectory)
                {
                    // Skip if directory ends with . or ..
                    if (image.FullName.EndsWith(".") || image.FullName.EndsWith(".."))
                    {
                        continue;
                    }
                    // Delete the directory and everything inside it
                    sshServer.ExecuteCommand($" rm -r {image.FullName}", false);
                }
                else
                {
                    sftpServer.DeleteFile(image.FullName);
                }
            }
        }
            
        string[] directories = Directory.GetDirectories("3DObject/");
        // Delette everything in Local 3DObject folder
        if (directories == null)
        {
            return;
        }
        foreach (var dir in directories)
        {
            // Delete the directory and its content
            Directory.Delete(dir, true);
        }
    }
}

