using CocosSharp;
using MoveMe;
using System.Collections.Generic;

namespace MoveMe
{
    public class EndScene : CCScene
    {
        string statistics, time, accuracy, deaths;
        decimal hits;
        
        public EndScene(CCWindow mainWindow, int completionTime, decimal touches, decimal misses, int deaths) : base(mainWindow)
        {
            time = completionTime.ToString();
            hits = touches - misses; 
            accuracy = (System.Math.Round((hits/touches)*100, 2)).ToString() + "%";
            statistics = "Time: " + time + "\n" + "Accuracy: " + accuracy + "\n"
                + "Touches: " + touches + "\n" + "Misses: " + misses + "\n" + "Deaths: " + deaths + "\n";
            CCLayerColor layer = new CCLayerColor(CCColor4B.Aquamarine);
            CCLabel label = new CCLabel(statistics, "fonts/Markerfelt", 22);
            label.Color = CCColor3B.Black;
            this.AddChild(layer);
            label.Position = layer.VisibleBoundsWorldspace.Center;
            layer.AddChild(label);
        }

    }
}