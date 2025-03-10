using System.Collections.Generic;
using System.Data;
using System.IO;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class Load3DModel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Pedestal; 
    void Start()
    {
        Pedestal = GameObject.Find("Pedestal");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void Create3DModel(string modelPath, string objectName)
    {
        
        Debug.Log("Creating 3D Model");
        // Get the name of the object (name of the parent folder)
       
        // Get the .obj file from in the ModelPath
        // Test is the object file exists
        if (!System.IO.Directory.Exists(modelPath))
        {
            Debug.LogError("Object file does not exist");
            return;
        }
        // Get the .obj file in the directory
        string[] files = System.IO.Directory.GetFiles(modelPath, "*.obj");
        if (files.Length == 0)
        {
            Debug.LogError("No .obj file found in the directory");
            return;
        }
        string modelFile = files[0];
        string assetFile = "Assets/Resources/3DObjects/" + objectName + ".obj";


        if (System.IO.File.Exists(modelFile))
        {
            System.IO.File.Copy(modelFile, assetFile, true);
        }
        else
        {
            Debug.LogError("Source file does not exist");
        }
        AssetDatabase.ImportAsset(assetFile);

        // FileName without extentions
        UnityEngine.ResourceRequest request = Resources.LoadAsync<GameObject>("3DObjects/" + objectName);
        request.completed += (asyncOperation) => {
            Object obj_ressource = request.asset; 
            GameObject obj = Instantiate(obj_ressource) as GameObject;
            obj.transform.position = Pedestal.transform.position + new Vector3(0, (float)0.5 + Pedestal.transform.localScale.y, 0);
#pragma warning restore UNT0022 // Inefficient position/rotation assignment
            obj.transform.localRotation = Quaternion.Euler(270, 0, 0);
            obj.name = Path.GetFileNameWithoutExtension(objectName);
             };
        
        
    }

    public void AddTexture(string modelPath, GameObject obj, MeshRenderer rend)
        {
        rend.AddComponent<MeshCollider>();
        MeshCollider meshCollider = rend.GetComponent<MeshCollider>();
        meshCollider.convex = true;
        obj.AddComponent<Rigidbody>();
        obj.AddComponent<XRGrabInteractable>();
        obj.AddComponent<XRGeneralGrabTransformer>();
        XRGrabInteractable grabInteractable = obj.GetComponent<XRGrabInteractable>();
        grabInteractable.useDynamicAttach = true;
        grabInteractable.matchAttachPosition = true;
        grabInteractable.matchAttachRotation = true;
        grabInteractable.snapToColliderVolume = true;
        grabInteractable.reinitializeDynamicAttachEverySingleGrab = true;

         
        // Get the .png file in the directory
        string[] files = System.IO.Directory.GetFiles(modelPath, "*.png");
        if (files.Length == 0)
        {
            Debug.LogError("No .png file found in the directory");
            return;
        }
        string textureFile = files[0];
        string assetFile = "Assets/Resources/3DObjects/" + obj.name + ".png";

        if (System.IO.File.Exists(textureFile))
        {
            System.IO.File.Copy(textureFile, assetFile, true);
        }
        else
        {
            Debug.LogError("Source file does not exist");
        }
        AssetDatabase.ImportAsset(assetFile);

        UnityEngine.ResourceRequest request = Resources.LoadAsync<Texture2D>("3DObjects/" + obj.name);
        request.completed += (asyncOperation) => { rend.material.mainTexture = request.asset as Texture2D;};

        // Texture2D texture = request.asset as Texture2D;
        // rend.material.mainTexture = texture;
        
        Debug.Log("Loaded texture");

    }

        void OnApplicationQuit()
    {
        // Delete the content of Asset/resources/3DObjects
        string path = "Assets/Resources/3DObjects";
        DirectoryInfo di = new DirectoryInfo(path);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }   
    }
}
