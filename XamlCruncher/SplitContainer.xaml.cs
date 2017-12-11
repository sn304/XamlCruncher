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
    public sealed partial class SplitContainer : UserControl
    {

          static SplitContainer()
        {
            Child1Property = DependencyProperty.Register("Child1", typeof(UIElement), typeof(SplitContainer), new PropertyMetadata(null, OnChildChanged));
            Child2Property = DependencyProperty.Register("Child2", typeof(UIElement), typeof(SplitContainer), new PropertyMetadata(null, OnChildChanged));
            OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitContainer), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));
            SwapChildProperty= DependencyProperty.Register("SwapChild", typeof(bool), typeof(SplitContainer), new PropertyMetadata(false, OnSwapChildChanged));
            MinimusSizeProperty= DependencyProperty.Register("MinimusSize", typeof(double), typeof(SplitContainer), new PropertyMetadata(100, OnMinumusSizeChanged));
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitContainer).OnOrientation((Orientation)e.NewValue,(Orientation)e.OldValue);
        }

        private void OnOrientation(Orientation newValue, Orientation oldValue)
        {
            if (newValue == oldValue)
                return;//WinRT's bug
            if(newValue==Orientation.Horizontal)
            {
                col1.Width = row1.Height;
                col2.Width = row2.Height;
                col1.MinWidth = Minimus;
                col2.MinWidth = Minimus;
                row1.MinHeight = 0;
                row2.MinHeight = 0;
                row1.Height = new GridLength(1, GridUnitType.Star);
                row2.Height = new GridLength(0);
                thumb.Width = 12;
                thumb.Height = double.NaN;
                Grid.SetColumn(thumb, 1);
                Grid.SetRow(thumb, 0);
                Grid.SetRow(grid1, 0);
                Grid.SetColumn(grid1, 0);
                Grid.SetRow(grid2, 0);
                Grid.SetColumn(grid2, 2);

            }
            else
            {
                row1.Height = col1.Width;
                row2.Height = col2.Width;
                row1.MinHeight = Minimus;
                row2.MinHeight = Minimus;
                col1.MinWidth = 0;
                col2.MinWidth = 0;
                col1.Width = new GridLength(1, GridUnitType.Star);
                col2.Width = new GridLength(0);
                thumb.Height = 12;
                thumb.Width = double.NaN;
                Grid.SetRow(thumb, 1);
                Grid.SetColumn(thumb, 0);
                Grid.SetColumn(grid1, 0);
                Grid.SetRow(grid1, 0);
                Grid.SetColumn(grid2, 0);
                Grid.SetRow(grid2, 2);
            }



        }

        private static void OnMinumusSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitContainer).OnMinimusSizeChanged(e);
        }

        private void OnMinimusSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if(this.Orientation==Orientation.Horizontal)
            {
                col1.MinWidth = (double)e.NewValue;
                col2.MinWidth = (double)e.NewValue;
            }
            else
            {
                row1.MinHeight = (double)e.NewValue;
                row2.MinHeight = (double)e.NewValue;
            }

        }

        private static void OnSwapChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitContainer).OnSwapChildChanged(e);
          
        }

        private void OnSwapChildChanged(DependencyPropertyChangedEventArgs e)
        {
            grid1.Children.Clear();
            grid2.Children.Clear();
            grid1.Children.Add((bool)e.NewValue ? Child2 : Child1);
            grid2.Children.Add((bool)e.NewValue ? Child1 : Child2);

        }

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitContainer).OnChildChanged(e);

        }

        private void OnChildChanged(DependencyPropertyChangedEventArgs e)
        {
            Grid target = (e.Property == Child1Property ^ SwapChild) ? grid1 : grid2;
            target.Children.Clear();
            if(e.NewValue!=null)
            {
                target.Children.Add((UIElement)e.NewValue);
            }
        }

        public static DependencyProperty Child1Property { get; private set; }
        public static DependencyProperty Child2Property { get; private set; }
        public static DependencyProperty OrientationProperty { get; private set; }
        public static DependencyProperty SwapChildProperty { get; private set; }
        public static DependencyProperty MinimusSizeProperty { get; private set; }

        public UIElement Child1 { get { return (UIElement)GetValue(Child1Property); } set { SetValue(Child1Property, value); } }
        public UIElement Child2 { get { return (UIElement)GetValue(Child2Property); } set { SetValue(Child2Property, value); } }
        public Orientation Orientation { get { return (Orientation)GetValue(OrientationProperty); } set { SetValue(OrientationProperty, value); } }
        public bool SwapChild { get { return (bool)GetValue(SwapChildProperty); } set { SetValue(SwapChildProperty, value); } }
        public double Minimus { get { return (double)GetValue(MinimusSizeProperty); } set { SetValue(MinimusSizeProperty, value); } }

        public SplitContainer()
        {
            this.InitializeComponent();
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if(Orientation==Orientation.Horizontal)
            {
                col1.Width = new GridLength(col1.ActualWidth, GridUnitType.Star);
                col2.Width = new GridLength(col2.ActualWidth, GridUnitType.Star);
            }
            else
            {
                row1.Height = new GridLength(row1.ActualHeight, GridUnitType.Star);
                row2.Height = new GridLength(row2.ActualHeight, GridUnitType.Star);
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Orientation == Orientation.Horizontal)
            {
                double newwidth1 = Math.Max(0, col1.Width.Value + e.HorizontalChange);
                double newwidth2 = Math.Max(0, col2.Width.Value - e.HorizontalChange);
                col1.Width = new GridLength(newwidth1, GridUnitType.Star);
                col2.Width = new GridLength(newwidth2, GridUnitType.Star);
            }
            else
            {
                double newheight1 = Math.Max(0, row1.Height.Value + e.VerticalChange);
                double newheight2 = Math.Max(0, row2.Height.Value - e.VerticalChange);
                row1.Height = new GridLength(newheight1, GridUnitType.Star);
                row2.Height = new GridLength(newheight2, GridUnitType.Star);

            }
        }
    }
}
