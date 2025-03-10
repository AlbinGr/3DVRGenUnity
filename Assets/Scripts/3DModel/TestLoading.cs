using UnityEngine;

public class TestLoading : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Load3DModel ObjectLoader;
    public string objectPath = "TestObject";
    void Start()
    {
        Debug.Log("TestLoading Start");
        ObjectLoader = GetComponent<Load3DModel>();
        // ObjectLoader.Create3DModel(objectPath);
        Debug.Log("TestLoading End");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
