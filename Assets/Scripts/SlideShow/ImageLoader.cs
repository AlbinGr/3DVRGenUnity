using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class ImageLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string imageFolder = "Images";
    public bool RandomStart = true;
    public bool shuffleImageOrder = true;
    public GameObject PythonInteractor;

    public float TimeBetweenImages = 5;
    private float timer = 0;
    private int CurrentSpriteIdx = 0;

    private LongClickEventTrigger longEventTrigger;
    private Image ImageScreen;
    private List<Sprite> ListImagesSprites = new List<Sprite>();
    private List<string> ListImagesPaths = new List<string>();

    public GameObject imageMask;
    public GameObject textLoading;
    private Interactor interactor;

    private bool isLoading = false;
    void Start()
    {
        // Initialize imagescreen
        Debug.LogWarning("ImageLoader: Start");

        ImageScreen = GetComponent<Image>();
        longEventTrigger = GetComponent<LongClickEventTrigger>();
        interactor = PythonInteractor.GetComponent<Interactor>();
        
        longEventTrigger.onLongPress.AddListener( () => interactor.Get3DObjectAsync(ListImagesPaths[CurrentSpriteIdx]));
        // Get a list of png images in the folder
        string[] images = System.IO.Directory.GetFiles(imageFolder, "*.png");

        
        foreach (string image in images)
        {
            Sprite SpriteImage = LoadImage(image);
            ListImagesSprites.Add(SpriteImage);
            ListImagesPaths.Add(image);
        }
        
        if (shuffleImageOrder)
        {
            // Reorder both lists with the same permutation
            System.Random rng = new System.Random();
            int n = ListImagesSprites.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Sprite value = ListImagesSprites[k];
                ListImagesSprites[k] = ListImagesSprites[n];
                ListImagesSprites[n] = value;
                string valuePath = ListImagesPaths[k];
                ListImagesPaths[k] = ListImagesPaths[n];
                ListImagesPaths[n] = valuePath;
            }

        }

        CurrentSpriteIdx = 0;
        if (RandomStart)
        {
            CurrentSpriteIdx = Random.Range(0, ListImagesSprites.Count);
        }
        else
        {
            CurrentSpriteIdx = 0;
        }

        UpdateImage();
        imageMask.SetActive(false);
        textLoading.SetActive(false);
}

    
    Sprite LoadImage(string image_path)
    {
        // Load the image from the path
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = System.IO.File.ReadAllBytes(image_path);
        texture.LoadImage(fileData);
        texture.Apply();
        
        // Make a sprite from the texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1f, 1f));
        
        return sprite;
        }

    // Update is called once per frame
    void Update()
    {
        isLoading = interactor.GetIsRunning();
        if (isLoading)
        {
            imageMask.SetActive(true);
            textLoading.SetActive(true);
            timer = 0;
        }
        else
        {
            imageMask.SetActive(false);
            textLoading.SetActive(false);
            if (timer > TimeBetweenImages)
            {
                NextImage();
            }
            timer += Time.deltaTime;
        }
        
    }

    void UpdateImage()
    {
        var sp = ListImagesSprites[CurrentSpriteIdx];
        ImageScreen.sprite = sp;
        ImageScreen.raycastPadding = new Vector4(0.5f, 0f , 0f ,0f );
    }

    public void NextImage()
    {   
        CurrentSpriteIdx = (CurrentSpriteIdx + 1) % ListImagesSprites.Count;
        UpdateImage();
        timer = 0;
    }

    public void PreviousImage()
    {
        if (CurrentSpriteIdx == 0)
        {
            CurrentSpriteIdx = ListImagesSprites.Count -1;
        }
        else
        {
            CurrentSpriteIdx = (CurrentSpriteIdx - 1) % ListImagesSprites.Count;
        }
        UpdateImage();
        timer = 0;
    }
}
