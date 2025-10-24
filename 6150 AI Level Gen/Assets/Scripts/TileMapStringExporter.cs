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
        EditorApplication.update += SetTestIn;
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

    static void SetTestIn()
    {
        var exporter = UnityEngine.Object.FindAnyObjectByType<TileMapStringExporter>();
        Debug.Log("Loaded TileMapStringExporter");
        if (exporter != null) 
        {
            Tilemap.tilemapTileChanged += exporter.TestIn;
             EditorApplication.update -= SetTestIn;
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

    public void TestIn(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
    {
        tilemap.CompressBounds();
        Debug.Log("TestIn called");
        Debug.Log(tilemap.cellBounds.max.x + " " + tilemap.cellBounds.min.x);
        Debug.Log(tilemap.cellBounds.max.y + " " + tilemap.cellBounds.min.y);
        if (tilemap.cellBounds.max.x > maxXBound || tilemap.cellBounds.max.y > maxYBound)
        {
            Debug.LogError("Tilemap bounds exceed maximum allowed size.");


        }
    }
}