/*
**  File Name:      LineGeneratorManager.cs
**	Project Name:	WMP_Assignment4
**	Author:         Matthew G. Schatz
**  Date:           October 17, 2019
**	Description:	This fils contains the source code for the LineGeneratorManager class. This class is a manager for the LineGenerator class. It will spawn tasks that use LineGenerator to help create lines which will be drawn on
**                  the canvas referenced in ActiveCanvas.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WMP_Assignment4
{
    class LineGeneratorManager
    {
        // This int is used to determine the length of the trails. It is marked volatile to force any changes to it to be atomic (not allowed to be interrupted by the operating system scheduler). This makes it thread-safe.
        private volatile int _TailLength = 1;

        // This int is used to determine the time span between iterations of the line generation thread. This determines how fast the lines appear on the screen, with lower times equaling faster line generation.
        private volatile int _SpawnTimeInMilliSeconds = 75;

        // This bool is used to stop the threads and allow them to complete / end.
        private volatile bool _StopFlag = false;

        // This bool is used to pause the tasks in their work. Resuming is possible by toggling the flag back to false.
        private volatile bool _PauseFlag = false;

        // This list is used to hold references to the tasks as they run.
        private List<Task> TaskPool = new List<Task>();

        // This object is used to act as a lock object, protecting a critical region that may come into contention from the various tasks running.
        private readonly object StopCommandLockObj = new object();

        // This object is used to act as a lock object, protecting a critical region that may come into contention from the various tasks running.
        private readonly object PauseCommandLockObj = new object();

        // This object is used to act as a lock object, protecting a critical region that may come into contention from the various tasks running.
        private readonly object TailPersistenceLockObj = new object();

        // This object is used to act as a lock object, protecting a critical region that may come into contention from the various tasks running.
        private readonly object SpawnTimeLockObj = new object();

        // This variable is used to hold a reference to the canvas that the lines are being drawn on.
        private Canvas ActiveCanvas;

        // This list is used to hold a variety of colours, which is used to select a random colour for each thread spawned.
        private List<Brush> myBrushList = new List<Brush>()
        {
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

        // Public property connected to _StopFlag. Uses a lock object.
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

        // Public property connected to _PauseFlag. Uses a lock object.
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

        // Public property connected to _TailLength. Uses a lock object.
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

        // Public property connected to _SpawnTimeInMilliSeconds. Has basic validation and uses a lock object.
        public int SpawnTimeInMilliSeconds
        {
            get 
            {
                return _SpawnTimeInMilliSeconds;
            }
            set 
            {
                lock(SpawnTimeLockObj)
                {
                    if (value < 1500 && value > 0)
                    {
                        _SpawnTimeInMilliSeconds = value;
                    }
                }
                
            }
        }

        /*
        **	Method Name:	LineGeneratorManager()
        **	Parameters:		Canvas IncommingCanvas: This entity holds a reference to the canvas being used to draw lines.
        **	Return Values:	None. 
        **	Description:	Basic constructor, passes a reference of the canvas to draw to.
        */
        public LineGeneratorManager(Canvas IncommingCanvas)
        {
            ActiveCanvas = IncommingCanvas;
        }

        /*
        **	Method Name:	ShutdownAllTasks()
        **	Parameters:		None.
        **	Return Values:	Void.
        **	Description:	This method is used to cause all the worker tasks to end/close. It checks to see if each has closed and if not makes a list of "non responsive tasks" which will be iterated through and forced to close.
        */
        public void ShutdownAllTasks()
        {
            _StopFlag = true;

            foreach(Task t in TaskPool)
            {
                t.Wait(_SpawnTimeInMilliSeconds + 10);
            }

            // Since all tasks have now shut down TaskPool can be cleared.
            TaskPool.Clear();
        }

        /*
        **	Method Name:	HowManyThreadSpawnersActive()
        **	Parameters:		None.
        **	Return Values:	int: This method returns an int representing how many task spawners are currently active.
        **	Description:	This method returns an int representing how many task spawners are currently active.
        */
        public int HowManyThreadSpawnersActive()
        {
            return TaskPool.Count;
        }

        
        /*
        **	Method Name:	StartTaskSpawner()
        **	Parameters:		None.
        **	Return Values:	Void.
        **	Description:	This method is used to spawn (start/create) a worker task that will continue to loop until the _StopFlag is changed to true.
        **                  The task can be paused with the _PauseFlag. When operating in active mode it constantly generates lines and passes them to the UI thread's dispatcher.
        **                  The task will allow lines to remain on the screen according to the _TailLength variable, lines beyond this value will be removed.
        **                  After the line has been drawn the task sleeps for a time determined by the value of _SleepTimeInMilliSeconds. 
        **                  Also, a reference to the task will be placed into the list called TaskPool, so it can be referenced later.
        */
        public void StartTaskSpawner()
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
                        Thread.Sleep(_SpawnTimeInMilliSeconds);
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
