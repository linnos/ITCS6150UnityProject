using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour
{
    public string modelName = "NoModifiers";
    public GameObject loadingImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGenerateLevelButtonClicked()
    {
        GameManager.modelName = modelName;
        for(int i = 0; i < 3; i++)
        {
            GameManager.Instance.GenerateLevel();
        }

        ShowLevelLoadImage();
    }

    public void ShowLevelLoadImage()
    {
        loadingImage.SetActive(true);
    }
}