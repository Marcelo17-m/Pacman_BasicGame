using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class GameMap
    {
        public int TileWidth { get; } // ancho de la celda en pixeles
        public int TileHeight { get; } // alto de la celda en pixeles
        public int Columns { get; } //cantidad de columnas
        public int Rows { get; } //cantidad de filas

        private readonly MapTileType[,] _tiles; 

        public GameMap(int columns, int rows, int tileWidth = 16, int tileHeight = 16)
        {
            Columns = columns;
            Rows = rows;
            TileHeight = tileHeight;
            TileWidth = tileWidth;
            _tiles = new MapTileType[rows, columns];
        }

        public MapTileType GetTile(int row, int col)
        {
            if (col < 0|| col >= Columns || row < 0 || row >= Rows)
            {
                return MapTileType.Wall; // por si se sale del mapa
            }

            return _tiles[row, col];
        }

        public void SetTile(int row, int col, MapTileType type)
        {
            if (col >= 0 && col < Columns && row >= 0 && row < Rows)
            {
                _tiles[row, col] = type;
            }
        }

        //convertir posicion en pixelex a coordeanadas de la celda 
        public (int row, int col) WorldToTile(double worldX, double worldY)
        {
            int row = (int)(worldY / TileHeight);
            int col = (int)(worldX / TileWidth);
            return (row, col);
        }

        // Convierte celda row, col  a posicion en pixeles
        public (double x, double y) TileToWorld(int row, int col)
        {
            double x = col * TileWidth;
            double y = row * TileHeight;
            return (x, y);
        }

        public bool IsWalkable(int row, int col)
        {
            var tile = GetTile(row, col);
            return tile != MapTileType.Wall && tile != MapTileType.GhostDoor;
        }


         // Comprueba si un rectangulo en posicion X,Y con tamaño de ancho y alto choca con un muro
        public bool CanMoveTo(double worldX, double worldY, double width, double height)
        {
            int colMin = (int)(worldX / TileWidth);
            int rowMin = (int)(worldY / TileHeight);
            int colMax = (int)((worldX + width - 0.001) / TileWidth);
            int rowMax = (int)((worldY + height - 0.001) / TileHeight);

            colMin = Math.Clamp(colMin, 0, Columns - 1);
            colMax = Math.Clamp(colMax, 0, Columns - 1);
            rowMin = Math.Clamp(rowMin, 0, Rows - 1);
            rowMax = Math.Clamp(rowMax, 0, Rows - 1);

            for (int row = rowMin; row <= rowMax; row++)
            {
                for (int col = colMin; col <= colMax; col++)
                {
                    if (!IsWalkable(row, col))
                    {
                        return false;
                    }
                }
            }
            return true;

        }

        // Crea un mapa de prueba: borde de muros y el resto vacio.

        public static GameMap CreateFromLayout(string[] layout, int tileSize = 16)
        {
            int rows = layout.Length;
            int cols = layout[0].Length;
            var map = new GameMap(cols, rows, tileSize, tileSize);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    char c = layout[row][col];
                    MapTileType type = CharToTileType(c);
                    map.SetTile(row, col, type);
                }
            }
            return map;
        }

        private static MapTileType CharToTileType(char c)
        {
            switch (c)
            {
                case '#':
                    return MapTileType.Wall;

                case '-':
                    return MapTileType.GhostDoor;

                case '.':
                    return MapTileType.Point;

                case 'o':
                    return MapTileType.PowerPoint;

                case ' ':
                    return MapTileType.Empty;

                case 'X':
                    return MapTileType.Empty;

                default: 
                    return MapTileType.Empty;
            }
        }

        public List<(int row, int col)> GetTilesOfType(MapTileType type)
        {
            var positions = new List<(int row, int col)>();

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (_tiles[row, col] == type)
                    {
                        positions.Add((row, col));
                    }
                }
            }
            return positions;
        }

    }
}
