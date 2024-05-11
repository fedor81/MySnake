using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MySnake.Model;
using Point = MySnake.Model.Point;

namespace MySnake.View;

public class GameView
{
    public GameView(GraphicsDevice graphicsDevice, GameModel model, int viewWidth, int viewHeight)
    {
        Model = model;
        GraphicsDevice = graphicsDevice;
        ViewWidth = viewWidth;
        ViewHeight = viewHeight;
    }

    private int ViewWidth { get; set; }
    private int ViewHeight { get; set; }

    private int WindowWidth { get; set; }
    private int WindowHeight { get; set; }

    private Rectangle CellSize { get; set; }
    public Texture2D SquareTexture { get; set; }

    private GraphicsDevice GraphicsDevice { get; init; }
    public SpriteBatch SpriteBatch { get; set; }
    public RenderTarget2D ToDrawBuffer { get; private set; }
    public GameModel Model { get; set; }

    private RenderTarget2D GameBuffer { get; set; }
    private RenderTarget2D MapBuffer { get; set; }
    private Vector2 MapPosition { get; set; }
    public Vector2 ToDrawPosition { get; private set; }

    public void ShowOrCloseMap()
    {
        if (ToDrawBuffer == MapBuffer)
            SetGameBufferToDraw();
        else
        {
            ToDrawBuffer = MapBuffer;
            ToDrawPosition = MapPosition;
        }
    }

    public void SetWindowSize(object sender, EventArgs eventArgs)
    {
        var window = sender as GameWindow;
        SetWindowSize(window!.ClientBounds.Width, window.ClientBounds.Height);
    }

    private void SetWindowSize(int width, int height)
    {
        WindowWidth = width;
        WindowHeight = height;
        var cellWidth = WindowWidth / ViewWidth;
        var cellHeight = WindowHeight / ViewHeight;
        CellSize = new Rectangle(0, 0, cellWidth, cellHeight);

        MapBuffer = GetDrawMap();
        // ReSharper disable once PossibleLossOfFraction
        GameBuffer = CreateBuffer(WindowWidth, WindowHeight);
        Update();
    }

    private RenderTarget2D CreateBuffer(int width, int height)
    {
        GameBuffer?.Dispose();
        return new RenderTarget2D(GraphicsDevice, width, height, false,
            GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
    }

    private void SetGameBufferToDraw()
    {
        ToDrawBuffer = GameBuffer;
        ToDrawPosition = Vector2.Zero;
    }

    // TODO: Подумать как вынести повторющийся код
    public void Update()
    {
        SetGameBufferToDraw();
        GraphicsDevice.SetRenderTarget(GameBuffer);
        GraphicsDevice.Clear(BackgroundColor);
        SpriteBatch.Begin();
        DrawGamePlay();
        SpriteBatch.End();
        GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawGamePlay()
    {
        var playerHead = Model.GetPlayerHead();
        var bounds = GetViewBounds(playerHead);

        for (int y = bounds.Top; y < bounds.Bottom; y++)
        {
            var cordY = (y - bounds.Top) * CellSize.Height;
            for (int x = bounds.Left; x < bounds.Right; x++)
            {
                var cordX = (x - bounds.Left) * CellSize.Width;
                var color = _cellToColor[Model.GetMapCell(x, y)];
                SpriteBatch.Draw(SquareTexture, new Vector2(cordX, cordY), CellSize, color);
            }
        }
    }

    private RenderTarget2D GetDrawMap()
    {
        var size = Math.Min(WindowWidth, WindowHeight);
        var buffer = CreateBuffer(size, size);

        var drawCordX = (WindowWidth - buffer.Width) / 2;
        var drawCordY = (WindowHeight - buffer.Height) / 2;
        
        MapPosition = new Vector2(drawCordX, drawCordY);
        
        var cellSize = size / Math.Min(Model.MapWidth, Model.MapHeight);
        var cell = new Rectangle(0, 0, cellSize, cellSize);

        GraphicsDevice.SetRenderTarget(buffer);
        GraphicsDevice.Clear(BackgroundColor);
        SpriteBatch.Begin();

        var map = Model.GetOriginalMap();

        for (int x = 0; x < Model.MapWidth; x++)
        {
            for (int y = 0; y < Model.MapHeight; y++)
            {
                var color = _cellToColor[map[x, y]];
                SpriteBatch.Draw(SquareTexture, new Vector2(x * cellSize, y * cellSize), cell, color);
            }
        }

        SpriteBatch.End();
        GraphicsDevice.SetRenderTarget(null);

        return buffer;
    }

    private Rectangle GetViewBounds(Point point)
    {
        var top = point.Y - ViewHeight / 2;
        var bottom = point.Y + ViewHeight / 2;
        var right = point.X + ViewWidth / 2;
        var left = point.X - ViewWidth / 2;

        NormalizeBounds(ref top, ref bottom, Model.MapHeight);
        NormalizeBounds(ref left, ref right, Model.MapWidth);

        return new Rectangle(left, top, bottom - top, right - left);
    }

    private void NormalizeBounds(ref int lowerBound, ref int upperBound, int maxValue)
    {
        if (lowerBound < 0)
        {
            upperBound -= lowerBound;
            lowerBound = 0;
        }
        else if (upperBound > maxValue)
        {
            lowerBound -= upperBound - maxValue;
            upperBound = maxValue;
        }
    }

    private readonly Dictionary<MapCell, Color> _cellToColor = new()
    {
        [MapCell.Empty] = Color.Black,
        [MapCell.Player] = Color.White,
        [MapCell.Snake] = Color.Red,
        [MapCell.Food] = Color.YellowGreen,
        [MapCell.Wall] = Color.Brown,
        [MapCell.Grass] = Color.Green
    };

    public Color BackgroundColor => _cellToColor[MapCell.Empty];

    public void UpdateMap() => MapBuffer = GetDrawMap();
}