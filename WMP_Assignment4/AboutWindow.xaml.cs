/*
**  File Name:      AboutWindow.xaml.cs
**	Project Name:	WMP_Assignment4
**	Author:         Matthew G. Schatz
**  Date:           October 17, 2019
**	Description:	This file contains the source code for the AboutWindow used in the WMP_Assignment4 project.
*/

using System.Windows;

namespace WMP_Assignment4
{
     /*
     **	Class Name:     AboutWindow		
     **	Description:    This class is designed to create an About status window. Nothing fancy here, just a window with some status info and a button to close it.
     */
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        /*
        **	Method Name:	btnOK_Click	
        **	Parameters:		object sender; An object containing information about the sender of this event.
        **                  RoutedEventArgs e; An object containing detailed information about the event, other than sender name/ID.
        **	Return Values:	void; No return values.	
        **	Description:	This method is triggered when the user clicks/selects the OK button. This causes the window to close.
        */
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
