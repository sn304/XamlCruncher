using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XamlCruncher
{
    enum EditOrientation
    {
        Left,Top,Right,Button
    }
    class AppSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        EditOrientation editOrientation = EditOrientation.Left;
        Orientation orientation = Orientation.Horizontal;
        bool swapandDisplay = false;
        bool showLines = false;
        bool showRuler = false;
        bool autoparsing = false;
        double fontsize = 18;
        int tabSpace = 4;
        public AppSetting() {
            ApplicationDataContainer appdata= ApplicationData.Current.LocalSettings;
            var x = appdata.Values;
            if (x.ContainsKey("EditOrientation"))
                EditOrientation = (EditOrientation)x["EditOrientation"];
            if (x.ContainsKey("ShowLines"))
                ShowLines = (bool)x["ShowLines"];
            if (x.ContainsKey("ShowRuler"))
                showRuler = (bool)x["ShowRuler"];
            if (x.ContainsKey("AutoParsing"))
                AutoParsing = (bool)x["AutoParing"];
            if (x.ContainsKey("FontSize"))
                FontSize = (double)x["FontSize"];
            if (x.ContainsKey("TabSpace"))
                TabSpace = (int)x["TabSpace"];
        }

        public void Save()
        {
            ApplicationDataContainer appdata = ApplicationData.Current.LocalSettings;
            var x = appdata.Values;
            x["EditOrientation"] = EditOrientation;
            x["ShowLines"] = ShowLines;
            x["ShowRuler"] = ShowRuler;
            x["AutoParing"] = AutoParsing;
            x["FontSize"] = FontSize;
            x["TabSpace"] = TabSpace;
    }
        public  EditOrientation EditOrientation { set {

                if(SetProperty<EditOrientation>(ref editOrientation, value))
                    {
                    switch(editOrientation)
                    {
                        case EditOrientation.Left:
                            this.SwapAndDisplay = false;
                            this.Orientation = Orientation.Horizontal;
                            break;
                        case EditOrientation.Right:
                            this.SwapAndDisplay = true;
                            this.Orientation = Orientation.Horizontal;
                            break;
                        case EditOrientation.Top:
                            this.SwapAndDisplay = false;
                            this.Orientation = Orientation.Vertical;
                            break;
                        case EditOrientation.Button:
                            this.SwapAndDisplay = true;
                            this.Orientation = Orientation.Vertical;
                            break;
                    }
                    

                }
            }
            get { return editOrientation; }
        }
      public  Orientation Orientation {protected set { SetProperty<Orientation>(ref orientation, value); }get { return orientation; } }
        public bool SwapAndDisplay { protected set { SetProperty<bool>(ref swapandDisplay, value); } get { return swapandDisplay; } }
        public bool ShowLines { protected set { SetProperty<bool>(ref showLines, value); } get { return showLines; } }
        public bool ShowRuler { protected set { SetProperty<bool>(ref showRuler, value); } get { return showRuler; } }
        public bool AutoParsing { protected set { SetProperty<bool>(ref autoparsing, value); } get { return autoparsing; } }
        public double FontSize { protected set { SetProperty<double>(ref fontsize, value); } get { return fontsize; } }
        public int TabSpace { protected set { SetProperty<int>(ref tabSpace, value); } get { return tabSpace; } }

        bool SetProperty<T>(ref T storage,T value,[CallerMemberName]string propertyname=null)
        {
            if (storage.Equals(value))
                return false;
            storage = value;
            OnpropertyChanged(propertyname);
            return true;
        }

        private void OnpropertyChanged(string propertyname)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
