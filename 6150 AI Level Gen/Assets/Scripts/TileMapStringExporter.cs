using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapStringExporter : MonoBehaviour
{
    public Tilemap tilemap;
    public List<Tile> tiles;
    public TileCharMap tileCharMap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
}

