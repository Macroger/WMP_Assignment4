
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
    class LineGenerator
    {
        private struct LineProperties
        {
            public double X1;
            public double X2;
            public double Y1;
            public double Y2;
            public Brush RandomBrush;

        };

        private LineProperties LineEnvelope;

        private volatile int _TailLength = 15;

        private List<Thread> ThreadPool = new List<Thread>();

        private readonly object LockObj = new object();

        private readonly object StopCommandLockObj = new object();

        private readonly object TailPersistenceLockObj = new object();

        private Canvas ActiveCanvas;

        private volatile bool _StopFlag = false;

        private Random RNG = new Random();

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

        private delegate void LineGenDelegate(LineProperties lp);
        private delegate void ThreadDelegate();
        public bool StopFlag
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

        public LineGenerator(Canvas IncommingCanvas)
        {
            LineEnvelope = new LineProperties();
            LineEnvelope.X1 = 0;
            LineEnvelope.X2 = 0;
            LineEnvelope.Y1 = 0;
            LineEnvelope.Y2 = 0;
            LineEnvelope.RandomBrush = System.Windows.Media.Brushes.Black; 
            ActiveCanvas = IncommingCanvas;
        }

        public void TaskMaster()
        {
            Queue<Line> myLineQue = new Queue<Line>();

            while(_StopFlag == false)
            {
                MessageBox.Show("_StopFlag: " + _StopFlag);
                LineEnvelope = LinePropertiesFiller(LineEnvelope, RNG);
                myLineQue.Enqueue(DrawLine(LineEnvelope));

                // This check is used to allow a specific number of lines to persist.
                if(myLineQue.Count >= _TailLength)
                {
                    RemoveLine(myLineQue.Dequeue());
                }


                Thread.Sleep(25);
            }
        }

        private void RemoveLine(Line LineToRemove)
        {
            // Using the Dispatcher to access the main UI thread.
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, (Action)(() =>
            {
                ActiveCanvas.Children.Remove(LineToRemove);
            }));
        }
        public void StartThreadSpawner()
        {
            Thread myThread = new Thread(TaskMaster);

            myThread.SetApartmentState(ApartmentState.STA);
            myThread.Start();
        }
        public void ThreadSpawner()
        {


            for (int i = 0; i < 10; i++)
            {
                LineEnvelope = LinePropertiesFiller(LineEnvelope, RNG);
                DrawLine(LineEnvelope);
                //    Thread.Sleep(75);
            }
            //while (_StopFlag == false)
            //{
            //    LineEnvelope = LinePropertiesFiller(LineEnvelope, RNG);
            //    DrawLine(LineEnvelope);
            //    Thread.Sleep(50);
            //}
        }
       

        //private LineProperties GenerateRandomStartPositions(Random RNG)
        //{
        //    double X1 = 0;
        //    double Y1 = 0;
        //    double X2 = 0;
        //    double Y2 = 0;

            
        //    RNG.Next(0, 9);
        //}

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
        private Line DrawLine(LineProperties lp)
        {
            Line myLine = new Line();

            myLine.Stroke = System.Windows.Media.Brushes.Black;
            myLine.X1 = lp.X1;
            myLine.Y1 = lp.Y1;
            myLine.X2 = lp.X2;
            myLine.Y2 = lp.Y2;

            MessageBox.Show("CheckAccess() shows: " + ActiveCanvas.Dispatcher.CheckAccess());
            // Using the Dispatcher to access the main UI thread.
            ActiveCanvas.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                ActiveCanvas.Children.Add(myLine);
            }));

            return myLine;

        }

        private LineProperties LinePropertiesFiller(LineProperties lp, Random RNG)
        {
            lp.RandomBrush = GetRandomBrushColour(RNG);
            lp.X1 += 4;
            lp.Y1 += 0;
            lp.X2 += 4;
            lp.Y2 += 0;

            return lp;
        }
    }
}
