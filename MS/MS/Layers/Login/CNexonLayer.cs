using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using reWZ.WZProperties;
using System.IO;
using System.Threading;
using MS.Common.Imaging;
using MS.Common.Net;
using MS.Common;
using System.Collections.Generic;

namespace MS.Layers.Login
{
    /// <summary>
    /// Layer that handles Nexon Logo
    /// </summary>
    public class CNexonLayer : CCLayer
    {
        private WZPointProperty origin;

        private CCTexture2D[] texture = new CCTexture2D[1024];
        private CCSprite[] sprite = new CCSprite[1024];
        private CCAnimation animation = new CCAnimation();
        private CCAnimate animate;

        private List<Microsoft.Xna.Framework.Graphics.Texture2D> frames = new List<Microsoft.Xna.Framework.Graphics.Texture2D>();
        private List<int> Width = new List<int>();
        private List<int> Height = new List<int>();
        private List<float> X = new List<float>();
        private List<float> Y = new List<float>();

        private int FrameCount { get; set; }

        public CNexonLayer()
        {
           
        }

        ~CNexonLayer()
        {
            texture = null;
            sprite = null;
            animation = null;
            frames = null;
            Width = null;
            Height = null;
            X = null;
            Y = null;
        }

        public override void OnEnter()
        {
            using (reWZ.WZFile ui = new reWZ.WZFile(AppDomain.CurrentDomain.BaseDirectory + @"/UI.wz", GameConstants.Variant, true))
            {
                var nexon = ui.MainDirectory["Logo.img"]["Nexon"];

                foreach (WZCanvasProperty canvas in nexon)
                {
                    frames.Add(CT2B.GetTexture(canvas.Value));

                    origin = (WZPointProperty)canvas["origin"];

                    Width.Add(canvas.Value.Width);
                    Height.Add(canvas.Value.Height);

                    X.Add(origin.Value.X);
                    Y.Add(origin.Value.Y);
                }
                
            }

            FrameCount = frames.Count;

            Console.WriteLine("X: {0} Y: {1}", X[0], Y[0]);
            Console.WriteLine("Width: {0} Height: {1}", Width[0], Height[0]);
            Console.WriteLine("Greater Version: {0}", GameConstants.GREATER_VERSION);

            for (int t = 0; t < FrameCount; t++)
            {
                texture[t] = new CCTexture2D();
                texture[t].InitWithTexture(frames[t]);

                if (t == FrameCount)
                    break;
            }

            for (int s = 0; s < FrameCount; s++)
            {
                sprite[s] = new CCSprite(texture[s]);
                sprite[s].SetPosition(X[s], Y[s]);
                sprite[s].SetTextureRect(new CCRect(0, 0, 800, 600));

                if (s == FrameCount)
                    break;
            }

            if (GameConstants.GREATER_VERSION)
            {

                for (int a = 0; a < FrameCount; a++)
                {
                    animation.AddSprite(sprite[a]);

                    if (a == FrameCount)
                        break;
                }

                animation.Loops = 1;
                animation.DelayPerUnit = .1f;

                animate = new CCAnimate(animation);
                animate.Duration = 7.0f;

                sprite[0].RunAction(animate);
            }

            AddChild(sprite[0]);

            MoveToWizet();

            base.OnEnter();
        }

        public override void OnExit()
        {
            Console.WriteLine("OnExit");
            GC.SuppressFinalize(this);
            frames.Clear();
            texture = null;
            sprite = null;
            animation = null;
            Width = null;
            Height = null;
            X = null;
            Y = null;
            base.OnExit();
        }

        private void MoveToWizet()
        {

            if (!GameConstants.GREATER_VERSION)
            {
                Console.WriteLine("Moved To Wizet None Greater");
                ScheduleOnce(TransitionOut, 1);
            }
            else
            {
                Console.WriteLine("Moved To Wizet Greater");
                ScheduleOnce(TransitionOut, 6);
            }
        }

        void TransitionOut(float delta)
        {

            CCLog.Log("Make Transition to Wizet Logo");

            CCTransitionScene transition = Fade;

            CCDirector.SharedDirector.ReplaceScene(transition);
        }

        #region Transitions

        CCTransitionScene FadeDown
        {
            get { return new CCTransitionFadeDown(1, CWizetLayer.Scene); }
        }

        CCTransitionScene FilpX
        {
            get { return new CCTransitionFlipX(1, CWizetLayer.Scene, CCTransitionOrientation.RightOver); }
        }

        CCTransitionScene Fade
        {
            get { return new CCTransitionFade(1, CWizetLayer.Scene, new CCColor3B(Microsoft.Xna.Framework.Color.White)); }
        }

        CCTransitionScene FlipAngular
        {
            get { return new CCTransitionFlipAngular(1, CWizetLayer.Scene, CCTransitionOrientation.DownOver); }
        }

        CCTransitionScene FadeTR
        {
            get { return new CCTransitionFadeTR(1, CWizetLayer.Scene); }
        }

        CCTransitionScene PageTurn
        {
            get { return new CCTransitionPageTurn(1, CWizetLayer.Scene, false); }
        }

        CCTransitionScene TurnOffTiles
        {
            get { return new CCTransitionTurnOffTiles(1, CWizetLayer.Scene); }
        }

        #endregion

        public static CCScene Scene
        {
            get
            {
                // 'scene' is an autorelease object.
                var scene = new CCScene();

                // 'layer' is an autorelease object.
                var layer = new CNexonLayer();

                // add layer as a child to scene
                scene.AddChild(layer);

                // return the scene
                return scene;

            }
        }
    }
}

