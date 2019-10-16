
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WMP_Assignment4
{
    class LineGeneratorManager
    {
        private volatile int _TailLength = 5;

        private List<Task> TaskPool = new List<Task>();

        private readonly object LockObj = new object();

        private readonly object StopCommandLockObj = new object();

        private readonly object PauseCommandLockObj = new object();

        private readonly object TailPersistenceLockObj = new object();

        private Canvas ActiveCanvas;

        private volatile bool _StopFlag = false;

        private volatile bool _PauseFlag = false;

        private string[] AllowedColours = new string[] {
            "Red",
            "Blue",
            "Green",
            "Yellow",
            "Aqua",
            "Teal",
            "Orange",
            "Black",
            "Purple",
            "Lavender"
        };

        public bool Stop
        {
            get
            {
                return _StopFlag;
            }
            set
            {
                lock(StopCommandLockObj)
                {
                    _StopFlag = value;
                }
            }
        }

        public bool Pause
        {
            get
            {
                return _PauseFlag;
            }
            set
            {
                lock (PauseCommandLockObj)
                {
                    _PauseFlag = value;
                }
            }
        }

        public int TailLength
        {
            get
            {
                return _TailLength;
            }
            set
            {
                lock (TailPersistenceLockObj)
                {
                    _TailLength = value;
                }
            }
        }

        public LineGeneratorManager(Canvas IncommingCanvas)
        {
            ActiveCanvas = IncommingCanvas;
        }


        public void StartThreadSpawner()
        {
            LineGenerator myLineGenerator = new LineGenerator();

            //Task.Run(() =>
            //{
            //    MessageBox.Show("Canvas Max Y: " +myLineGenerator.CanvasMaxY+ "\nCanvas Max X: " + myLineGenerator.CanvasMaxX);
            //});

            Task t = Task.Run(() =>
            {
                Queue<Line> LineQue = new Queue<Line>();
                int Counter = 0;

                while(_StopFlag == false)
                {

                    while(_PauseFlag == true)
                    {
                        Thread.Sleep(150);
                    }

                    ActiveCanvas.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, (Action)(() =>
                    {
                        Counter++;
                        Line myLine = new Line();
                        myLineGenerator.CalculateNewLinePoints(myLine);
                        myLine.Stroke = Brushes.Black;
                        ActiveCanvas.Children.Add(myLine);
                        LineQue.Enqueue(myLine);
                        

                        while(LineQue.Count >= (_TailLength + 1))
                        {
                            ActiveCanvas.Children.Remove(LineQue.Dequeue());
                        }
                        //if(LineQue.Count >= _TailLength)
                        //{
                        //    ActiveCanvas.Children.Remove(LineQue.Dequeue());
                        //}

                        //Task.Run(() =>
                        //{
                        //    MessageBox.Show(
                        //        "PointA Direction Trend: " + myLineGenerator.PointADirectionTrend +
                        //        "PointB Direction Trend: " + myLineGenerator.PointBDirectionTrend +
                        //        "\n");
                        //});


                    }));
                    Thread.Sleep(150);
                    

                }
                //for(int i = 0; i< 5; i++)
                //{
                //    ActiveCanvas.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(() =>
                //    {
                //        Line myLine = new Line();
                //        myLineGenerator.CalculateNewLinePoints(myLine);
                //        myLine.Stroke = Brushes.Black;
                //        ActiveCanvas.Children.Add(myLine);
                //    }));
                //}
                
            });

            TaskPool.Add(t);
        }

        /*
        **	Method Name:	GetRandomBrushColour()
        **	Parameters:		None.
        **	Return Values:	None.
        **	Description:	This method is used to select a random brush color from the set of brushes. The code is heavily inspired by code I found online at stackoverflow on October 14, 2019.
        **                  https://stackoverflow.com/questions/27549546/pick-wpf-random-brush-color. This code is designed to choose a random brush from the set of brushes using a form of reflection.
        */

        private Brush GetRandomBrushColour(Random RNG)
        {
            Brush Result = Brushes.Transparent;

            Type BrushesType = typeof(Brushes);

            PropertyInfo[] BrushProperties = BrushesType.GetProperties();

            int RandomNumber = RNG.Next(BrushProperties.Length);

            Result = (Brush)BrushProperties[RandomNumber].GetValue(null, null);

            return Result;
        }

    }
}
