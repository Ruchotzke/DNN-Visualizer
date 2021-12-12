using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TiledFeaturemap : MonoBehaviour
{
    Tilemap tmap;

    /* Generated tiles */
    Tile tile;

    private void Awake()
    {
        tmap = GetComponentInChildren<Tilemap>();

        /* Generate Tiles */
        tile = new Tile();
        tile.sprite = Resources.Load<Sprite>("Cell");

        tmap.SetTile(Vector3Int.zero, tile);
    }
}
