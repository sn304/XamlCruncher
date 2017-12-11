using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace XamlCruncher
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AppSetting appSetting;
        StorageFile LoadedstorageFile;
        Brush TextBlockBrush;
        Brush TextBoxBrush;
        Brush ErrorBrush;
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
            Application.Current.Suspending += Current_Suspending;
            TextBlockBrush = (Brush)Resources["ApplicationForegroundThemeBrush"];
            TextBoxBrush = (Brush)Resources["TextBoxForegroundThemeBrush"];
            ErrorBrush = new SolidColorBrush(Colors.Red);
        }

        private  async void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            appSetting.Save();
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await storageFolder.CreateFileAsync("XamlCrucher.xmal", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(storageFile, edittext.Text);
            deferral.Complete();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            appSetting = new AppSetting();
            DataContext = appSetting;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile storageFile=await storageFolder.CreateFileAsync("XamlCrucher.xmal", CreationCollisionOption.OpenIfExists);
            edittext.Text = await FileIO.ReadTextAsync(storageFile);
            if(edittext.Text.Length==0)
            {
                await SetDefaultXmal();
            }
            ParseText();
            edittext.Focus(FocusState.Programmatic);
            DisplayLineAndColumn();
       /*     Application.Current.UnhandledException += (Sender, E) =>
            {
                SetErrorStatus(E.Message);
                E.Handled = true;
            };*/
        }

        private void DisplayLineAndColumn()
        {
            int line, col;
            edittext.GetPositionFromIndex(edittext.SelectionStart, out line, out col);
            linecoltext.Text = string.Format("line {0} col {1}",line + 1, col + 1);
            if(edittext.SelectionLength>0)
            {
                edittext.GetPositionFromIndex(edittext.SelectionStart + edittext.SelectionLength - 1, out line, out col);
                linecoltext.Text += string.Format("-line {0} col {1}", line, col);
            }
        }

        private void ParseText()
        {
            object result = null;
            try
            {
                result = XamlReader.Load(edittext.Text);
            }
            catch(Exception exc)
            {
                SetErrorStatus(exc.Message);
                return;
            }
            if (result == null)
                SetErrorStatus("no result");
            else if (!(result is UIElement))
                SetErrorStatus("result is " + result.GetType().Name);
            else
            {
                reasultgrid.Child = (UIElement)result;
                SetOkStatus();
            }

        }
        void SetErrorStatus(string text) {
            SetStatusText(text, ErrorBrush, ErrorBrush);
        }
        void SetOkStatus()
        {
            SetStatusText("OK", TextBlockBrush, TextBoxBrush);

        }
        void SetStatusText(string text,Brush statusbrush,Brush editbursh) {
            statusText.Text = text;
            statusText.Foreground = statusbrush;
            edittext.Foreground = editbursh;

        }
       async private Task SetDefaultXmal()
        {
            edittext.Text= @" <Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns: x = ""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns: local = ""using:XamlCruncher""
    xmlns: d = ""http://schemas.microsoft.com/expression/blend/2008""
    xmlns: mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc: Ignorable = ""d"" >
 

 </ Page >

   ";
            edittext.IsModified = false;
            LoadedstorageFile = null;
            filenametext.Text = "";
        }

        private void edittext_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (appSetting.AutoParsing)
                ParseText();

        }

        private void edittext_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DisplayLineAndColumn();

        }

        private void refreshbutton_Click(object sender, RoutedEventArgs e)
        {
            ParseText();
            BottomAppBar.IsOpen = false;
        }

        private void settingbutton_Click(object sender, RoutedEventArgs e)
        {

            SettingsDialog settingsDialog = new SettingsDialog();
            settingsDialog.DataContext = appSetting;
            Popup popup = new Popup
            {
                Child = settingsDialog,
                IsLightDismissEnabled = true
            };
            settingsDialog.Loaded += (s, e1) => {
                popup.VerticalOffset = this.ActualHeight - settingsDialog.ActualHeight - BottomAppBar.ActualHeight - 24;
                popup.HorizontalOffset = 24;

            };
            popup.IsOpen = true;

        }

     async   private void openbutton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = true;
            await CheckIfOkToTrashFile(LoadFileFromOpenPicker);
            button.IsEnabled = true;
            BottomAppBar.IsOpen = false;

        }

        async private void save_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            if(LoadedstorageFile!=null)
            {
               await SaveXamlToFile(LoadedstorageFile);

            }
            else
            {
                StorageFile storageFile = await GetFileFromSavePicker();
                if (storageFile != null)
                {
                    await SaveXamlToFile(storageFile);
                }
            }
            button.IsEnabled = true;
        }

       async private void save_as_Click(object sender, RoutedEventArgs e)
        {
            StorageFile storage = await GetFileFromSavePicker();
            if (storage != null)
                return;
            await SaveXamlToFile(storage);

        }
       async private void add_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            await CheckIfOkToTrashFile(SetDefaultXmal);
            button.IsEnabled = true;
            BottomAppBar.IsOpen = false;

        }


        async Task<StorageFile> GetFileFromSavePicker()
        {
            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.DefaultFileExtension = ".xaml";
            fileSavePicker.FileTypeChoices.Add("Xaml", new List<string> { ".xaml" });
            fileSavePicker.SuggestedSaveFile = LoadedstorageFile;
            return await fileSavePicker.PickSaveFileAsync();
        }
        async Task SaveXamlToFile(StorageFile storage) {
            LoadedstorageFile = storage;
            string exception = null;
            try
            {
                await FileIO.WriteTextAsync(LoadedstorageFile, edittext.Text);
            }
            catch(Exception ex)
            {
                exception = ex.Message;
            }
            if(exception!=null)
            {
                string message = string.Format("找不到文件 {0}:{1}", LoadedstorageFile.Name, exception);
                MessageDialog messageDialog = new MessageDialog(message, "Xmal Cruncher");
                await messageDialog.ShowAsync();
            }
            else
            {
                edittext.IsModified = false;
                filenametext.Text = storage.Path;

            }

        }
        async Task LoadFileFromOpenPicker()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".xaml");
            StorageFile storageFile = await picker.PickSingleFileAsync();
            if (storageFile == null)
                return;
            string message = null;
            try
            {
                edittext.Text = await FileIO.ReadTextAsync(storageFile);

            }
            catch(Exception ex)
            {
                message = ex.Message;
            }
            if(message!=null)
            {
                string text = string.Format("can't open this file{0}:{1}", storageFile.Name, message);
                MessageDialog messageDialog = new MessageDialog(text, "XamlCruncher");
                await messageDialog.ShowAsync();
                return;
            }
            LoadedstorageFile = storageFile;
            filenametext.Text = storageFile.Name;
            edittext.IsModified = false;

        }
        async Task CheckIfOkToTrashFile(Func<Task> func)
        {
            if(!edittext.IsModified)
            {
               await func();
                return;
            }
            string text = string.Format("do you want to save changes{0}", LoadedstorageFile == null ? "untitle" : LoadedstorageFile.Name);
            MessageDialog messageDialog = new MessageDialog(text, "XamlCruncher");
            messageDialog.Commands.Add(new UICommand("Cancel", null, "Cancel"));
            messageDialog.Commands.Add(new UICommand("Save", null, "Save"));
            messageDialog.Commands.Add(new UICommand("Not Save", null, "Not Save"));
            IUICommand command = await messageDialog.ShowAsync();
            if((string)command.Id=="Cancel")
            {
                return;
            }
            else if((string)command.Id=="Save")
            {
                if(LoadedstorageFile!=null)
                {
                    await SaveXamlToFile(LoadedstorageFile);
                    await func();
                    return;
                }
                else
                {
                    StorageFile storageFile = await GetFileFromSavePicker();
                    if (storageFile == null)
                        return;
                    await SaveXamlToFile(storageFile);
                    await func();
                    return;
                }

            }
            if((string)command.Id=="Not Save")
            {
                await func();
                return;
            }


        }
    }
}
