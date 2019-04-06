using CocosSharp;
using MoveMe;

namespace MoveMe
{
    public class EndScene : CCScene
    {
        string time;
        public EndScene(CCWindow mainWindow, float completionTime) : base(mainWindow)
        {
            time = completionTime.ToString();
            CCLayerColor layer = new CCLayerColor(CCColor4B.Aquamarine);
            CCLabel label = new CCLabel("Time: "+ time, "fonts/Markerfelt", 22);
            label.Color = CCColor3B.Black;
            this.AddChild(layer);
            label.Position = layer.VisibleBoundsWorldspace.Center;
            layer.AddChild(label);
        }

    }
}