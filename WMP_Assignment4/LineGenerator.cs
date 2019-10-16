using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WMP_Assignment4
{
    class LineGenerator
    {
        private struct PreviousPoint
        {
            public double X;
            public double Y;

        };

        private enum LineSpeeds
        {
            Default = 1,
            Low = 2,
            Medium = 4,
            High = 6
        }

        private enum DirectionTrends
        {
            None = -1,
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest            
        }

        PreviousPoint PreviousPointA = new PreviousPoint();
        PreviousPoint PreviousPointB = new PreviousPoint();
        
        private bool FirstLine = true;

        // These values were determined by observing the values of height and width of the canvas element. I used the properties menu of visual studio to do this in conjunction with the XAML designer/viewer.
        public double CanvasMaxX = 582;
        public double CanvasMaxY = 359;
        public double CanvasMinX = 10;
        public double CanvasMinY = 10;
        
        // This value is connected with the enum LineSpeeds, as the values contained there represent the valid values for this private int named LineSpeed. 
        // This value represents the speed (or rather, increment value) for the line movement.
        private double PointA_SpeedOfX = (double)LineSpeeds.Default;
        private double PointA_SpeedOfY = (double)LineSpeeds.Default;
        private double PointB_SpeedOfX = (double)LineSpeeds.Default;
        private double PointB_SpeedOfY = (double)LineSpeeds.Default;

        // Each point must have its own direction trend, so they can move independently of each other.
        public int PointADirectionTrend = (int)DirectionTrends.None;
        public int PointBDirectionTrend = (int)DirectionTrends.None;

        public LineGenerator()
        {

        }
        private void GenerateRandomStartingPoints(Line MyLine)
        {
            Random RNG = new Random();

            // Need to generate randomized starting positions for this line.
            MyLine.X1 = RNG.Next(0, (int)CanvasMaxX);
            MyLine.X2 = RNG.Next(0, (int)CanvasMaxX);
            
            MyLine.Y1 = RNG.Next(0, (int)CanvasMaxY);
            MyLine.Y2 = RNG.Next(0, (int)CanvasMaxY);

            // Rounding the results.
            MyLine.X1 = Math.Round(MyLine.X1, 0);
            MyLine.Y1 = Math.Round(MyLine.Y1, 0);

            MyLine.X2 = Math.Round(MyLine.X2, 0);
            MyLine.Y2 = Math.Round(MyLine.Y2, 0);


            if(MyLine.Y1 == MyLine.Y2 && MyLine.X1 == MyLine.X2)
            {
                // Points are identical, need to re-adjust.
                MyLine.X1 = RNG.Next(0, (int)CanvasMaxX);
                MyLine.X2 = RNG.Next(0, (int)CanvasMaxX);

                MyLine.Y1 = RNG.Next(0, (int)CanvasMaxY);
                MyLine.Y2 = RNG.Next(0, (int)CanvasMaxY);

                // Rounding the results.
                MyLine.X1 = Math.Round(MyLine.X1, 0);
                MyLine.Y1 = Math.Round(MyLine.Y1, 0);

                MyLine.X2 = Math.Round(MyLine.X2, 0);
                MyLine.Y2 = Math.Round(MyLine.Y2, 0);
            }

            // Recording the values so they can be referenced by future lines.
            PreviousPointA.X = MyLine.X1;
            PreviousPointA.Y = MyLine.Y1;

            PreviousPointB.X = MyLine.X2;            
            PreviousPointB.Y = MyLine.Y2;

        }

        private bool DetermineDirectionTrend(string WhichPoint)
        {
            PreviousPoint PreviousPoints = new PreviousPoint();

            int DirectionTrend = 0;

            bool DirectionTrendChanged = false;

            double NextPosition = 0;

            double X_MovementIncrement = 0;
            double Y_MovementIncrement = 0;

            Random RNG = new Random();

            // Temporarily generalizing the points so I only have to write one set of logic conditions.
            if(WhichPoint == "A")
            {
                DirectionTrend = PointADirectionTrend;
                PreviousPoints = PreviousPointA;
                X_MovementIncrement = PointA_SpeedOfX;
                Y_MovementIncrement = PointA_SpeedOfY;
            }
            else if(WhichPoint == "B")
            {
                DirectionTrend = PointBDirectionTrend;
                PreviousPoints = PreviousPointB;
                X_MovementIncrement = PointB_SpeedOfX;
                Y_MovementIncrement = PointB_SpeedOfY;
            }

            // This section provides the logic for when a point is on or beyond the edge of the screen. It provides the bouncing action of the lines.
            if(DirectionTrend == (int)DirectionTrends.North)
            {
                // Check if on the Northern edge.
                NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                if (NextPosition <= CanvasMinY)
                {
                    // Point is on Northern edge.


                    // Check if on eastern edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if(NextPosition >= CanvasMaxX)
                    {
                        // Point is on North Eastern edge.


                        // Point is along the north eastern edge, it needs to be pushed off. Redirect to SouthWest.
                        DirectionTrend = (int)DirectionTrends.SouthWest;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if on western edge.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // Point is on North Western edge.


                            // Point is along the north western edge, it needs to be pushed off.
                            DirectionTrend = (int)DirectionTrends.SouthEast;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point will be at Northern edge of screen but not Eastern or Western; Change its direction trend to SouthEast, South, or SouthWest.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthWest
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }

                    
                }
                else
                {
                    // Point is NOT on Northern edge.


                    // Check if on Western edge.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // Point is on Western edge.

                        // Redirect to NorthEast, East, or SouthEast.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East
                            };

                        // Applying a randomly selected direction trend.
                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if point is on Eastern edge.
                        NextPosition = PreviousPoints.X + X_MovementIncrement;
                        if (NextPosition >= CanvasMaxX)
                        {
                            // Point is on Eastern edge, redirect to NorthWest, West, or SouthWest.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.West
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
            }
            else if(DirectionTrend == (int)DirectionTrends.NorthEast)
            {
                // Checks if the next increment will put the Y point beyond the Northern edge of the canvas. 
                NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                if (NextPosition <= CanvasMinY)
                {
                    // Point is beyond the Northern Edge.

                    // Check if X is at the Eastern edge, that would indicate the current position is the top right.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // Point is at NorthEastern edge.
                        
                        // This point is either in the top right of the screen or beyond it. Redirect to SouthWest.
                        DirectionTrend = (int)DirectionTrends.South;
                        DirectionTrendChanged = true;
                    }
                    else
                    {   
                        // Checks if X is on the western edge.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if(NextPosition <= CanvasMinX)
                        {
                            // Point is at NorthWestern edge.

                            // This point is either in the top left of the screen or beyond it. Redirect to SouthEast.
                            DirectionTrend = (int)DirectionTrends.SouthEast;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point is at North edge.

                            // This point is either in the top of the screen or beyond it. Redirect to SouthEast, South, or SouthWest.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.SouthEast

                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                        
                    }
                }
                else
                {
                    // Check if the X coord is close to the eastern edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // Point is at/beyond the Eastern edge but NOT the Northern edge.

                        // This point is either at the right of the screen or beyond it. Redirect to SouthWest, West, NorthWest).
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.NorthWest
                            };

                        // Applying a randomly selected direction trend.
                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if point will be at western edge.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // Point is at/beyond the Western edge.

                            // X component will be beyond the western edge of the canvas. Redirect to NorthEast, East, or SouthEast.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.SouthEast
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.East)
            {
                // Check if near the right edge.
                NextPosition = PreviousPoints.X + X_MovementIncrement;
                if (NextPosition >= CanvasMaxX)
                {
                    // Checking if the Y is near the edge also. Checking if on the northern edge.
                    NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                    if (NextPosition <= CanvasMaxY)
                    {
                        // Line is stuck on an edge, need to push it off.
                        // Setting up a customized array to make it easier to randomly select one of the valid options in this situation.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.West,
                            };

                        // Applying a randomly selected direction trend.
                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;

                    }
                    else
                    {
                        // Checking if on the Southern edge.
                        NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                        if(NextPosition >= CanvasMaxY)
                        {
                            // Line is stuck on south edge, need to push it off.
                            // Setting up a customized array to make it easier to randomly select one of the valid options in this situation.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.West,
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // X component will be beyond the edge of the canvas.
                            // This point is either at the right of the screen or beyond it. Redirect to anything without East component (South, SouthWest, West, NorthWest, North).

                            // Setting up a customized array to make it easier to randomly select one of the valid options in this situation.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.North
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                       
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.SouthEast)
            {
                // Checks if the next increment will put the Y point at or beyond the southern edge of the canvas.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                if (NextPosition >= CanvasMaxY)
                {
                    // Check if X is also going to be over the edge, that would indicate the current position is the bottom right.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // X component will be beyond the edge of the canvas. This is happening at same time as Y component being over the edge. 
                        // This point is either in the bottom right of the screen or beyond it. Redirect to NorthWest.
                        DirectionTrend = (int)DirectionTrends.North;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if X is possibly going to be over the western(left) edge, that would indicate the current position is the bottom left.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // This point is either in the bottom left of the screen or beyond it. Redirect to North or NorthEast.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point will be at Southern edge of screen, change its direction trend to North, NorthEast, or NorthWest.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.NorthEast,
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                           
                    }
                }
                else
                {
                    // Check if the X coord is close to the eastern edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // X component will be beyond the edge of the canvas.  
                        // This point is either at the right of the screen or beyond it. Redirect to anything without East component (South, SouthWest, West, NorthWest, North).
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.North
                            };

                        // Applying a randomly selected direction trend.
                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if point is on the western edge.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMaxX)
                        {
                            // Point is on western edge, redirect to NorthEast, East, or SouthEast.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.SouthEast
                                
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.South)
            {
                // Checks if the next increment will put the Y point beyond the southern edge of the canvas. Movement is to the south.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                if (NextPosition >= CanvasMaxY)
                {
                    // Check if X is also going to be over the edge, that would indicate the current position is the bottom right.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // Point will be at south eastern edge of screen, change its direction to NorthWest.
                        DirectionTrend = (int)DirectionTrends.NorthWest;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if X is going to be over the south western edge, that would indicate the current position is the bottom left.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // Point is in south western corner. Redirect to NorthEast.
                            DirectionTrend = (int)DirectionTrends.NorthEast;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point will be at mid-Southern edge of screen, change its direction trend to North, NorthEast, NorthWest.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.NorthWest
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.SouthWest)
            {
                // Checks if the next increment will put the Y point beyond the southern edge of the canvas. Movement is to the bottom left.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;

                if (NextPosition >= CanvasMaxY)
                {
                    // Check if X is also going to be over the western edge, that would indicate the current position is the bottom left.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // X component will be beyond the edge of the canvas. This is happening at same time as Y component being over the edge. 
                        // This point is either in the bottom left of the screen or beyond it. Redirect to NorthEast.

                        DirectionTrend = (int)DirectionTrends.NorthEast;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if X is also going to be over the eastern edge, that would indicate the current position is the bottom right.
                        NextPosition = PreviousPoints.X + X_MovementIncrement;
                        if(NextPosition >= CanvasMaxX)
                        {
                            // Point is going to be in / beyond south eastern corner. Redirect to NorthWest.
                            DirectionTrend = (int)DirectionTrends.NorthWest;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point will be at Southern edge of screen, change its direction trend to North, NorthEast, or NorthWest.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.NorthWest
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
                else
                {
                    // Check if the X coord is close to the  western edge. Movement is to the bottom left.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // X component will be beyond the edge of the canvas.  
                        // This point is either at the left of the screen or beyond it. Redirect to NorthEast.
                        DirectionTrend = (int)DirectionTrends.NorthEast;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if the X coord is close to the eastern edge. Movement is to the bottom right.
                        NextPosition = PreviousPoints.X + X_MovementIncrement;
                        if(NextPosition >= CanvasMaxX)
                        {
                            // Point is on or beyond the eastern edge. NorthWest, West, or SouthWest.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.SouthWest
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.West)
            {
                // Movement is to the left. 
                NextPosition = PreviousPoints.X - X_MovementIncrement;
                if (NextPosition <= CanvasMinX)
                {
                    // Check if on or beyond northern edge.
                    NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                    if(NextPosition <= CanvasMinY)
                    {
                        // Point is on or beyond Northern edge at same time as beyond Western edge. Redirect to SouthEast.
                        DirectionTrend = (int)DirectionTrends.SouthEast;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if on or beyond southern edge.
                        NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                        if(NextPosition >= CanvasMaxY)
                        {
                            // Point is at or beyond south edge while also beyond western edge. Redirect to NorthEast.
                            DirectionTrend = (int)DirectionTrends.NorthEast;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point is NOT beyond south or north edges, just western. Redirect to NorthEast, East, or SouthEast.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
                else
                {
                    // The point is NOT beyond the western edge. Check if its current riding along one of the walls.
                    
                    // Check if point is riding along / beyond northern edge.
                    NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                    if(NextPosition <= CanvasMinY)
                    {
                        // Point will be on the northern edge. Redirect to SouthWest, South, or SouthEast.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.South
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if point is riding along / beyond southern edge.
                        NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                        if (NextPosition >= CanvasMaxY)
                        {
                            // Point will be on the southern edge. Redirect to NorthWest, North, or NorthEast.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.North
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.NorthWest)
            {
                // Movement is to the top left. Check if position would be beyond northern edge of canvas.
                NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                if (NextPosition <= CanvasMinY)
                {
                    // Point is at / beyond the Northern edge.

                    // Check if X is going to be at/over the Western edge.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // Point is at/beyond NorthWest edge.

                        // This point is either in the top left of the screen or beyond it. Redirect to SouthEast.
                        DirectionTrend = (int)DirectionTrends.SouthEast;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Point will be at Northern edge. Redirect to SouthWest, South, or SouthEast.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.SouthWest
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                }
                else
                {
                    // Point is NOT beyond Northern edge.


                    // Check if the X coord is close to the Western edge.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // Point is at/beyond the Western edge.

                        // This point is either at the left of the screen or beyond it. Redirect to SouthEast, East, NorthEast.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.NorthEast
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                }
            }

            // Applying the new direction trend to the appropriate point.
            if (WhichPoint == "A")
            {
                PointADirectionTrend = DirectionTrend;
            }
            else if (WhichPoint == "B")
            {
                PointBDirectionTrend = DirectionTrend;
            }

            return DirectionTrendChanged;
        }

        public void CalculateNewLinePoints(Line MyLine)
        {
            if(FirstLine == true)
            {
                FirstLine = false;

                GenerateRandomStartingPoints(MyLine);
                DetermineDirectionTrend("A");
                DetermineDirectionTrend("B");
                RandomizeSpeed("A");
                RandomizeSpeed("B");
            }
            else
            {
                if(DetermineDirectionTrend("A"))
                {
                    RandomizeSpeed("A");
                }

                if(DetermineDirectionTrend("B"))
                {
                    RandomizeSpeed("B");
                }

                ProcessMovement(MyLine);
            }
        }

        private void RandomizeSpeed(string WhichPoint)
        {
            Random RNG = new Random();

            if(WhichPoint == "A")
            {
                PointA_SpeedOfX = RNG.Next((int)LineSpeeds.Default, (int)LineSpeeds.High);
                PointA_SpeedOfY = RNG.Next((int)LineSpeeds.Default, (int)LineSpeeds.High);
            }
            else if(WhichPoint == "B")
            {
                PointB_SpeedOfX = RNG.Next((int)LineSpeeds.Default, (int)LineSpeeds.High);
                PointB_SpeedOfY = RNG.Next((int)LineSpeeds.Default, (int)LineSpeeds.High);
            }
        }

        private void ProcessMovement(Line MyLine)
        {
            double PointA_Xpos = 0;
            double PointA_Ypos = 0;

            double PointB_Xpos = 0;
            double PointB_Ypos = 0;

            // PointADirectionTrend Section:

            if (PointADirectionTrend == (int)DirectionTrends.None)
            {
                Random RNG = new Random();
                PointADirectionTrend = RNG.Next(0, (int)DirectionTrends.NorthWest);
            }
            
            if (PointADirectionTrend == (int)DirectionTrends.North)
            {
                PointA_Ypos = PreviousPointA.Y - PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.NorthEast)
            {
                PointA_Xpos = PreviousPointA.X + PointA_SpeedOfX;
                PointA_Ypos = PreviousPointA.Y - PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.East)
            {
                PointA_Xpos = PreviousPointA.X + PointA_SpeedOfX;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.SouthEast)
            {
                PointA_Xpos = PreviousPointA.X + PointA_SpeedOfX;
                PointA_Ypos = PreviousPointA.Y + PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.South)
            {
                PointA_Ypos = PreviousPointA.Y + PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.SouthWest)
            {
                PointA_Xpos = PreviousPointA.X - PointA_SpeedOfX;
                PointA_Ypos = PreviousPointA.Y + PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.West)
            {
                PointA_Xpos = PreviousPointA.X - PointA_SpeedOfX;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.NorthWest)
            {
                PointA_Xpos = PreviousPointA.X - PointA_SpeedOfX;
                PointA_Ypos = PreviousPointA.Y - PointA_SpeedOfY;
            }


            if (PointBDirectionTrend == (int)DirectionTrends.None)
            {
                // This is an error condition; I do not anticipate it occuring, but incase it does I think a graceful way to handle it is to adjust the PointBDirectionTrend to a better value.
                Random RNG = new Random();
                PointBDirectionTrend = RNG.Next(0, (int)DirectionTrends.NorthWest);
            }
            
            if(PointBDirectionTrend == (int)DirectionTrends.North)
            {
                PointB_Ypos = PreviousPointB.Y - PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.NorthEast)
            {
                PointB_Xpos = PreviousPointB.X + PointB_SpeedOfX;
                PointB_Ypos = PreviousPointB.Y - PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.East)
            {
                PointB_Xpos = PreviousPointB.X + PointB_SpeedOfX;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.SouthEast)
            {
                PointB_Xpos = PreviousPointB.X + PointB_SpeedOfX;
                PointB_Ypos = PreviousPointB.Y + PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.South)
            {
                PointB_Ypos = PreviousPointB.Y + PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.SouthWest)
            {
                PointB_Xpos = PreviousPointB.X - PointB_SpeedOfX;
                PointB_Ypos = PreviousPointB.Y + PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.West)
            {
                PointB_Xpos = PreviousPointB.X - PointB_SpeedOfX;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.NorthWest)
            {
                PointB_Xpos = PreviousPointB.X - PointB_SpeedOfX;
                PointB_Ypos = PreviousPointB.Y - PointB_SpeedOfY;
            }

            PointA_Xpos = Math.Round(PointA_Xpos, 0);
            PointA_Ypos = Math.Round(PointA_Ypos, 0);
            PointB_Xpos = Math.Round(PointB_Xpos, 0);
            PointB_Ypos = Math.Round(PointB_Ypos, 0);

            MyLine.X1 = PointA_Xpos;
            MyLine.Y1 = PointA_Ypos;

            MyLine.X2 = PointB_Xpos;
            MyLine.Y2 = PointB_Ypos;

            // Recording the new values as the previous points.
            PreviousPointA.X = PointA_Xpos;
            PreviousPointA.Y = PointA_Ypos;

            PreviousPointB.X = PointB_Xpos;
            PreviousPointB.Y = PointB_Ypos;
        }
    }
}
