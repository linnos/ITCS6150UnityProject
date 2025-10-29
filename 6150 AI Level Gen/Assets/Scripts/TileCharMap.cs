using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCharMap : MonoBehaviour
{
    [System.Serializable]
    public class TileCharMapEntry
    {
        public Tile tile;
        public string character;
    }

    public List<TileCharMapEntry> tileCharMapEntries;  
    
    [ContextMenu("Simple Value Assignment")]
    public void SimpleValueAssignment()
    {
        int unicode = 33;

        foreach( TileCharMapEntry entry in tileCharMapEntries)
        {
            if ((Convert.ToChar(unicode)).ToString() == "-")
            {
                unicode++;
            }
            
            entry.character = (Convert.ToChar(unicode)).ToString();
            
            unicode++;
        }
    }
}
