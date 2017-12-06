using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace XamlCruncher
{
    class TabbableTextBox : TextBox
    {
        static public DependencyProperty TabSpaceProperty { get; private set; }
        public int TabSpace { get { return (int)GetValue(TabSpaceProperty); } set { SetValue(TabSpaceProperty, value); } }
        static TabbableTextBox() {
            TabSpaceProperty = DependencyProperty.Register("TabSpace", typeof(int), typeof(TabbableTextBox), new PropertyMetadata(4));
        }
        public bool IsModified { get; set; }
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            IsModified = true;
            if(e.Key==Windows.System.VirtualKey.Tab)
            {
                int line, col;
                GetPositionFromIndex(this.SelectionStart, out line, out col);
                int insertcount = TabSpace - col % TabSpace;
                this.SelectedText = new string(' ', insertcount);
                this.SelectionStart += insertcount;
                this.SelectionLength = 0;
                e.Handled = true;
                return;
            }


            base.OnKeyDown(e);
        }

        private void GetPositionFromIndex(int selectionStart, out int line, out int col)
        {
            if(selectionStart>Text.Length)
            {
                line = -1;
                col = -1;
            }
            line = 0;
            col = 0;
            int i = 0;
            while(i<selectionStart)
            {
                if (Text[i] == '\n')
                {
                    line++;
                    col = 0;
                }
                else if (Text[i] == '\r')
                {
                    line++;
                }
                else
                    col++;
                i++;
            }

        }
    }
}
