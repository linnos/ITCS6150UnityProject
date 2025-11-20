using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour
{
    public string level = "NoModifiers";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class LevelRequest
    {
        public string modelName = "NoModifiers";
        public int numChunks = 15;
        public int numColsPerChunk = 16;
        public int numRowsPerCol = 16;
    }

    private IEnumerator SendLevelGenerationRequest()
    {
        var requestObj = new LevelRequest();
        requestObj.modelName = level;
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
            }
        }
    }

    public void OnGenerateLevelButtonClicked()
    {
        StartCoroutine(SendLevelGenerationRequest());
    }
}