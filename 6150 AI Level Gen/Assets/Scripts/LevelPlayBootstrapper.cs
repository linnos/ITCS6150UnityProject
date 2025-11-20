using UnityEngine;

public class LevelPlayBootstrapper : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] LevelUI ui;

    Transform startT;
    EndZone endZone;

    void Awake()
    {
        startT = GameObject.FindGameObjectWithTag("Start")?.transform;
        if (startT == null)
        {
            Debug.LogError("No Start tag found!");
        }
        else
        {
            // Raise spawn position slightly above the start marker
            startT.position += new Vector3(0f, 4.5f, 0f);
            Debug.Log($"Start position set to {startT.position}");
        }

        var endGO = GameObject.FindGameObjectWithTag("End");
        if (endGO)
        {
            var col = endGO.GetComponent<Collider2D>();
            if (!col) col = endGO.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            endZone = endGO.GetComponent<EndZone>() ?? endGO.AddComponent<EndZone>();
        }
        else Debug.LogError("No End tag found!");

        foreach (var spike in GameObject.FindGameObjectsWithTag("Spike"))
        {
            var col = spike.GetComponent<Collider2D>();
            if (!col) col = spike.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            if (!spike.GetComponent<KillZone>()) spike.AddComponent<KillZone>();
        }

        var player = Instantiate(playerPrefab, startT ? startT.position : Vector3.zero, Quaternion.identity);
        LevelState.Init(player, startT, ui);
        if (endZone)
        {
            endZone.OnReachedEnd += () =>
            {
                Debug.Log("Player reached end zone  restarting level");
                LevelState.Respawn();
            };
        }
    }
}
