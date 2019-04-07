using CocosSharp;
using MoveMe;
using System;
namespace MoveMe
{
    public class EndScene : CCScene
    {
        string statistics, time, accuracy, actualCoinsCollected;
        decimal hits;
        
        public EndScene(CCWindow mainWindow, int completionTime, decimal touches, decimal misses, int deaths, float distance, string coinsCollected) : base(mainWindow)
        {
            time = completionTime.ToString() + " s";
            hits = touches - misses;
            actualCoinsCollected = coinsCollected.Split(" ")[1];
            accuracy = (Math.Round((hits/touches)*100, 2)).ToString() + "%";
            statistics = "Time: " + time + "\n" + "Accuracy: " + accuracy + "\n" + "Touches: " + touches + "\n" 
                 + "Misses: " + misses + "\n" + "Deaths: " + deaths + "\n" + "Distance travelled: " + Math.Round(distance, 2) + "\n"
                 + "Coins Collected: " + actualCoinsCollected + "\n";
            CCLayerColor layer = new CCLayerColor(CCColor4B.Aquamarine);
            CCLabel label = new CCLabel(statistics, "arial", 22);
            label.Color = CCColor3B.Black;
            this.AddChild(layer);
            label.Position = layer.VisibleBoundsWorldspace.Center;
            layer.AddChild(label);
        }

    }
}