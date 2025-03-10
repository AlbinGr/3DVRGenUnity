using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using Unity.VisualScripting;
public class NewMonoBehaviourScript : MonoBehaviour
{
    public InputActionReference ButtonA;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Starting the custom button script");
        ButtonA.action.started += ButtonWasPressed;
        ButtonA.action.canceled += ButtonWasReleased;
    }


    void ButtonWasPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Button A was pressed");
    }


    void ButtonWasReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Button A was released");
        // Test is the object file exists

        // Copy and paste the file obj to Ressource folder
        string sourcePath = ".temp/mesh.obj";
        string destinationPath = Application.dataPath + "/Resources/mesh.obj";

        if (System.IO.File.Exists(sourcePath))
        {
            System.IO.File.Copy(sourcePath, destinationPath, true);
            Debug.Log("File copied to Resources folder");
        }
        else
        {
            Debug.Log("Source file does not exist");
        }

        AssetDatabase.ImportAsset("Assets/Resources/mesh.obj");

        // Load the object file from the Ressource folder
        Debug.Log("The file was copied to the Ressource folder and Import Asset was called");

        

        if (Resources.Load("mesh") == null)
        {
            Debug.Log("Object file is null with full path");
            return;
        }
        // Instantiate the object file in obj
        GameObject obj = Instantiate(Resources.Load("mesh")) as GameObject;
        obj.transform.position = new Vector3(0, 1, 0);
        obj.name = "Object 3D";


    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        // Delete all the object files from the Ressource folder with glob pattern
        string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Resources", "*.obj");
        foreach (string file in files)
        {
            Debug.Log("Deleting file: " + file);
            System.IO.File.Delete(file);
        }

        files = System.IO.Directory.GetFiles(Application.dataPath + "/Resources", "*.obj.meta");
        foreach (string file in files)
        {
            Debug.Log("Deleting file: " + file);
            System.IO.File.Delete(file);
        }

    }
}

public class PostProcessImportAsset : AssetPostprocessor
{
    
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        Debug.Log("OnPostprocessAllAssets");

        foreach (var imported in importedAssets)
            Debug.Log("Imported: " + imported);

        foreach (var deleted in deletedAssets)
            Debug.Log("Deleted: " + deleted);

        foreach (var moved in movedAssets)
            Debug.Log("Moved: " + moved);

        foreach (var movedFromAsset in movedFromAssetPaths)
            Debug.Log("Moved from Asset: " + movedFromAsset);
    }
}

