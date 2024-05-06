using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MySnake.Model;

namespace MySnake.View;

public class GameView
{
    public GameView(GameModel model, int viewWidth, int viewHeight, int windowWidth, int windowHeight)
    {
        ViewWidth = viewWidth;
        ViewHeight = viewHeight;
        Model = model;
        SetWindowSize(windowWidth, windowHeight);
    }

    private int ViewWidth { get; set; }
    private int ViewHeight { get; set; }
    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    private Rectangle CellSize { get; set; }
    public Texture2D SquareTexture { get; set; }
    public SpriteBatch SpriteBatch { get; set; }
    public GameModel Model { get; set; }

    public void SetWindowSize(object sender, EventArgs eventArgs)
    {
        var window = sender as GameWindow;
        SetWindowSize(window.ClientBounds.Width, window.ClientBounds.Height);
    }

    public void SetWindowSize(int width, int height)
    {
        WindowWidth = width;
        WindowHeight = height;
        var cellWidth = WindowWidth / ViewWidth;
        var cellHeight = WindowHeight / ViewHeight;
        CellSize = new Rectangle(0, 0, cellWidth, cellHeight);
        
    }

    public void Update()
    {
        var player = Model.GetPlayerHead();

        // var top = Math.Min(player.Y - ViewHeight / 2, 0);
        // var bottom = Math.Max(player.Y + ViewHeight / 2, Model.MapHeight);
        // var right = Math.Max(player.X + ViewWidth / 2, Model.MapWidth);
        // var left = Math.Min(player.X - ViewWidth / 2, 0);
        var top = player.Y - ViewHeight / 2;
        var bottom = player.Y + ViewHeight / 2;
        var right = player.X + ViewWidth / 2;
        var left = player.X - ViewWidth / 2;

        if (top < 0)
        {
            bottom -= top;
            top = 0;
        }
        else if (bottom > Model.MapHeight)
        {
            top -= bottom - Model.MapHeight;
            bottom = Model.MapHeight;
        }

        if (left < 0)
        {
            right -= left;
            left = 0;
        }
        else if (right > Model.MapWidth)
        {
            left -= right - Model.MapWidth;
            right = Model.MapWidth;
        }


        SpriteBatch.Begin();
        for (int y = top; y < bottom; y++)
        {
            var cordY = (y - top) * CellSize.Height;
            for (int x = left; x < right; x++)
            {
                var cordX = (x - left) * CellSize.Width;
                var color = _cellToColor[Model.GetMapCell(x, y)];
                SpriteBatch.Draw(SquareTexture, new Vector2(cordX, cordY), CellSize, color);
            }
        }
        SpriteBatch.End();
    }

    private readonly Dictionary<MapCell, Color> _cellToColor = new()
        { [MapCell.Empty] = Color.Black, [MapCell.Player] = Color.White, [MapCell.Snake] = Color.Red };
}