using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MySnake.Model;
using Point = MySnake.Model.Point;

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
    private int WindowWidth { get; set; }
    private int WindowHeight { get; set; }
    private Rectangle CellSize { get; set; }
    public Texture2D SquareTexture { get; set; }
    public SpriteBatch SpriteBatch { get; set; }
    private GameModel Model { get; set; }

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
        var playerHead = Model.GetPlayerHead();
        var bounds = GetViewBounds(playerHead);

        SpriteBatch.Begin();
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

        SpriteBatch.End();
    }

    private Rectangle GetViewBounds(Point point)
    {
        var top = point.Y - ViewHeight / 2;
        var bottom = point.Y + ViewHeight / 2;
        var right = point.X + ViewWidth / 2;
        var left = point.X - ViewWidth / 2;

        NormalizeBounds(ref top, ref bottom);
        NormalizeBounds(ref left, ref right);

        return new Rectangle(left, top, bottom - top, right - left);
    }

    private void NormalizeBounds(ref int lowerBound, ref int upperBound)
    {
        if (lowerBound < 0)
        {
            upperBound -= lowerBound;
            lowerBound = 0;
        }
        else if (upperBound > Model.MapHeight)
        {
            lowerBound -= upperBound - Model.MapHeight;
            upperBound = Model.MapHeight;
        }
    }

    private readonly Dictionary<MapCell, Color> _cellToColor = new()
    {
        [MapCell.Empty] = Color.Black, [MapCell.Player] = Color.White,
        [MapCell.Snake] = Color.Red, [MapCell.Food] = Color.GreenYellow
    };
}