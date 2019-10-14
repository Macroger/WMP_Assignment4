/*
**  File Name:      CustomCommands.cs
**	Project Name:	WMP_Assignment4
**	Author:         Matthew G. Schatz
**  Date:           October 14, 2019
**	Description:	This class provides custom commands to be used by the UI layer. 
*/
using System.Windows.Input;

namespace WMP_Assignment4
{
    /*
     **	Class Name:     CustomCommands		
     **	Description:    This class contains methods that can be used by the UI layer to enable custom commands for buttons, menu options, or events.
     */

    public static class CustomCommands
    {
        /*
        **	Command Name:	    About	
        **	Description:	    This custom event method is used when the user triggers the About menu item. This event is used to capture the User's desire to view the About info panel.
        */
        public static RoutedUICommand About = new RoutedUICommand(
            "About",
            "About",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.A, ModifierKeys.Control)
            }
        );
    }
}
