using CocosSharp;
using MoveMe;
using System;
using System.Collections.Generic;
using System.IO;
namespace MoveMe
{
    public class EndScene : CCScene
    {
        string statistics, time, accuracy, actualCoinsCollected, nextLevel, currentLevel;
        decimal hits;
        CCSprite navButton;
        CCDirector director;
        CCWindow mainWindow;
        
        public EndScene(CCWindow mainWindow, CCDirector director, string nextLevel, decimal completionTime, decimal touches, decimal misses, int deaths, float distance, string coinsCollected) : base(mainWindow)
        {
            this.nextLevel = nextLevel;
            this.director = director;
            this.mainWindow = mainWindow;
            time = Math.Round(completionTime, 1) + " s";
            hits = touches - misses;
            actualCoinsCollected = coinsCollected.Split(" ")[1];
            var env = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/MoveMe";
            Directory.CreateDirectory(env);
            var path = Path.Combine(env, "statistics.txt");
            accuracy = (Math.Round((hits/touches)*100, 2)).ToString() + "%";
            statistics = "Time: " + time + "\n" + "Accuracy: " + accuracy + "\n" + "Touches: " + touches + "\n" 
                 + "Misses: " + misses + "\n" + "Deaths: " + deaths + "\n" + "Distance travelled: " + Math.Round(distance, 2) + "\n"
                 + "Coins Collected: " + actualCoinsCollected + "\n";
            if (nextLevel == "GameScene1")
            {
                currentLevel = "Buttons\n";
            }
            else if (nextLevel == "GameScene2")
            {
                currentLevel = "Swipe\n";
            }
            else if (nextLevel == "GameScene3")
            {
                currentLevel = "Analog\n";
            }
            else if (nextLevel == "GameScene")
            {
                currentLevel = "Screen Position\n";
            }
            else
            {
                currentLevel = "Unknown\n";
            }

            File.AppendAllText(path, currentLevel + statistics + DateTime.Now + "\n\n");
            CCLayerColor layer = new CCLayerColor(CCColor4B.Aquamarine);
            CCLabel label = new CCLabel(statistics, "arial", 22);
            label.Color = CCColor3B.Black;
            this.AddChild(layer);
            label.Position = layer.VisibleBoundsWorldspace.Center;
            layer.AddChild(label);
   
            navButton = new CCSprite("images/navButton.png");
            navButton.Position = new CCPoint(190, 30);
            if (nextLevel != "GameScene")
            {
                layer.AddChild(navButton);
            }
            else
            {
                CCLabel thanks = new CCLabel("Thank you for taking part in the study!", "arial", 22);
                thanks.Position = navButton.Position;
                thanks.Color = CCColor3B.Blue;
                layer.AddChild(thanks);
            }
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            layer.AddEventListener(touchListener);
        }

        
        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                if (this.navButton.BoundingBox.ContainsPoint(touches[0].Location))
                {
                    if (nextLevel == "GameScene1")
                    {
                        var scene = new GameScene1(mainWindow, director);
                        director.ReplaceScene(scene);
                    }
                    else if (nextLevel == "GameScene2")
                    {
                        var scene = new GameScene2(mainWindow, director);
                        director.ReplaceScene(scene);
                    }
                    else if (nextLevel == "GameScene3")
                    {
                        var scene = new GameScene3(mainWindow, director);
                        director.ReplaceScene(scene);
                    }

                }
            }
        }
    }
}