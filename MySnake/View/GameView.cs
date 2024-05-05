using Microsoft.Xna.Framework.Graphics;

namespace MySnake.View;

public class GameView
{
    public GameView(int viewWidth, int viewHeight)
    {
        ViewWidth = viewWidth;
        ViewHeight = viewHeight;
    }
    
    private int ViewWidth { get; set; }
    private int ViewHeight { get; set; }
    public Texture2D SquareTexture { get; set; }
    public SpriteBatch SpriteBatch { get; set; }
}