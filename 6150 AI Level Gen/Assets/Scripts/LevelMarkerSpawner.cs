using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Tilemaps;

public class LevelMarkerSpawner : MonoBehaviour
{
    [Header("Marker Tilemap (the one with Start/End/Spike tiles)")]
    public Tilemap markerTilemap;

    [Header("Marker Tiles (from your tile palette)")]
    public TileBase startTile;
    public TileBase endTile;
    public TileBase spikeTile;

    [Header("Prefabs to place at markers")]
    public GameObject startPrefab;  // e.g., a StartSign prefab or an empty with tag "Start"
    public GameObject endPrefab;    // e.g., EndSign prefab (no collider needed; script will add)
    public GameObject spikePrefab;  // e.g., spike prefab (no collider needed; script will add)

    [Header("Options")]
    public bool clearMarkerTilesAfterSpawning = true;
    public Transform spawnedParent;     // Optional parent for organization

    [Header("Outputs")]
    public Transform startTransform;    // First start found (for player spawn)
    public Transform endTransform;      // First end found

    public List<GameObject> spawnedObjects = null;
    public GameObject loadingImage;

    void Awake()
    {
        spawnedObjects = new List<GameObject>();
        if (!markerTilemap)
        {
            Debug.LogError("LevelMarkerSpawner: Marker Tilemap is not assigned.");
            return;
        }

        SpawnFromMarkers();
    }

    public void SpawnFromMarkers()
    {
        var bounds = markerTilemap.cellBounds;

        // Important: get the Tilemap origin Grid for proper world position
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                var cell = new Vector3Int(x, y, 0);
                var tile = markerTilemap.GetTile(cell);
                if (tile == null) continue;

                Vector3 worldPos = markerTilemap.GetCellCenterWorld(cell);

                if (tile == startTile)
                {
                    var go = Instantiate(startPrefab, worldPos, Quaternion.identity, spawnedParent);
                    startTransform ??= go.transform;
                    EnsureTagged(go, "Start");
                    if (clearMarkerTilesAfterSpawning) markerTilemap.SetTile(cell, null);
                }
                else if (tile == endTile)
                {
                    var go = Instantiate(endPrefab, worldPos, Quaternion.identity, spawnedParent);
                    endTransform ??= go.transform;
                    EnsureTagged(go, "End");
                    EnsureEndZone(go);
                    if (clearMarkerTilesAfterSpawning) markerTilemap.SetTile(cell, null);
                }
                else if (tile == spikeTile)
                {
                    var go = Instantiate(spikePrefab, worldPos, Quaternion.identity, spawnedParent);
                    EnsureTagged(go, "Spike");
                    EnsureSpikeKill(go);
                    spawnedObjects.Add(go);
                    if (clearMarkerTilesAfterSpawning) markerTilemap.SetTile(cell, null);
                }
            }
        }

        if (!startTransform) Debug.LogWarning("LevelMarkerSpawner: No Start tile found.");
        if (!endTransform) Debug.LogWarning("LevelMarkerSpawner: No End tile found.");
        loadingImage.SetActive(false);
    }

    void EnsureTagged(GameObject go, string tag)
    {
        // Assumes you created these tags in Project Settings → Tags and Layers
        try { go.tag = tag; } catch { /* ignore if tag doesn't exist */ }
    }

    public void Clear()
    {
        foreach (var gameObject in spawnedObjects)
        {
            Destroy(gameObject);
        }
        spawnedObjects = new List<GameObject>();
    }

    void EnsureSpikeKill(GameObject go)
    {
        // Collider (trigger) + KillZone script
        var col = go.GetComponent<Collider2D>();
        if (!col) col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        if (!go.GetComponent<KillZone>()) go.AddComponent<KillZone>();
    }

    void EnsureEndZone(GameObject go)
    {
        // Collider (trigger) + EndZone script
        var col = go.GetComponent<Collider2D>();
        if (!col) col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        if (!go.GetComponent<EndZone>()) go.AddComponent<EndZone>();
    }
}
