﻿/*
**  File Name:      LineGenerator.cs
**	Project Name:	WMP_Assignment4
**	Author:         Matthew G. Schatz
**  Date:           October 17, 2019
**	Description:	This file holds the source code for the LineGenerator class. This class is designed to represnt a object that generates the coordinates for Lines
**                  to be drawn on the screen repeatedly. It has logic to contain the coordinates within a shape that is square (dimensions are irrelevant as long as its a square).
**                  It will generate one set of coordinates for a line per call to CalculateNewLinePoints().
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WMP_Assignment4
{
    class LineGenerator
    {
        // Data members:

        // This struct is used to hold a pair of coordinates for a single point. (X, and Y coords).
        private struct PointInfo
        {
            public double X;
            public double Y;

        };

        // This enum is used to house several possible line speeds. Their values represent how many pixels the line moves per increment.
        private enum LineSpeeds
        {
            Default = 1,
            Low = 2,
            Medium = 5,
            High = 10
        }

        // This enum is used to house the directions that a point can be trending in. 
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

        // These structs of PointInfo type are used to hold the X and Y coords for each point of a line.
        PointInfo PreviousPointA = new PointInfo();
        PointInfo PreviousPointB = new PointInfo();

        // This bool is used to determine if this is the first time this class has been called since construction occurred.
        private bool FirstLine = true;

        private Canvas ActiveCanvas;

        // These values were determined by observing the values of height and width of the canvas element. I used the properties menu of visual studio to do this in conjunction with the XAML designer/viewer.
        // *WARNING* If the window size is changed these values will need to be adjusted.
        // Note: It would be better to link these values to the active canvas so the values could just be read in - however I tried this and it led to some irregularities in edge calculations. Developer beware!
        private double CanvasMaxX = 582;
        private double CanvasMaxY = 359;
        private double CanvasMinX = 10;
        private double CanvasMinY = 10;
        
        // This value is connected with the enum LineSpeeds, as the values contained there represent the valid values for this private int named LineSpeed. 
        // This value represents the speed (or rather, increment value) for the line movement.
        private double PointA_SpeedOfX = (double)LineSpeeds.Default;
        private double PointA_SpeedOfY = (double)LineSpeeds.Default;
        private double PointB_SpeedOfX = (double)LineSpeeds.Default;
        private double PointB_SpeedOfY = (double)LineSpeeds.Default;

        // Each point must have its own direction trend, so they can move independently of each other.
        public int PointADirectionTrend = (int)DirectionTrends.None;
        public int PointBDirectionTrend = (int)DirectionTrends.None;

        // Method(s):
        
        /*
        **	Method Name:	GenerateRandomStartingPoints()
        **	Parameters:		Line MyLine: This entity holds a reference to the Line that is currently being created.
        **	Return Values:	Void.
        **	Description:	This method takes a reference to a Line variable and gives it random positions for its A and B points. The values are recorded afterwards so the line can be tracked accurately.
        */
        private void GenerateRandomStartingPoints(Line MyLine)
        {
            Random RNG = new Random();

            // Need to generate randomized starting positions for this line.
            MyLine.X1 = RNG.Next((int)CanvasMinX, (int)CanvasMaxX);
            MyLine.X2 = RNG.Next((int)CanvasMinX, (int)CanvasMaxX);
            
            MyLine.Y1 = RNG.Next((int)CanvasMinY, (int)CanvasMaxY);
            MyLine.Y2 = RNG.Next((int)CanvasMinY, (int)CanvasMaxY);

            // Rounding the results.
            MyLine.X1 = Math.Round(MyLine.X1, 0);
            MyLine.Y1 = Math.Round(MyLine.Y1, 0);

            MyLine.X2 = Math.Round(MyLine.X2, 0);
            MyLine.Y2 = Math.Round(MyLine.Y2, 0);


            // Recording the values so they can be referenced by future lines.
            PreviousPointA.X = MyLine.X1;
            PreviousPointA.Y = MyLine.Y1;

            PreviousPointB.X = MyLine.X2;            
            PreviousPointB.Y = MyLine.Y2;

        }
        /*
        **	Method Name:	DetermineDirectionTrend
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	bool: This method returns a bool to indicate whether a direction was changed or not. True indicates direction trend was changed, false indicates not.
        **	Description:	This method uses a very long series of if statements to determine if the individual point is near any of the edges of the canvas. 
        **                  The way I organized it there are 8 possible movement directions North, NorthEast, East, SouthEast, South, SouthWest, and NorthWest.'
        **                  This method checks to see if the point is going to be at or beyond a wall and chooses an appropriate direction to change to.
        */
        private bool DetermineDirectionTrend(string WhichPoint)
        {
            // This struct is used to hold the info about the revelent points previous location.
            PointInfo PreviousPoints = new PointInfo();

            // This variable is used to hold the points direction trend, also part of the generalizing of the code so I don't have to write the same logic twice (once for Point A, once for Point B).
            int DirectionTrend = 0;

            // This bool indicates whether the point bounced off a wall.
            bool DirectionTrendChanged = false;

            // This value is used to hold the value of the points position, after the next increment has been added to it.
            double NextPosition = 0;

            // These values are used to hold the points values, to help with generalizing the code.
            double X_MovementIncrement = 0;
            double Y_MovementIncrement = 0;

            // Setting up a Random object to be used for some of the decision making about which direction to head next.
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
                            // Point is beyond Northern edge, but not Eastern or Western edges.

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
                            // Point is on Eastern edge.

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
                // Check if point is at/beyond the right edge.
                NextPosition = PreviousPoints.X + X_MovementIncrement;
                if (NextPosition >= CanvasMaxX)
                {
                    // Point is at or beyond the Eastern edge.

                    // Checking if the Y is on the Northern edge.
                    NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                    if (NextPosition <= CanvasMinY)
                    {
                        // Point is at/beyond North Eastern edge.

                        // Point is on or beyond the North Eastern edge. Redirect to SouthWest. 
                        DirectionTrend = (int)DirectionTrends.SouthWest;
                        DirectionTrendChanged = true;

                    }
                    else
                    {
                        // Checking if point is at/beyond the Southern edge.
                        NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                        if(NextPosition >= CanvasMaxY)
                        {
                            // Point is at/beyond South Eastern edge.


                            // Point is on or beyond the South Eastern edge. Redirect to NorthWest. 
                            DirectionTrend = (int)DirectionTrends.NorthWest;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point is at/beyond South edge, but NOT Eastern or Western edges.

                            // Point is on or beyond the South edge. Redirect to NorthWest, North, or NorthEast. 
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast                                
                            };

                            // Applying a randomly selected direction trend.
                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                       
                    }
                }
                else
                {
                    // Point is NOT beyond East edge.

                    // Check if point is on/beyond the Northern edge.
                    NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                    if(NextPosition <= CanvasMinY)
                    {
                        // Point is on/beyond Northern edge, but NOT the Eastern edge.

                        // Point is on/beyond the Northern edge. Redirect it to the South, SouthEast, or SouthWest.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.South,
                                (int)DirectionTrends.SouthEast,
                                (int)DirectionTrends.SouthWest
                            };

                        // Applying a randomly selected direction trend.
                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Point is NOT beyond East edge and NOT beyond the North.

                        // Check if point is on/beyond the Southern edge.
                        NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                        if(NextPosition >= CanvasMaxY)
                        {
                            // Point is NOT beyond East edge and IS beyond the Southern edge.


                            // Point is beyond South edge. Redirect it to North, NorthEast, or NorthWest.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.NorthWest
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
                // Checks if the next increment will put the point at or beyond the Southern edge of the canvas.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                if (NextPosition >= CanvasMaxY)
                {
                    // Point is at/beyond Southern edge.

                    // Check if X is also going to be over the edge, that would indicate the current position is at/beyond the bottom right.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // Point is at/beyond SouthEastern edge.

                        // This point is either in the bottom right of the screen or beyond it. Redirect to NorthWest.
                        DirectionTrend = (int)DirectionTrends.NorthWest;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Point is NOT at/beyond Eastern edge but IS at/beyond South edge.


                        // Check if point will be over Western edge. 
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // Point is at/beyond SouthWestern edge.


                            // This point is at/beyond the SouthWestern edge of the screen. Redirect to NorthEast.
                            DirectionTrend = (int)DirectionTrends.NorthEast;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point is at/beyond South edge, but not East or West edges.


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
                    // Point is NOT beyond South edge.


                    // Check if point is at/beyond the Eastern edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // Point is at/beyond the East edge but NOT beyond the South edge.


                        // This point is either at the right of the screen or beyond it. Redirect to SouthWest, West, NorthWest,.
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
                        // Check if point is on the western edge.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // Point is at/beyond the West edge, but NOT the South edge.


                            // Point is on/beyond Western edge. Redirect to NorthEast, East, or SouthEast.
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
                // Check if point will be at/beyond the South edge.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                if (NextPosition >= CanvasMaxY)
                {
                    // Point is at/beyond the South edge.


                    // Check if point will be at/beyond the East edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // Point is at/beyond the SouthEastern edge.


                        // Point will be at/beyond South eastern edge of screen, change its direction to NorthWest.
                        DirectionTrend = (int)DirectionTrends.NorthWest;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if X is going to be over the South western edge, that would indicate the current position is the bottom left.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // Point is at/beyond the SouthWestern edge.


                            // Point is in south western corner. Redirect to NorthEast.
                            DirectionTrend = (int)DirectionTrends.NorthEast;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point is beyond the South edge, but NOT East or West edges.


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
                else
                {
                    // Point is NOT at/beyond South edge.

                    // Check if point will be at/beyond East edge.
                    NextPosition = PreviousPoints.X + X_MovementIncrement;
                    if (NextPosition >= CanvasMaxX)
                    {
                        // Point is at/beyond the East edge but NOT the South.


                        // Redirect point to NorthWest, West, or SouthWest.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.West,
                                (int)DirectionTrends.SouthWest
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if point will be at/beyond the West edge.
                        NextPosition = PreviousPoints.X - X_MovementIncrement;
                        if (NextPosition <= CanvasMinX)
                        {
                            // Point is at/beyond the West edge, but NOT the South.


                            // Redirect to NorthEast, East, or SouthEast
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.SouthEast
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;

                        }
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.SouthWest)
            {
                // Checks if the next increment will put the point beyond the Southern edge.
                NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                if (NextPosition >= CanvasMaxY)
                {
                    // Point is at/beyond the South edge.


                    // Check if point is at/beyond the West edge.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // Point is at/beyond the SouthWest edge.


                        // This point is either in the bottom left of the screen or beyond it. Redirect to NorthEast.
                        DirectionTrend = (int)DirectionTrends.NorthEast;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if point is at/beyond the East edge.
                        NextPosition = PreviousPoints.X + X_MovementIncrement;
                        if(NextPosition >= CanvasMaxX)
                        {
                            // Point is at/beyond the SouthEast edge.


                            // Point is going to be at/beyond SouthEastern corner. Redirect to NorthWest.
                            DirectionTrend = (int)DirectionTrends.NorthWest;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point is beyond South edge but NOT East or West edges.


                            // Point will be at South edge of screen, change its direction trend to North, NorthEast, or NorthWest.
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
                    // Point is NOT beyond South edge.


                    // Check if the point is at/beyond the West edge.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // Point is at/beyond the West edge, but NOT South edge.


                        // This point is either at the left of the screen or beyond it. Redirect to NorthEast, East, or SouthEast.
                        int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.East,
                                (int)DirectionTrends.SouthEast
                            };

                        DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if point is at/beyond the East edge.
                        NextPosition = PreviousPoints.X + X_MovementIncrement;
                        if(NextPosition >= CanvasMaxX)
                        {
                            // Point is at/beyond the Eastern edge, but NOT South edge.


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
                // Check if point is at/beyond the West edge. 
                NextPosition = PreviousPoints.X - X_MovementIncrement;
                if (NextPosition <= CanvasMinX)
                {
                    // Point is at/beyond the West edge.


                    // Check if point is at/beyond the North edge.
                    NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                    if(NextPosition <= CanvasMinY)
                    {
                        // Point is at/beyond the NorthWest edge.
                        

                        // Redirect to SouthEast.
                        DirectionTrend = (int)DirectionTrends.SouthEast;
                        DirectionTrendChanged = true;
                    }
                    else
                    {
                        // Check if point is at/beyond the South edge.
                        NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                        if(NextPosition >= CanvasMaxY)
                        {
                            // Point is at/beyond the SouthWest edge.


                            // Redirect to NorthEast.
                            DirectionTrend = (int)DirectionTrends.NorthEast;
                            DirectionTrendChanged = true;
                        }
                        else
                        {
                            // Point is at/beyond West edge but NOT beyond North or South edges.

                            // Redirect to NorthEast, East, or SouthEast.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthEast,
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.SouthEast
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
                else
                {
                    // Point is NOT beyond West edge.
                    
                    // Check if point is at/beyond North edge.
                    NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                    if(NextPosition <= CanvasMinY)
                    {
                        // Point is at/beyond the North edge, but NOT the West.


                        // Redirect to SouthWest, South, or SouthEast.
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
                        // Check if point is at/beyond South edge.
                        NextPosition = PreviousPoints.Y + Y_MovementIncrement;
                        if (NextPosition >= CanvasMaxY)
                        {
                            // Point is at/beyond the South edge, but NOT the West.


                            // Point will be on the South edge. Redirect to NorthWest, North, or NorthEast.
                            int[] DirectionArray = new int[] {
                                (int)DirectionTrends.NorthWest,
                                (int)DirectionTrends.North,
                                (int)DirectionTrends.NorthEast
                            };

                            DirectionTrend = DirectionArray[RNG.Next(0, DirectionArray.Length)];
                            DirectionTrendChanged = true;
                        }
                    }
                }
            }
            else if (DirectionTrend == (int)DirectionTrends.NorthWest)
            {
                // Check if point is at/beyond North edge.
                NextPosition = PreviousPoints.Y - Y_MovementIncrement;
                if (NextPosition <= CanvasMinY)
                {
                    // Point is at / beyond the North edge.

                    // Check if point is at/beyond the West edge.
                    NextPosition = PreviousPoints.X - X_MovementIncrement;
                    if (NextPosition <= CanvasMinX)
                    {
                        // Point is at/beyond NorthWest edge.

                        // Redirect to SouthEast.
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

        /*
        **	Method Name:	CalculateNewLinePoints()
        **	Parameters:		Line MyLine: This entity provides a reference to the Line that were tracking / building.
        **	Return Values:	Void.
        **	Description:	This method gives brand new incoming Lines starting points and directions.
        **                  If FirstLine is NOT true it processes the next move and applies the new values to MyLine. 
        */
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
        /*
        **	Method Name:	RandomizeSpeed()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	Void.
        **	Description:	This method chooses a speed from a list of available options at random and applies it to the SpeedOfX and SpeedOfY variables for each point.
        */
        private void RandomizeSpeed(string WhichPoint)
        {
            Random RNG = new Random();
            int[] Speeds = new int[]
            {
                (int)LineSpeeds.Default,
                (int)LineSpeeds.Low,
                (int)LineSpeeds.Medium,
                (int)LineSpeeds.High
            };

            if(WhichPoint == "A")
            {
                PointA_SpeedOfX = Speeds[RNG.Next(0, Speeds.Length)];
                PointA_SpeedOfY = Speeds[RNG.Next(0, Speeds.Length)];
            }
            else if(WhichPoint == "B")
            {
                PointB_SpeedOfX = Speeds[RNG.Next(0, Speeds.Length)];
                PointB_SpeedOfY = Speeds[RNG.Next(0, Speeds.Length)];
            }
        }

        /*
        **	Method Name:	MovePointNorth()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	PointInfo: This method returns a filled out PointInfo struct containing the new X and Y coords.
        **	Description:	This method is used to generate a pair of new X and Y coordinates for a point, depending on the value fed in as WhichPoint.
        */
        private PointInfo MovePointNorth(String WhichPoint)
        {
            PointInfo Result = new PointInfo();

            if (WhichPoint == "A")
            {
                Result.Y = PreviousPointA.Y - PointA_SpeedOfY;
                Result.X = PreviousPointA.X;
            }
            else if(WhichPoint == "B")
            {
                Result.Y = PreviousPointB.Y - PointB_SpeedOfY;
                Result.X = PreviousPointB.X;
            }

            return Result;
        }

        /*
        **	Method Name:	MovePointNorthEast()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	PointInfo: This method returns a filled out PointInfo struct containing the new X and Y coords.
        **	Description:	This method is used to generate a pair of new X and Y coordinates for a point, depending on the value fed in as WhichPoint.
        */
        private PointInfo MovePointNorthEast(String WhichPoint)
        {

            PointInfo Result = new PointInfo();

            if (WhichPoint == "A")
            {
                Result.Y = PreviousPointA.Y - PointA_SpeedOfY;
                Result.X = PreviousPointA.X + PointA_SpeedOfX;
            }
            else if (WhichPoint == "B")
            {
                Result.Y = PreviousPointB.Y - PointB_SpeedOfY;
                Result.X = PreviousPointB.X + PointB_SpeedOfX;
            }

            return Result;
        }

        /*
        **	Method Name:	MovePointEast()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	PointInfo: This method returns a filled out PointInfo struct containing the new X and Y coords.
        **	Description:	This method is used to generate a pair of new X and Y coordinates for a point, depending on the value fed in as WhichPoint.
        */
        private PointInfo MovePointEast(String WhichPoint)
        {
            PointInfo Result = new PointInfo();

            if (WhichPoint == "A")
            {
                Result.Y = PreviousPointA.Y;
                Result.X = PreviousPointA.X + PointA_SpeedOfX;
            }
            else if (WhichPoint == "B")
            {
                Result.Y = PreviousPointB.Y;
                Result.X = PreviousPointB.X + PointB_SpeedOfX;
            }

            return Result;
        }

        /*
        **	Method Name:	MovePointSouthEast()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	PointInfo: This method returns a filled out PointInfo struct containing the new X and Y coords.
        **	Description:	This method is used to generate a pair of new X and Y coordinates for a point, depending on the value fed in as WhichPoint.
        */
        private PointInfo MovePointSouthEast(String WhichPoint)
        {
            PointInfo Result = new PointInfo();

            if (WhichPoint == "A")
            {
                Result.Y = PreviousPointA.Y + PointA_SpeedOfY;
                Result.X = PreviousPointA.X + PointA_SpeedOfX;
            }
            else if (WhichPoint == "B")
            {
                Result.Y = PreviousPointB.Y + PointB_SpeedOfY;
                Result.X = PreviousPointB.X + PointB_SpeedOfX;
            }

            return Result;
        }

        /*
        **	Method Name:	MovePointSouth()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	PointInfo: This method returns a filled out PointInfo struct containing the new X and Y coords.
        **	Description:	This method is used to generate a pair of new X and Y coordinates for a point, depending on the value fed in as WhichPoint.
        */
        private PointInfo MovePointSouth(String WhichPoint)
        {
            PointInfo Result = new PointInfo();

            if (WhichPoint == "A")
            {
                Result.Y = PreviousPointA.Y + PointA_SpeedOfY;
                Result.X = PreviousPointA.X;
            }
            else if (WhichPoint == "B")
            {
                Result.Y = PreviousPointB.Y + PointB_SpeedOfY;
                Result.X = PreviousPointB.X;
            }

            return Result;
        }

        /*
        **	Method Name:	MovePointSouthWest()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	PointInfo: This method returns a filled out PointInfo struct containing the new X and Y coords.
        **	Description:	This method is used to generate a pair of new X and Y coordinates for a point, depending on the value fed in as WhichPoint.
        */
        private PointInfo MovePointSouthWest(String WhichPoint)
        {
            PointInfo Result = new PointInfo();

            if (WhichPoint == "A")
            {
                Result.Y = PreviousPointA.Y + PointA_SpeedOfY;
                Result.X = PreviousPointA.X - PointA_SpeedOfX;
            }
            else if (WhichPoint == "B")
            {
                Result.Y = PreviousPointB.Y + PointB_SpeedOfY;
                Result.X = PreviousPointB.X - PointB_SpeedOfX;
            }

            return Result;
        }

        /*
        **	Method Name:	MovePointWest()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	PointInfo: This method returns a filled out PointInfo struct containing the new X and Y coords.
        **	Description:	This method is used to generate a pair of new X and Y coordinates for a point, depending on the value fed in as WhichPoint.
        */
        private PointInfo MovePointWest(String WhichPoint)
        {
            PointInfo Result = new PointInfo();

            if (WhichPoint == "A")
            {
                Result.Y = PreviousPointA.Y;
                Result.X = PreviousPointA.X - PointA_SpeedOfX;
            }
            else if (WhichPoint == "B")
            {
                Result.Y = PreviousPointB.Y;
                Result.X = PreviousPointB.X - PointB_SpeedOfX;
            }

            return Result;
        }

        /*
        **	Method Name:	MovePointNorthWest()
        **	Parameters:		string WhichPoint: This string is used to determine which Point of the line is being worked on (either A, or B).
        **	Return Values:	PointInfo: This method returns a filled out PointInfo struct containing the new X and Y coords.
        **	Description:	This method is used to generate a pair of new X and Y coordinates for a point, depending on the value fed in as WhichPoint.
        */
        private PointInfo MovePointNorthWest(String WhichPoint)
        {
            PointInfo Result = new PointInfo();

            if (WhichPoint == "A")
            {
                Result.Y = PreviousPointA.Y - PointA_SpeedOfY;
                Result.X = PreviousPointA.X - PointA_SpeedOfX;
            }
            else if (WhichPoint == "B")
            {
                Result.Y = PreviousPointB.Y - PointB_SpeedOfY;
                Result.X = PreviousPointB.X - PointB_SpeedOfX;
            }

            return Result;
        }

        /*
        **	Method Name:	ProcessMovement()
        **	Parameters:		Line MyLine: This entity holds a reference to the Line that is being worked on.
        **	Return Values:	Void.
        **	Description:	This method is used to process the new coordinates for a line. The new coordinates are generated depending on what the current direction trend is (each point has unique trend).
        **                  The resulting calculations are rounded to get rid of extra digits past the decmial point (they would interfere with some aspects of the drawing of the line).
        **                  The previous coordinates are recorded into the PreviousPoints structs and finally the new values are written to MyLine.
        */
        private void ProcessMovement(Line MyLine)
        {
            PointInfo NewPointA = new PointInfo();
            PointInfo NewPointB = new PointInfo();

            // PointA Section:

            if (PointADirectionTrend == (int)DirectionTrends.None)
            {
                Random RNG = new Random();
                PointADirectionTrend = RNG.Next((int)DirectionTrends.NorthWest);
            }
            
            if (PointADirectionTrend == (int)DirectionTrends.North)
            {
                NewPointA = MovePointNorth("A");
            }
            else if (PointADirectionTrend == (int)DirectionTrends.NorthEast)
            {
                NewPointA = MovePointNorthEast("A");
            }
            else if (PointADirectionTrend == (int)DirectionTrends.East)
            {
                NewPointA = MovePointEast("A");
            }
            else if (PointADirectionTrend == (int)DirectionTrends.SouthEast)
            {
                NewPointA = MovePointSouthEast("A");
            }
            else if (PointADirectionTrend == (int)DirectionTrends.South)
            {
                NewPointA = MovePointSouth("A");
            }
            else if (PointADirectionTrend == (int)DirectionTrends.SouthWest)
            {
                NewPointA = MovePointSouthWest("A");
            }
            else if (PointADirectionTrend == (int)DirectionTrends.West)
            {
                NewPointA = MovePointWest("A");
            }
            else if (PointADirectionTrend == (int)DirectionTrends.NorthWest)
            {
                NewPointA = MovePointNorthWest("A");
            }

            // PointB Section:

            if (PointBDirectionTrend == (int)DirectionTrends.None)
            {
                // This is an error condition; I do not anticipate it occuring, but incase it does I think a graceful way to handle it is to adjust the PointBDirectionTrend to a better value.
                Random RNG = new Random();
                PointBDirectionTrend = RNG.Next((int)DirectionTrends.NorthWest);
            }
            
            if(PointBDirectionTrend == (int)DirectionTrends.North)
            {
                NewPointB = MovePointNorth("B");
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.NorthEast)
            {
                NewPointB = MovePointNorthEast("B");
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.East)
            {
                NewPointB = MovePointEast("B");
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.SouthEast)
            {
                NewPointB = MovePointSouthEast("B");
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.South)
            {
                NewPointB = MovePointSouth("B");
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.SouthWest)
            {
                NewPointB = MovePointSouthWest("B");
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.West)
            {
                NewPointB = MovePointWest("B");
            }
            else if (PointBDirectionTrend == (int)DirectionTrends.NorthWest)
            {
                NewPointB = MovePointNorthWest("B");
            }


            // Rounding numbers to eliminate excessively long values beyond the decimal point.
            NewPointA.X = Math.Round(NewPointA.X, 0);
            NewPointA.Y = Math.Round(NewPointA.Y, 0);
            NewPointB.X = Math.Round(NewPointB.X, 0);
            NewPointB.Y = Math.Round(NewPointB.Y, 0);

            // Assigning new values as the values for MyLine.
            MyLine.X1 = NewPointA.X;
            MyLine.Y1 = NewPointA.Y;

            MyLine.X2 = NewPointB.X;
            MyLine.Y2 = NewPointB.Y;

            // Recording the new values as the previous points.
            PreviousPointA.X = NewPointA.X;
            PreviousPointA.Y = NewPointA.Y;

            PreviousPointB.X = NewPointB.X;
            PreviousPointB.Y = NewPointB.Y;
        }
    }
}
