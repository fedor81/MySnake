using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MySnake.Model;
using MySnake.View;

namespace MySnake.Controller;

public class GameController
{
    public GameController(GameModel model, GameView view)
    {
        Model = model;
        View = view;
    }

    public GameModel Model { get; set; }
    public GameView View { get; set; }

    public void KeyDown(object sender, InputKeyEventArgs e)
    {
        switch (e.Key)
        {
            case Keys.A:
                Model.MovePlayer(Direction.Left);
                break;
            case Keys.W:
                Model.MovePlayer(Direction.Up);
                break;
            case Keys.D:
                Model.MovePlayer(Direction.Right);
                break;
            case Keys.S:
                Model.MovePlayer(Direction.Down);
                break;
            case Keys.M:
                View.ShowOrCloseMap();
                break;
        }
    }
}