using UnityEngine;

public class LoadingShow : MonoBehaviour
{
    public GameObject PythonInteractor;
    private GameObject imageMask;
    private GameObject textLoading;
    private Interactor interactor;
    private bool isLoading = false;

    void Start()
    {
        interactor = PythonInteractor.GetComponent<Interactor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
