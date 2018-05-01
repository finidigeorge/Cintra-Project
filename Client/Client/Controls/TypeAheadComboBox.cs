using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Controls
{
    public class TypeAheadComboBox: ComboBox
    {
        public TypeAheadComboBox()
        {
            IsEditable = true;
            StaysOpenOnEdit = true;
            PreviewTextInput += new TextCompositionEventHandler(OnPreviewTextInputHandler);
        }

        protected void OnPreviewTextInputHandler(object sender, TextCompositionEventArgs e)
        {
            IsDropDownOpen = true;
        }
    }
}
