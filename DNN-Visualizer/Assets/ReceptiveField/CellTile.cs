using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellTile : TileBase
{
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite = Resources.Load<Sprite>("Cell");
    }
}
