using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CSCRefactor.ProceduralGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace CSCRefactor
{
    public class Scene
    {
        public string Tileset;
        public int TilesetWidth;
        public int TileSize;
        public int TilemapWidth;
        public int TilemapHeight;
        public int[] Tilemap;
        public int[] Grassmap;
        public string[] Backgrounds;
        public string[] Links;
        public int[] LinkOffsets;
    }
    public class Background
    {
        public float X;
        public float Y;
        public Texture2D Texture;
        public float Parallax;
        public int Duplicates;
        public Background(float x, float y, Texture2D texture, float parallax)
        {
            X = x;
            Y = y;
            Texture = texture;
            Parallax = parallax;
        }
        public void Update(Camera camera, float scale)
        {
            if (X + camera.XOffset * Parallax > 0) X -= Texture.Width * scale;
            if (X + (Duplicates * Texture.Width * scale) + camera.XOffset * Parallax < camera.Width) X += Texture.Width * scale;
        }
        public void Draw(SpriteBatch _spriteBatch, Camera camera, float scale)
        {
            for (int i = 0; i < Duplicates; i++)
                _spriteBatch.Draw(Texture, new Vector2(X + (i * Texture.Width * scale) + camera.XOffset * Parallax,
                                  Y - camera.YOffset * Parallax),
                                  null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
    public class SceneManager
    {
        private float elapsed = 0;
        private readonly float scale;
        public int[,] Tilemap;
        public List<Grass> Grass = new List<Grass>();
        private const int grassDensity = 4;
        public List<RectangleF> Colliders = new List<RectangleF>();
        private RectangleF previousBounds = new RectangleF(0, 0, 0, 0);
        public RectangleF Bounds = new RectangleF(0, 0, 0, 0);
        private Dictionary<string, Texture2D> tilesets;
        private RenderTarget2D drawnTilemap;
        private RenderTarget2D previousDrawnTilemap;
        private RenderTarget2D grassRenderTarget;
        private RenderTarget2D previousGrassRenderTarget;
        private List<Background> backgrounds;
        private string[] links;
        public SceneManager(GraphicsDevice graphicsDevice, Dictionary<string, Texture2D> tilesets, float scale)
        {
            this.tilesets = tilesets;
            this.scale = scale;
        }

        public void LoadScene(string sceneName, List<Background> backgrounds,
                              GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, int viewWidth, int viewHeight, float scale, int direction)
        {
            string localPath = @$"Scenes\{sceneName}.json";
            string fullPath = System.IO.Path.GetFullPath(localPath);
            string json = System.IO.File.ReadAllText(fullPath);
            Scene scene = JsonConvert.DeserializeObject<Scene>(json);
            previousBounds = Bounds;
            if (direction == -1)
            {
                Bounds = new RectangleF(Bounds.X, Bounds.Y, scene.TilemapWidth * scene.TileSize * scale, scene.TilemapHeight * scene.TileSize * scale);
                drawnTilemap = new RenderTarget2D(graphicsDevice, 1, 1);
                grassRenderTarget = new RenderTarget2D(graphicsDevice, 1, 1);
            }
            else if (direction == 0)
                Bounds = new RectangleF(Bounds.X - scene.TilemapWidth * scene.TileSize * scale, Bounds.Y, scene.TilemapWidth * scene.TileSize * scale, scene.TilemapHeight * scene.TileSize * scale);
            else if (direction == 2)
                Bounds = new RectangleF(Bounds.Right, Bounds.Y, scene.TilemapWidth * scene.TileSize * scale, scene.TilemapHeight * scene.TileSize * scale);

            previousDrawnTilemap = drawnTilemap;
            previousGrassRenderTarget = grassRenderTarget;
            drawnTilemap = new RenderTarget2D(graphicsDevice, scene.TilemapWidth * scene.TileSize, scene.TilemapHeight * scene.TileSize);
            Texture2D tileset = tilesets[scene.Tileset];
            int tilesetWidth = tileset.Width / scene.TileSize;

            links = scene.Links;

            Tilemap = new int[scene.TilemapWidth, scene.TilemapHeight];
            Colliders.Clear();
            Grass.Clear();
            graphicsDevice.SetRenderTarget(drawnTilemap);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            for (int yIndex = 0; yIndex < scene.TilemapHeight; yIndex++)
            {
                int y = (scene.TilemapHeight - 1) - yIndex;
                for (int x = 0; x < scene.TilemapWidth; x++)
                {
                    // grass
                    if (scene.Grassmap[scene.TilemapWidth * y + x] == 0)
                        for (float grassX = x * scene.TileSize * scale + (scene.TileSize * scale) / (grassDensity * 2); grassX < (x + 1) * scene.TileSize * scale;
                             grassX += (scene.TileSize * scale) / grassDensity)
                            Grass.Add(new Grass(grassX + (Utils.Random.NextSingle() * 4 - 2),
                                                yIndex * scene.TileSize * scale - 4 * scale, 4, 
                                                10 + (Utils.Random.NextSingle() * 2 - 1), MathF.PI / 2, new Color(157, 217, 141)));
                    // tile
                    Tilemap[x, y] = scene.Tilemap[scene.TilemapWidth * y + x];
                    if (Tilemap[x, y] == -1) continue;
                    Colliders.Add(new RectangleF(x * scene.TileSize * scale + Bounds.X, yIndex * scene.TileSize * scale + Bounds.Y, scene.TileSize * scale, scene.TileSize * scale));
                    spriteBatch.Draw(tileset, new Vector2(x * scene.TileSize, y * scene.TileSize),
                                     TileRectangle(Tilemap[x, y], tilesetWidth, scene.TileSize), Color.White);
                }
            }
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
            grassRenderTarget = new RenderTarget2D(graphicsDevice, scene.TilemapWidth * scene.TileSize, scene.TilemapHeight * scene.TileSize);

            foreach (Background background in backgrounds)
                background.Duplicates = viewWidth / (int)(background.Texture.Width * scale) + 2;
            this.backgrounds = backgrounds;
        }
        public void Update(float deltaTime, Player player, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Camera camera)
        {
            elapsed += deltaTime;
            if (player.Rectangle.CenterX > Bounds.Right)
                LoadScene(links[2], backgrounds, graphicsDevice, spriteBatch, camera.Width, camera.Height, scale, 2);
            else if (player.Rectangle.CenterX < Bounds.Left)
                LoadScene(links[0], backgrounds, graphicsDevice, spriteBatch, camera.Width, camera.Height, scale, 0);
            foreach (Grass grass in Grass)
            {
                grass.Update(elapsed);
                grass.Push(player, Bounds);
            }
            foreach (Background background in backgrounds) background.Update(camera, scale);
        }
        public void DrawRenderTargets(Shapes _shapes)
        {
            _shapes.BeginRenderTarget(ref grassRenderTarget);
            grassRenderTarget.GraphicsDevice.Clear(Color.Transparent);
            foreach (Grass grass in Grass) grass.Draw(_shapes, scale);
            _shapes.End();
        }
        public void DrawSceneBackground(SpriteBatch _spriteBatch, Camera camera)
        {
            foreach (Background background in backgrounds) background.Draw(_spriteBatch, camera, scale);
        }
        public void DrawScene(SpriteBatch _spriteBatch, Camera camera)
        {
            Vector2 mapPoint = new Vector2(Bounds.X + camera.XOffset, Bounds.Y + camera.Height - drawnTilemap.Height * scale - camera.YOffset);
            Vector2 previousMapPoint = new Vector2(previousBounds.X + camera.XOffset, previousBounds.Y + camera.Height - drawnTilemap.Height * scale - camera.YOffset);
            _spriteBatch.Draw(previousGrassRenderTarget, previousMapPoint, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            _spriteBatch.Draw(previousDrawnTilemap, previousMapPoint, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            _spriteBatch.Draw(grassRenderTarget, mapPoint, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            _spriteBatch.Draw(drawnTilemap, mapPoint, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
        private static Rectangle TileRectangle(int index, int tilesetWidth, int tileSize)
        {
            int x = index % tilesetWidth;
            int y = index / tilesetWidth;
            return new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
        }
    }
}
