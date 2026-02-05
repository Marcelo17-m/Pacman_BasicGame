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

        public int WidthPx => Columns * TileWidth;

        public int HeightPx => Rows * TileHeight;

        private readonly MapTileType[,] _tiles; // para mas adelante [5,5] ejemplo

        public GameMap(int columns, int rows, int tileWidth = 15, int tileHeight = 15)
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
                return MapTileType.Wall;
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
    }
}
