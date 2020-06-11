using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Diagnostics.Eventing.Reader;

namespace SnakeGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            GameWindow Game = new GameWindow(); Game.Run();
        }
    }
    
    public class GameWindow : OpenTK.GameWindow
    {
        Snake Snake;
        public GameWindow() : base(300, 300)
        {
            Title = "Snake Game";
            SetRender();
        }

        #region Overrides
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Snake = new Snake(Width, Height, 15);

        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            Snake.Update();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.ClearColor(Color.Black); GL.Clear(ClearBufferMask.ColorBufferBit);
            
            Snake.Draw();

            SwapBuffers();
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            Debug.Print(e.Key.ToString());
            Snake.Direction(e.Key.ToString());

        }
        #endregion
        private void SetRender()
        {
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.MatrixMode(MatrixMode.Projection);

            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, 0, 1);
        }
    }

    public class Snake
    {
        //Declarations
        int Width, Height, Scale;
        
        int Speed = 300;        
        long UpdateTimer = Environment.TickCount;

        String CurrentDirection = "Right";
        int xMove = 1, yMove=0; 
        List<Point> Segments = new List<Point>();
        Point Food;
        Random Random = new Random();
        bool Grow = false;

        //Constructor
        public Snake(int Width, int Height, int Scale)
        {
            this.Width = Width / Scale;
            this.Height = Height / Scale;
            this.Scale = Scale;

            Segments.Add(new Point(this.Width / 2, this.Height / 2));
            PickFood();
        }

        //Functions
        public void Draw()
        {
            foreach (Point p in Segments)
            {
                GL.Color3(Color.White);
                DrawBox(p);
            }
            GL.Color3(Color.Red);
            DrawBox(Food);
        }
        private void DrawBox(Point p)
        {
            GL.Begin(PrimitiveType.Polygon);

            GL.Vertex2(Calc(p.X), Calc(p.Y));
            GL.Vertex2(Calc(p.X) + Scale, Calc(p.Y));
            GL.Vertex2(Calc(p.X) + Scale, Calc(p.Y) + Scale);
            GL.Vertex2(Calc(p.X), Calc(p.Y) + Scale);

            GL.End();
        }

        public void Update()
        {
            if (Environment.TickCount > UpdateTimer + Speed)
            {
                UpdateTimer = Environment.TickCount;
                
                //move
                Point nSegment = new Point(Segments[0].X + xMove, Segments[0].Y + yMove);
                Segments.Insert(0, nSegment);
                if (Grow == false)
                {
                    Segments.RemoveAt(Segments.Count - 1);
                }
                else { Grow = false; }
                
                //eat
                if (Segments[0] == Food) { Grow = true; PickFood(); }

                //death
                bool death = false;
                //check edges
                if (Segments[0].X < 0 || Segments[0].X > Width || Segments[0].Y < 0 || Segments[0].Y > Height)
                {
                    death = true;
                }
                else
                {
                    //check body
                    for (int i = 1; i < Segments.Count; i++)
                    {
                        if(Segments[i] == Segments[0]) { death = true; break; }
                    }
                }

                if (death) { Segments.Clear(); Segments.Add(new Point(this.Width / 2, this.Height / 2)); }



            }
        }
        public void Direction(string dir)
        {

            switch (dir)
            {
                case "Left":
                    if (CurrentDirection != "Right")
                    {
                        xMove = -1; yMove = 0; CurrentDirection = dir;
                    }
                    break;

                case "Right":
                    if (CurrentDirection != "Left")
                    {
                        xMove = 1; yMove = 0; CurrentDirection = dir;
                    }
                    break;

                case "Up":
                    if (CurrentDirection != "Down")
                    {
                        xMove = 0; yMove = -1; CurrentDirection = dir;
                    }
                    break;

                case "Down":
                    if (CurrentDirection != "Up")
                    {
                        xMove = 0; yMove = 1; CurrentDirection = dir;
                    }
                    break;

                case "Space": Grow = true; break;
            }

        }

        private int Calc(int val) {  return val * Scale; }
        
        private void PickFood()
        {
            Food = new Point(Random.Next(0, Width), Random.Next(0, Height));
        }
    }
}
