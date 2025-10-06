using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIRequestHandler : MonoBehaviour
{
    // Api url example:
    // https://pokeapi.co/api/v2/pokemon/${pokemon}/

    private string url = "https://pokeapi.co/api/v2/pokemon/ditto";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetDatas());
    }

    private void OnDestroy()
    {
    }

    IEnumerator GetDatas()
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
                Debug.Log(levelData);
            }
        }
    }

    public void DeleteSelf(){
        Destroy(this.gameObject);
    }
}
