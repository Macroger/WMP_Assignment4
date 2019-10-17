
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

        private volatile int _SpawnTimeInMilliSeconds = 150;

        private volatile bool _StopFlag = false;

        private volatile bool _PauseFlag = false;

        private List<Task> TaskPool = new List<Task>();

        private readonly object StopCommandLockObj = new object();

        private readonly object PauseCommandLockObj = new object();

        private readonly object TailPersistenceLockObj = new object();

        private Canvas ActiveCanvas;

        private List<Brush> myBrushList = new List<Brush>()
        {
            Brushes.Transparent,
            Brushes.Red,
            Brushes.Green,
            Brushes.MediumOrchid,
            Brushes.Goldenrod,
            Brushes.IndianRed,
            Brushes.Teal,
            Brushes.MediumPurple,
            Brushes.Magenta,
            Brushes.SpringGreen,
            Brushes.SteelBlue,
            Brushes.SeaGreen,
            Brushes.DarkOrange
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

        public int SpawnTimeInMilliSeconds
        {
            get 
            {
                return _SpawnTimeInMilliSeconds;
            }
            set 
            {
                if (value < 1500 && value > 0)
                {
                    _SpawnTimeInMilliSeconds = value;
                }
            }
        }


        public LineGeneratorManager(Canvas IncommingCanvas)
        {
            ActiveCanvas = IncommingCanvas;
        }


        public void CloseAllThreads()
        {
            _StopFlag = true;

            List<Task> NonRespondingTasksList = new List<Task>();

            foreach(Task t in TaskPool)
            {
                // Wait for the equivalent of one cycle for the thread to end, 
                if(t.Wait(_SpawnTimeInMilliSeconds) == false)
                {
                    NonRespondingTasksList.Add(t);
                }
            }
            
            // For any tasks that refuse to shut down properly a forced shut down is in order.
            for(int i = 0; i < NonRespondingTasksList.Count; i++)
            {
                NonRespondingTasksList[i].Dispose();
            }

            // Since all tasks have now shut down TaskPool can be cleared.
            TaskPool.Clear();
        }


        public void StartThreadSpawner()
        {
            LineGenerator myLineGenerator = new LineGenerator();
            Brush ThreadColour = GetRandomBrushColour();

            // Spawning the task (thread) that will perform the work of constantly generating new lines and sending them to the UI thread. Can be stopped via _StopFlag.
            Task t = Task.Run(() =>
            {
                Queue<Line> LineQue = new Queue<Line>();

                while (_StopFlag == false)
                {
                    while(_PauseFlag == true)
                    {
                        Thread.Sleep(150);
                    }

                    // I am putting this check in to ensure that no more lines are drawn once the stop flag has tripped.
                    if(_StopFlag == true)
                    {
                        break;
                    }

                    // This use of the dispatcher is necessary to have the UI thread access and told to draw the lines that we are generating.
                    ActiveCanvas.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, (Action)(() =>
                    {
                        // Generate a new line.
                        Line myLine = new Line();
                        myLineGenerator.CalculateNewLinePoints(myLine);
                        myLine.Stroke = ThreadColour;

                        // Add the line to the collection of elements for the ActiveCanvas.
                        ActiveCanvas.Children.Add(myLine);

                        // Add the line to the que of lines.
                        LineQue.Enqueue(myLine);
                        
                        // This code clears up the extra lines, once the desired number of tails has been reached.
                        while(LineQue.Count >= (_TailLength + 1))
                        {
                            ActiveCanvas.Children.Remove(LineQue.Dequeue());
                        }
                    }));

                    // This sleep command determines how fast the lines are generated, reducing the time here equals faster line generation.
                    Thread.Sleep(_SpawnTimeInMilliSeconds);
                }
            });

            TaskPool.Add(t);

        }

        /*
        **	Method Name:	GetRandomBrushColour()
        **	Parameters:		None.
        **	Return Values:	None.
        **	Description:	This method is used to select a random brush color from the set of brushes. It makes use of a list loaded with colour brushes and chooses one at random.
        */
        private Brush GetRandomBrushColour()
        {
            Random RNG = new Random();

            Brush Result = Brushes.Transparent;

            int RandomBrushIndex = RNG.Next(myBrushList.Count);

            Result = myBrushList[RandomBrushIndex];

            return Result;
        }
    }
}
