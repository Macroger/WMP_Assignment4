/*
**  File Name:      MainWindow.xaml.cs
**	Project Name:	WMP_Assignment4
**	Author:         Matthew G. Schatz
**  Date:           October 17, 2019
**	Description:	This file holds the code for the User Interface portion of the WMP_Assignment4 project. This assignment has the student working with threads to draw lines on the screen.
**                  The main objective is to learn how to work with threads and gain experience in their use. This project contains several classes which contribute to the opearation of the application
**                  including LineGenerator and LineGeneratorManager classes. 
*/

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WMP_Assignment4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Datamember(s):

        // Creating a reference to a LineGeneratorManager object.
        LineGeneratorManager myLineGeneratorManager;


        /*
        **	Method Name:	MainWindow()
        **	Parameters:		None.
        **	Return Values:	None.
        **	Description:	This is the main entry point for this application. I use it similar to a constructor for a class.
        */
        public MainWindow()

        {
            InitializeComponent();

            // Instantiating the reference to LineGeneratorManager.
            myLineGeneratorManager = new LineGeneratorManager(LineCanvas);
        }

        /*
        **	Method Name:	AboutCommand_CanExecute	
        **	Parameters:		object sender; An object containing information about the sender of this event.
        **                  CanExecuteRoutedEventArgs e; An object containing detailed information about the event, other than sender name/ID.
        **	Return Values:	void; No return values.	
        **	Description:	This method is used to determine whether About command is in a position to activate. This method determines the enabled/disabled state of the About command.	
        */
        private void AboutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = true;
        }

        /*
        **	Method Name:	AboutCommand_Executed	
        **	Parameters:		object sender; An object containing information about the sender of this event.
        **                  CanExecuteRoutedEventArgs e; An object containing detailed information about the event.
        **	Return Values:	void; No return values.	
        **	Description:	This method is used to instantiate an AboutWindow object, and display it for the user to read. It is designed to be a modal window, forcing the user to dismiss it in order to resume normal operation. 
        */
        private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutWindow window1 = new AboutWindow();
            window1.Owner = this;
            window1.ShowDialog();
        }

        /*
        **	Method Name:	CloseCommand_CanExecute	
        **	Parameters:		object sender; An object containing information about the sender of this event.
        **                  CanExecuteRoutedEventArgs e; An object containing detailed information about the event, other than sender name/ID.
        **	Return Values:	void; No return values.	
        **	Description:	This method is used to determine whether Close command is in a position to activate. This method determines the enabled/disabled state of the Close command.	
        */
        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /*
        **	Method Name:	CloseCommand_Executed	
        **	Parameters:		object sender; An object containing information about the sender of this event.
        **                  CanExecuteRoutedEventArgs e; An object containing detailed information about the event, other than sender name/ID.
        **	Return Values:	void; No return values.	
        **	Description:	This method is used to shut down the application. A call to the LineGeneratorManager object is made to ShutdownAllTasks to gracefully end the worker tasks.
        */
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Make call to LineGeneratorManager to shut down all worker tasks.
            myLineGeneratorManager.ShutdownAllTasks();

            // Finally, shut down the application.
            System.Windows.Application.Current.Shutdown();
        }

        /*
        **	Method Name:	Canvas_SizeChanged()
        **	Parameters:		object sender: The originator of this event (in this case, should be a canvas element).
        **		            SizeChangedEventArgs e: This variable holds values related to the changing of the window size.
        **  Return Values:  Void.
        **	Description:	This method is one I got from an online source: https://codedocu.com/Details?d=2043&a=9&f=331&l=0. The author of the code posted a youtube explaining it, as well
        **                  as a link to acquire the code(https://www.youtube.com/watch?v=Wum-btDCygo). I want to acknowledge his awesome code and I would like to use it in my own, so Thank You codeDocu_com! 
        **                  This code is designed to resize child objects of a canvas to be resized as the user resizes the whole window. This is required because
        **                  normally a canvas element would use absolute positioning and child elements would NOT resize when the window resizes. This method provides behaviour closer to a grid panel.
        **  Code Retrieved: October 14, 2019.
        */
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //----------------< Canvas_SizeChanged() >----------------

            Canvas canvas = sender as Canvas;

            SizeChangedEventArgs canvas_Changed_Args = e;

            //< check >

            //*if size=0 then initial

            if (canvas_Changed_Args.PreviousSize.Width == 0) return;

            //</ check >



            //< init >

            double old_Height = canvas_Changed_Args.PreviousSize.Height;

            double new_Height = canvas_Changed_Args.NewSize.Height;

            double old_Width = canvas_Changed_Args.PreviousSize.Width;

            double new_Width = canvas_Changed_Args.NewSize.Width;



            double scale_Width = new_Width / old_Width;

            double scale_Height = new_Height / old_Height;

            //</ init >


            //----< adapt all children >----

            foreach (FrameworkElement element in canvas.Children)
            {

                //< get >

                double old_Left = Canvas.GetLeft(element);

                double old_Top = Canvas.GetTop(element);

                //</ get >



                // < set Left-Top>

                Canvas.SetLeft(element, old_Left * scale_Width);

                Canvas.SetTop(element, old_Top * scale_Height);

                // </ set Left-Top >



                //< set Width-Heigth >

                element.Width = element.Width * scale_Width;

                element.Height = element.Height * scale_Height;

                //</ set Width-Heigth >

            }

            //----</ adapt all children >----


            //----------------</ Canvas_SizeChanged() >----------------

        }

        /*
        **	Method Name:	StartButton_Click()
        **	Parameters:		object sender: The originator of this event.
        **		            SizeChangedEventArgs e: This variable holds values related to the event.
        **	Return Values:	Void.
        **	Description:	This method is an event handler for when the user clicks the start button. 
        */
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if(myLineGeneratorManager.Stop == true)
            {
                myLineGeneratorManager.Stop = false;
            }
            // Make this thread sleep for a few milliseconds so that the call to StartTaskSpawner doesn't occur immediately after toggling myLineGeneratorManager.Stop.
            Thread.Sleep(10);

            // Start the TaskSpawner that will continuously generate lines.
            myLineGeneratorManager.StartTaskSpawner();

            // This statement updates the UI with the number of Task Spawners that are currently active.
            tbThreadsActive.Text = myLineGeneratorManager.HowManyThreadSpawnersActive().ToString();
        }

        /*
        **	Method Name:	StopButton_Click()
        **	Parameters:		object sender: The originator of this event.
        **		            SizeChangedEventArgs e: This variable holds values related to the event.
        **	Return Values:	Void.
        **	Description:	This method is an event handler for when the user clicks the stop button. 
        */
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if(myLineGeneratorManager.Stop == false)
            {
                myLineGeneratorManager.ShutdownAllTasks();

                LineCanvas.Children.Clear();

                tbThreadsActive.Text = 0.ToString();
            }
        }

        /*
        **	Method Name:	PauseButton_Click()
        **	Parameters:		object sender: The originator of this event.
        **		            SizeChangedEventArgs e: This variable holds values related to the event.
        **	Return Values:	Void.
        **	Description:	This method is an event handler for when the user clicks the pause button. 
        */
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if(myLineGeneratorManager.Pause == false)
            {
                myLineGeneratorManager.Pause = true;
                tbErrorMessages.Text = "PAUSED";
            }
        }

        /*
        **	Method Name:	ResumeButton_Click()
        **	Parameters:		object sender: The originator of this event.
        **		            SizeChangedEventArgs e: This variable holds values related to the event.
        **	Return Values:	Void.
        **	Description:	This method is an event handler for when the user clicks the resume button. 
        */
        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (myLineGeneratorManager.Pause == true)
            {
                myLineGeneratorManager.Pause = false;
                tbErrorMessages.Text = "";
            }
        }

        /*
        **	Method Name:	TailLengthSlider_ValueChanged()
        **	Parameters:		object sender: The originator of this event.
        **		            SizeChangedEventArgs e: This variable holds values related to the event.
        **	Return Values:	Void.
        **	Description:	This method is an event handler for when the user clicks adjusts the TailLength slider.
        */
        private void TailLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (TailLengthSlider.IsLoaded)
            {
                myLineGeneratorManager.TailLength = (int)TailLengthSlider.Value;
            }
        }

        /*
        **	Method Name:	SpeedSlider_ValueChanged()
        **	Parameters:		object sender: The originator of this event.
        **		            SizeChangedEventArgs e: This variable holds values related to the event.
        **	Return Values:	Void.
        **	Description:	This method is an event handler for when the user clicks adjusts the speed slider.
        */
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            // This check makes sure that the SpeedSlider has finished loading before attempting to acces its values.
            if(SpeedSlider.IsLoaded)
            {
                if ((int)SpeedSlider.Value == 0)
                {
                    SpeedSliderValueLabel.Content = "Lowest";
                    myLineGeneratorManager.SpawnTimeInMilliSeconds = 150;
                }
                else if ((int)SpeedSlider.Value == 1)
                {
                    SpeedSliderValueLabel.Content = "Lower";
                    myLineGeneratorManager.SpawnTimeInMilliSeconds = 125;
                }
                else if ((int)SpeedSlider.Value == 2)
                {
                    SpeedSliderValueLabel.Content = "Low";
                    myLineGeneratorManager.SpawnTimeInMilliSeconds = 100;
                }
                else if ((int)SpeedSlider.Value == 3)
                {
                    SpeedSliderValueLabel.Content = "Default";
                    myLineGeneratorManager.SpawnTimeInMilliSeconds = 75;
                }
                else if ((int)SpeedSlider.Value == 4)
                {
                    SpeedSliderValueLabel.Content = "High";
                    myLineGeneratorManager.SpawnTimeInMilliSeconds = 50;
                }
                else if ((int)SpeedSlider.Value == 5)
                {
                    SpeedSliderValueLabel.Content = "Higher";
                    myLineGeneratorManager.SpawnTimeInMilliSeconds = 25;
                }
                else if ((int)SpeedSlider.Value == 6)
                {
                    SpeedSliderValueLabel.Content = "Highest";
                    myLineGeneratorManager.SpawnTimeInMilliSeconds = 10;
                }
            }
        }
    }
}
