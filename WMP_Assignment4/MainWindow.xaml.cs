using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WMP_Assignment4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        LineGeneratorManager myLineGeneratorManager;
        
        public MainWindow()

        {
            InitializeComponent();
            myLineGeneratorManager = new LineGeneratorManager(MainCanvas);
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
        **                  CanExecuteRoutedEventArgs e; An object containing detailed information about the event, other than sender name/ID.
        **	Return Values:	void; No return values.	
        **	Description:	This method is used to instantiate a AboutWindow object, and display it for the user to read. It is designed to be a modal window, forcing the user to dismiss it in order to resume normal operation. 
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
        **	Description:	This method is used to shut down the application. If the user has entered text it will ask them if they wish to save first.
        */
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            myLineGeneratorManager.CloseAllThreads();

            System.Windows.Application.Current.Shutdown();
        }


        /*
        **	Method Name:	Canvas_SizeChanged()
        **	Parameters:		object sender: The originator of this event (in this case, should be a canvas element).
        **	Return Values:	SizeChangedEventArgs e: This variable holds values related to the changing of the window size.
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


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if(myLineGeneratorManager.Stop == true)
            {
                myLineGeneratorManager.Stop = false;
            }

            myLineGeneratorManager.StartThreadSpawner();
            
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if(myLineGeneratorManager.Stop == false)
            {
                myLineGeneratorManager.CloseAllThreads();

                List<Line> LineList = new List<Line>();

                foreach(UIElement element in MainCanvas.Children)
                {
                    MessageBox.Show(element.GetType().ToString());      // LAST ENTRY MADE 12:53 AM Oct 17, 2019.
                    //LineList.Add(line);
                }
                foreach(Line line in LineList)
                {
                    MainCanvas.Children.Remove(line);
                }

            }
        }
        

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if(myLineGeneratorManager.Pause == false)
            {
                myLineGeneratorManager.Pause = true;
            }
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (myLineGeneratorManager.Pause == true)
            {
                myLineGeneratorManager.Pause = false;
            }
        }

        private void TailLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            myLineGeneratorManager.TailLength = (int) TailLengthSlider.Value;
        }
    }
}
