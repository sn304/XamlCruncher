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
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace XamlCruncher
{
    public sealed partial class RulerContainer : UserControl
    {
        const double RULER_WIDTH = 12;
        public RulerContainer()
        {
            this.InitializeComponent();
        }
        static RulerContainer() {

            ChildPorperty = DependencyProperty.Register("Child", typeof(UIElement), typeof(RulerContainer), new PropertyMetadata(null, OnChildChanged));
            ShowLinesPorperty=DependencyProperty.Register("ShowLines",typeof(bool),typeof(RulerContainer),new PropertyMetadata(false,OnShowLinesChanged));
            ShowRulerPorperty = DependencyProperty.Register("ShowRuler", typeof(bool), typeof(RulerContainer), new PropertyMetadata(false, OnShowRulerChanged));

        }

        private static void OnShowRulerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnShowLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static DependencyProperty ChildPorperty { get; private set; }
        public static DependencyProperty ShowRulerPorperty { get; private set; }
        public static DependencyProperty ShowLinesPorperty { get; private set; }
        public UIElement Child { get { return (UIElement)GetValue(ChildPorperty); } set { SetValue(ChildPorperty, value); } }
        public bool ShowLines { get { return (bool)GetValue(ShowLinesPorperty); } set { SetValue(ShowLinesPorperty, value); } }
        public bool ShowRuler { get { return (bool)GetValue(ShowRulerPorperty); } set { SetValue(ShowRulerPorperty, value); } }
        void RedrawLines() {
            gridlinesGrid.Children.Clear();
            if (!ShowLines)
                return;
            for(double x=24;x<this.ActualWidth;x+=24)
            {

                Line line = new Line
                { X1 = x,
                  X2 = x,
                  Y1 = 0,
                  Y2=ActualHeight,
                 Stroke=Foreground,
                 StrokeThickness=x%96==0?1:0.5
                };
                gridlinesGrid.Children.Add(line);
            }
            for (double y = 24; y < this.ActualHeight; y += 24)
            {

                Line line = new Line
                {
                    X1 = 0,
                    X2 = ActualWidth,
                    Y1 = y,
                    Y2 = y,
                    Stroke = Foreground,
                    StrokeThickness = y % 96 == 0 ? 1 : 0.5
                };
                gridlinesGrid.Children.Add(line);
            }

        }
        void RedrawRuler() {
            rulercanvas.Children.Clear();
            if(!ShowRuler)
            {
                innergrid.Margin = new Thickness();//取消之前设置的边距
                return;
            }
            innergrid.Margin = new Thickness(RULER_WIDTH,RULER_WIDTH,0,0);
            for (double x=0;x<ActualWidth-RULER_WIDTH;x+=12)
            {

                if(x>0&&x%96==0)
                {
                    TextBlock text = new TextBlock
                    {
                        Text = (x / 96).ToString("F0"),
                        FontSize = RULER_WIDTH - 2
                    };
                    text.Measure(new Size());//逼迫计算text的实际长宽
                    Canvas.SetLeft(text,RULER_WIDTH+ x - text.ActualWidth / 2);
                    Canvas.SetTop(text, 0);
                    rulercanvas.Children.Add(text);
                }
                else
                {
                    Line line = new Line
                    {
                        X1 = x + RULER_WIDTH,
                        X2 = x + RULER_WIDTH,
                        Y1 = x % 48 == 0 ? 2 : 4,
                        Y2 = x % 48 == 0 ? RULER_WIDTH - 2 : RULER_WIDTH - 4,
                       Stroke=Foreground,
                       StrokeThickness=1

                    };

                    rulercanvas.Children.Add(line);
                }      
            }
            Line topline = new Line
            {
                X1 = RULER_WIDTH - 1,
                X2 = ActualWidth,
                Y1 = RULER_WIDTH - 1,
                Y2 = RULER_WIDTH-1,
                Stroke=Foreground,
                StrokeThickness=2            
            };
            rulercanvas.Children.Add(topline);



        }

    }
}
