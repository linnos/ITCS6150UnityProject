using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[InitializeOnLoad]
public class TileMapStringExporter : MonoBehaviour
{
    public Tilemap tilemap;
    public List<Tile> tiles;
    public TileCharMap tileCharMap;

    public int maxXBound = 250;
    public int maxYBound = 16;

    public EventSystem eventSystem;

    static TileMapStringExporter()
    {
        EditorApplication.update += EnableBoundaryFunction;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // eventSystem.
        // Tilemap.tilemapTileChanged += TestIn;
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Enables the boundary function once the editor is done loading
    static void EnableBoundaryFunction()
    {
        var exporter = UnityEngine.Object.FindAnyObjectByType<TileMapStringExporter>();
        Debug.Log("Loaded TileMapStringExporter");
        if (exporter != null)
        {
            Tilemap.tilemapTileChanged += exporter.EnableBoundary;
            EditorApplication.update -= EnableBoundaryFunction;
        }
    }

    [ContextMenu("Export Tiles As String")]
    public void ExportAsString()
    {
        tilemap.CompressBounds();

        List<string> result = new List<string>();

        String output = "";

        for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
        {
            for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
            {
                Tile tile = tilemap.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    if (tileCharMap.tileCharMapEntries.Exists(entry => entry.tile == tile))
                    {
                        output += tileCharMap.tileCharMapEntries.Find(entry => entry.tile == tile).character;
                    }
                }
                else
                {
                    output += "X";
                }
                // if (tile != null && !tiles.Contains(tile))
                // {
                //     tiles.Add(tile);
                // }
            }

            result.Add(output);
            output = "";
        }

        result.Reverse();
        output = string.Join("\n", result);
        Debug.Log(output);
        // foreach (Tile tile in tiles)
        // {
        //     Debug.Log(tile.name);
        // }
    }

    [ContextMenu("Initialize Tile Char Map")]
    public void InitializeTileCharMap()
    {
        if (tiles.Count == 0)
        {
            Debug.LogError("Tiles list is empty. Please move tiles to the Tiles list in the inspector.");
            return;
        }

        foreach (Tile tile in tiles)
        {
            tileCharMap.tileCharMapEntries.Add(new TileCharMap.TileCharMapEntry { tile = tile, character = "" });
        }

    }

    // Function that checks if the tilemap bounds exceed the maximum allowed size.
    //If so, deletes the excess tiles and logs an error.
    public void EnableBoundary(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
    {
        tilemap.CompressBounds();
        Debug.Log("TestIn called");
        
        if (tilemap.cellBounds.max.x > maxXBound || tilemap.cellBounds.max.y > maxYBound ||
            tilemap.cellBounds.min.x < 0 || tilemap.cellBounds.min.y < 0)
        {
            Debug.LogError("Tilemap bounds exceed maximum allowed size.");

            List<Vector3Int> toRemove = new List<Vector3Int>();

            for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
                {
                    Debug.Log(x + " " + y);
                    if (x >= maxXBound || y >= maxYBound || x < 0 || y < 0)
                    {
                        var pos = new Vector3Int(x, y, 0);
                        if (tilemap.HasTile(pos))
                            toRemove.Add(pos);
                    }
                }
            }

            if (toRemove.Count > 0)
            {
                foreach (var pos in toRemove)
                {
                    tilemap.SetTile(pos, null);
                }

                tilemap.RefreshAllTiles();
                Debug.LogError($"Tilemap bounds exceed maximum allowed size. Removed {toRemove.Count} tile(s) outside ({maxXBound}, {maxYBound}).");
            }
            else
            {
                Debug.Log("Tilemap bounds exceed limits but no tiles were found in the excess area to remove.");
            }
        }
    }
}