﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace BFG.Runtime {
public enum CellType {
    None,
    Road,
    Station
}

public class HorseMovementSystemInterface : MonoBehaviour {
    [SerializeField]
    [Required]
    Grid _grid;

    HorseMovementSystem _system;

    public List<GameObject> Nodes = new();

    [SerializeField]
    [Required]
    Tilemap _tilemap;

    [SerializeField]
    TileBase _arrow0;

    [SerializeField]
    TileBase _arrow1;

    [SerializeField]
    TileBase _arrow21;

    [SerializeField]
    TileBase _arrow22;

    [SerializeField]
    TileBase _arrow3;

    [SerializeField]
    TileBase _arrow4;

    [SerializeField]
    Vector2Int _pointA;

    [SerializeField]
    Vector2Int _pointB;

    TileBase GetTilebase(MovementGraphCell mapMovementCell) {
        if (mapMovementCell == null) {
            return null;
        }

        var c = mapMovementCell.Count();
        switch (c) {
            case 0:
                return _arrow0;
            case 1:
                return _arrow1;
            case 2:
                return mapMovementCell.TwoTypeIsVertical() ? _arrow21 : _arrow22;
            case 3:
                return _arrow3;
            case 4:
                return _arrow4;
        }

        return null;
    }

    // [SerializeField]
    // [TableMatrix(SquareCells = true)]
    // Texture2D[,] _cells = new Texture2D[,] { { null } };

    MapCell[,] _cells = {
        {
            MapCell.Road, MapCell.Road, MapCell.Road, MapCell.None, MapCell.Road
        }, {
            MapCell.Road, MapCell.None, MapCell.Road, MapCell.Road, MapCell.Road
        }, {
            MapCell.Road, MapCell.Road, MapCell.Road, MapCell.None, MapCell.Road
        }, {
            MapCell.None, MapCell.Road, MapCell.None, MapCell.Road, MapCell.Road
        }, {
            MapCell.Road, MapCell.Road, MapCell.Road, MapCell.Road, MapCell.Road
        }, {
            MapCell.None, MapCell.Road, MapCell.None, MapCell.Road, MapCell.None
        }
    };

    MovementGraphCell[,] _movementCells;

    [Button("Generate tilemap")]
    void GenerateTilemap() {
        _tilemap.ClearAllTiles();

        var sizeY = _cells.GetLength(0);
        var sizeX = _cells.GetLength(1);
        _grid.transform.position = new Vector3(-sizeX / 2f, -sizeY / 2f, 0);

        _movementCells = new MovementGraphCell[sizeY, sizeX];
        for (var y = 0; y < sizeY; y++) {
            for (var x = 0; x < sizeX; x++) {
                var cell = _cells[y, x];
                var mCell = new MovementGraphCell(false, false, false, false);
                _movementCells[y, x] = cell.Type == CellType.Road ? mCell : null;

                if (cell.Type == CellType.Road) {
                    mCell.Up = y < sizeY - 1 && _cells[y + 1, x].Type == CellType.Road;
                    mCell.Down = y > 0 && _cells[y - 1, x].Type == CellType.Road;
                    mCell.Left = x > 0 && _cells[y, x - 1].Type == CellType.Road;
                    mCell.Right = x < sizeX - 1 && _cells[y, x + 1].Type == CellType.Road;
                }
            }
        }

        for (var y = 0; y < sizeY; y++) {
            for (var x = 0; x < sizeX; x++) {
                var cell = _movementCells[y, x];
                var tb = GetTilebase(cell);
                if (tb == null) {
                    continue;
                }

                var td = new TileChangeData(
                    new Vector3Int(x, y, 0),
                    tb,
                    Color.white,
                    Matrix4x4.TRS(
                        new Vector3(0, 0, 0),
                        Quaternion.Euler(0, 0, 90 * cell.Rotation()),
                        Vector3.one
                    )
                );
                _tilemap.SetTile(td, false);
            }
        }
    }
}
}
