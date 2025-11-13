using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIRequestHandler : MonoBehaviour
{
    // Api url example:
    // https://pokeapi.co/api/v2/pokemon/${pokemon}/

    public string url = "http://127.0.0.1:8000/";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDestroy()
    {
    }

    
    public IEnumerator GetDatas()
    {

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            Debug.Log(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode levelData = SimpleJSON.JSON.Parse(json);
                Debug.Log(levelData[0]);
            }
        }
    }

    public void DeleteSelf(){
        Destroy(this.gameObject);
    }
}
