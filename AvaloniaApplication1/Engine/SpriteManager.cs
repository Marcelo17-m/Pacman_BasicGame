using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine
{
    public class SpriteManager
    {
        private readonly Dictionary<string, Bitmap> _spriteCache = new();

        public Bitmap? LoadSprite(string relativepath) //path es la direccion hacia la imagen.
        {
            if (_spriteCache.ContainsKey(relativepath))
            {
                return _spriteCache[relativepath];
            }
            try
            {
                Uri uri = new Uri($"avares://AvaloniaApplication1/Assets/Sprites/{relativepath}");
                var asset = AssetLoader.Open(uri);
                var bitmap = new Bitmap(asset);

                _spriteCache[relativepath] = bitmap;
                // obtiene el archivo lo vuelve bitmap y lo almacena
                return bitmap;

            }
            catch
            {
                Console.WriteLine("Failed to load resource");
                return null;
            }
        }

        public static Rect CreateSpriteRect(int x, int y, int width, int height)
        {
            return new Rect(x, y, width, height);
        }

        public void ClearCache()
        {
            foreach (Bitmap bitmap in _spriteCache.Values)
            {
                bitmap.Dispose();
                //libera memoria
            }

            _spriteCache.Clear();
        }
    }
}
