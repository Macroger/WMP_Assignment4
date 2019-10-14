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
using System.Windows.Shapes;

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
