using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MySnake.Controller;
using MySnake.Model;
using MySnake.View;

namespace MySnake;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GameModel _model;
    private GameView _view;
    private GameController _controller;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        _model = new GameModel(30, 30);
        _view = new GameView(GraphicsDevice, _model, 30, 30);
        _controller = new GameController(_model, _view);

        _model.StateChanged += _view.Update;
        _model.MapChanged += _view.UpdateMap;

        Window.KeyDown += _controller.KeyDown;
        Window.ClientSizeChanged += _view.SetWindowSize;
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _view.SpriteBatch = _spriteBatch;
        _view.SquareTexture = Content.Load<Texture2D>("white-square");
        _view.SetWindowSize(Window, EventArgs.Empty);
        _view.Update();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_view.BackgroundColor);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_view.ToDrawBuffer, _view.ToDrawPosition, Color.White);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}