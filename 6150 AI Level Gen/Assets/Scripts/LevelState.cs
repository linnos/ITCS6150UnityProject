using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelState
{
    static GameObject player;
    static Transform startT;
    static LevelUI ui;

    public static void Init(GameObject p, Transform s, LevelUI u) { player = p; startT = s; ui = u; }

    public static void Respawn()
    {
        if (!player || !startT) return;
        player.transform.position = startT.position;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;
    }

    public static void ResetLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public static void NextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    public static void MarkUnplayable()
    {
        Debug.LogWarning($"Level marked UNPLAYABLE: {SceneManager.GetActiveScene().name}");
        ResetLevel();
    }
}
