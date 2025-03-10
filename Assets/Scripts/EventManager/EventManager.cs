using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Action ExampleEvent;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ExampleEvent?.Invoke();
    }
}
