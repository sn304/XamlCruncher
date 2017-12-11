using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace XamlCruncher
{
    public sealed partial class SettingsDialog : UserControl
    {
        public SettingsDialog()
        {
            this.InitializeComponent();
            Loaded += SettingsDialog_Loaded;
        }

        private void SettingsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            
            AppSetting setting = DataContext as AppSetting;
            foreach(EditOrientationRadioButton x in orientationgrid.Children)
            {
                x.IsChecked = (setting.EditOrientation == x.EditOrientation);
            }
 
        }

        private void EditOrientationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSetting setting = DataContext as AppSetting;
            EditOrientationRadioButton editOrientationRadioButton = sender as EditOrientationRadioButton;
            setting.EditOrientation = editOrientationRadioButton.EditOrientation;

        }





    
    }
}
