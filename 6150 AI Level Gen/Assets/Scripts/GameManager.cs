using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static string modelName = "NoModifiers";

    public Queue<string> levels = new Queue<string>();
    public string currentLevel = "";
    public TileMapStringExporter tileMapStringExporter = null;
    public LevelMarkerSpawner spawner = null;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
        }
        else if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            GoToNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            ReturnToMainMenu();
        }
    }

    private IEnumerator SendLevelGenerationRequest()
    {
        var requestObj = new LevelRequest();
        requestObj.modelName = modelName;
        string json = JsonUtility.ToJson(requestObj);
        Debug.Log($"Generating Level... Request JSON: {json}");

        using (var www = new UnityWebRequest("http://localhost:5000/api/LevelGeneration/generate", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "*/*");

            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError($"Request error: {www.error}");
            }
            else
            {
                SimpleJSON.JSONNode text = SimpleJSON.JSON.Parse(www.downloadHandler.text);
                string[] cleanedString = text[1].ToString().Split("\\n");
                string joinedString = string.Join("\n", cleanedString);
                Debug.Log($"Response: {joinedString}");
                levels.Enqueue(joinedString);

                if(SceneManager.GetActiveScene().name != "LevelEditor")
                {
                    SceneManager.LoadScene("LevelEditor");
                }
            }
        }
    }

    public void GenerateLevel()
    {
        StartCoroutine(SendLevelGenerationRequest());
    }

    #region hotkeys / cheats

    public void ResetLevel()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            return;
        }
    }

    public void GoToNextLevel()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            return;
        }
        if (levels.Count > 0)
        {
            string currentLevel = levels.Dequeue();
            if(tileMapStringExporter == null)
            {
                tileMapStringExporter = FindFirstObjectByType<TileMapStringExporter>(FindObjectsInactive.Exclude);
            }
            if(spawner == null)
            {
                spawner = FindFirstObjectByType<LevelMarkerSpawner>(FindObjectsInactive.Exclude);
            }

            tileMapStringExporter.importString = currentLevel;
            tileMapStringExporter.ImportTilesFromString();

            GenerateLevel();
            spawner.SpawnFromMarkers();
            LevelState.Respawn();
            
        }
        else
        {
            GenerateLevel();
            Debug.Log("No more levels in queue. Wait for level to load");
            if(spawner != null){
                spawner.loadingImage.SetActive(true);
            }
        }
    }

    public void ReturnToMainMenu()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            return;
        }
        //empty list of levels
        levels = new Queue<string>();
        SceneManager.LoadScene("Main Menu");
        Destroy(this.gameObject);
    }
    #endregion
}

[System.Serializable]
public class LevelRequest
{
    public string modelName = "NoModifiers";
    public int numChunks = 15;
    public int numColsPerChunk = 16;
    public int numRowsPerCol = 16;
}
