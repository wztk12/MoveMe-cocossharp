using CocosSharp;

namespace MoveMe
{
    public class EndScene : CCScene
    {
        public EndScene(CCWindow mainWindow) : base(mainWindow)
        {
            CCLayerColor layer = new CCLayerColor(CCColor4B.Aquamarine);
            this.AddChild(layer);
        }

    }
}