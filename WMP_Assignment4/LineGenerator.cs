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
                    // Check if on eastern edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if(NextPosition >= CanvasMaxX)
                    {
                        // Point is along the north eastern edge, it needs to be pushed off.
                        // Setting up a customized array to make it easier to randomly select one of the valid options in this situation.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.South
                            };

                        // Applying a randomly selected direction trend.
                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;

                    }
                    else
                    {
                        // Check if on western edge.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // Point is along the north western edge, it needs to be pushed off.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.East
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point will be at Northern edge of screen but not Eastern or Western; Change its direction trend to something that doesn't have a North component.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthWest,
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
                // Checks if the next increment will put the Y point beyond the edge of the canvas. 
                NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                if (NextPosition <= CanvasMinY)
                {
                    // Check if X is also at the edge, that would indicate the current position is the top right.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // X component will be beyond the edge of the canvas. This is happening at same time as Y component being over the edge. 
                        // This point is either in the top right of the screen or beyond it. Redirect to South, SouthWest, or West.
                        DirectionTrend = RNG.Next((int)DirectionTrends.South, (int)DirectionTrends.West);
                    }
                    else
                    {
                        // Point will be at Northern edge of screen, change its direction trend to something that doesn't have a North component.
                        DirectionTrend = RNG.Next((int)DirectionTrends.East, (int)DirectionTrends.West);
                    }

                    DirectionTrendChanged = true;

                }
                else
                {
                    // Check if the X coord is close to the edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
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
                // Checks if the next increment will put the Y point beyond the edge of the canvas. Movement is to the south.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;

                if (NextPosition >= CanvasMinY)
                {
                    // Check if X is also going to be over the edge, that would indicate the current position is the bottom right.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;

                    if (NextPosition >= CanvasMaxX)
                    {
                        // X component will be beyond the edge of the canvas. This is happening at same time as Y component being over the edge. 
                        // This point is either in the bottom right of the screen or beyond it. Redirect to North, NorthWest, or West.
                        
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.West
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                    }
                    else
                    {
                        // Point will be at Southern edge of screen, change its direction trend to something that doesn't have a South component.

                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                    }

                    DirectionTrendChanged = true;

                }
                else
                {
                    // Check if the X coord is close to the edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
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
            else if (DirectionTrend == (int)DirectionTrends.South)
            {
                // Checks if the next increment will put the Y point beyond the edge of the canvas. Movement is to the south.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                if (NextPosition >= CanvasMaxY)
                {
                    // Check if X is also going to be over the edge, that would indicate the current position is the bottom right.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;

                    // Point will be at Southern edge of screen, change its direction trend to something that doesn't have a South component.

                    int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East
                            };

                    DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                    DirectionTrendChanged = true;


                }
            }
            else if (DirectionTrend == (int)DirectionTrends.SouthWest)
            {
                // Checks if the next increment will put the Y point beyond the edge of the canvas. Movement is to the bottom left.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;

                if (NextPosition >= CanvasMaxY)
                {
                    // Check if X is also going to be over the edge, that would indicate the current position is the bottom left.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // X component will be beyond the edge of the canvas. This is happening at same time as Y component being over the edge. 
                        // This point is either in the bottom left of the screen or beyond it. Redirect to North, NorthEast, or East.

                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                    }
                    else
                    {
                        // Point will be at Southern edge of screen, change its direction trend to something that doesn't have a South component.

                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                    }

                    DirectionTrendChanged = true;
                }
                else
                {
                    // Check if the X coord is close to the edge. Movement is to the left so its a subtract operation.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // X component will be beyond the edge of the canvas.  
                        // This point is either at the left of the screen or beyond it. Redirect to anything without West component (South, SouthEast, East, NorthEast, North).

                        // Setting up a customized array to make it easier to randomly select one of the valid options in this situation.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.North
                            };

                        // Applying a randomly selected direction trend.
                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.West)
            {
                // Movement is to the left. 
                NextPosition = PreviousPoints.X - X_MovementIncrement;
                if (NextPosition <= CanvasMinX)
                {
                    // Point is at Western edge of screen, change its direction trend to something that doesn't have a West component.
                    int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.South
                            };

                    DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                    DirectionTrendChanged = true;
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.NorthWest)
            {
                // Movement is to the top left. Check if position would be beyond top edge of canvas.
                NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                if (NextPosition <= CanvasMinY)
                {
                    // Check if X is also going to be over the edge, that would indicate the current position is the top left.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // X component will be beyond the edge of the canvas. This is happening at same time as Y component being over the edge. 
                        // This point is either in the top left of the screen or beyond it. Redirect to South, SouthEast, or East.

                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.East
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];

                    }
                    else
                    {
                        // Point will be at Northern edge of screen, change its direction trend to something that doesn't have a North component.

                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.SouthWest,
                                (int)DirectionTrends.West
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                    }
                    DirectionTrendChanged = true;
                }
                else
                {
                    // Check if the X coord is close to the edge. Movement is to the left so its a subtract operation.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // X component will be beyond the edge of the canvas.  
                        // This point is either at the left of the screen or beyond it. Redirect to anything without West component (South, SouthEast, East, NorthEast, North).

                        // Setting up a customized array to make it easier to randomly select one of the valid options in this situation.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.North
                            };

                        // Applying a randomly selected direction trend.
                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                }
            }
            else if(DirectionTrend == (int)DirectionTrends.None)
            {
                DirectionTrend = RNG.Next(0, (int)DirectionTrends.NorthWest);
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
            double NewX1 = 0;
            double NewY1 = 0;
            double NewX2 = 0;
            double NewY2 = 0;

            // PointADirectionTrend Section:

            if (PointADirectionTrend == (int)DirectionTrends.None)
            {
                // This is an error condition; I do not anticipate it occuring, but incase it does I think a graceful way to handle it is to adjust the PointADirectionTrend to a better value.
                Random RNG = new Random();
                PointADirectionTrend = RNG.Next(0, (int)DirectionTrends.NorthWest);
            }
            else if (PointADirectionTrend == (int)DirectionTrends.North)
            {
                NewY1 = PreviousPointA.Y - PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.NorthEast)
            {
                NewX1 = PreviousPointA.X + PointA_SpeedOfX;
                NewY1 = PreviousPointA.Y - PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.East)
            {
                NewX1 = PreviousPointA.X + PointA_SpeedOfX;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.SouthEast)
            {
                NewX1 = PreviousPointA.X + PointA_SpeedOfX;
                NewY1 = PreviousPointA.Y + PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.South)
            {
                NewY1 = PreviousPointA.Y + PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.SouthWest)
            {
                NewX1 = PreviousPointA.X - PointA_SpeedOfX;
                NewY1 = PreviousPointA.Y + PointA_SpeedOfY;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.West)
            {
                NewX1 = PreviousPointA.X - PointA_SpeedOfX;
            }
            else if (PointADirectionTrend == (int)DirectionTrends.NorthWest)
            {
                NewX1 = PreviousPointA.X - PointA_SpeedOfX;
                NewY1 = PreviousPointA.Y - PointA_SpeedOfY;
            }


            if (PointBDirectionTrend == (int)DirectionTrends.None)
            {
                // This is an error condition; I do not anticipate it occuring, but incase it does I think a graceful way to handle it is to adjust the PointBDirectionTrend to a better value.
                Random RNG = new Random();
                PointBDirectionTrend = RNG.Next(0, (int)DirectionTrends.NorthWest);
            }
            else if(PointBDirectionTrend == (int)DirectionTrends.North)
            {
                NewY2 = PreviousPointB.Y - PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.NorthEast)
            {
                NewX2 = PreviousPointB.X + PointB_SpeedOfX;
                NewY2 = PreviousPointB.Y - PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.East)
            {
                NewX2 = PreviousPointB.X + PointB_SpeedOfX;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.SouthEast)
            {
                NewX2 = PreviousPointB.X + PointB_SpeedOfX;
                NewY2 = PreviousPointB.Y + PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.South)
            {
                NewY2 = PreviousPointB.Y + PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.SouthWest)
            {
                NewX2 = PreviousPointB.X - PointB_SpeedOfX;
                NewY2 = PreviousPointB.Y + PointB_SpeedOfY;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.West)
            {
                NewX2 = PreviousPointB.X - PointB_SpeedOfX;
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.NorthWest)
            {
                NewX2 = PreviousPointB.X - PointB_SpeedOfX;
                NewY2 = PreviousPointB.Y - PointB_SpeedOfY;
            }

            NewX1 = Math.Round(NewX1, 0);
            NewY1 = Math.Round(NewY1, 0);
            NewX2 = Math.Round(NewX2, 0);
            NewY2 = Math.Round(NewY2, 0);

            MyLine.X1 = NewX1;
            MyLine.Y1 = NewY1;

            MyLine.X2 = NewX2;
            MyLine.Y2 = NewY2;

            // Recording the new values as the previous points.
            PreviousPointA.X = NewX1;
            PreviousPointA.Y = NewY1;

            PreviousPointB.X = NewX2;
            PreviousPointB.Y = NewY2;
        }
    }
}
