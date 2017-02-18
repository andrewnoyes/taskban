using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TamedTasks.Util
{
    public class Library
    {
        private void focus(ref RichEditBox display)
        {
            display.Focus(FocusState.Keyboard);
        }

        private void set(ref RichEditBox display, string value)
        {
            display.Document.SetText(TextSetOptions.FormatRtf, value);
            focus(ref display);
        }

        public string get(ref RichEditBox display)
        {
            string value = string.Empty;
            display.Document.GetText(TextGetOptions.FormatRtf, out value);
            return value;
        }

        public bool Bold(ref RichEditBox display)
        {
            display.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
            focus(ref display);
            return display.Document.Selection.CharacterFormat.Bold.Equals(FormatEffect.On);
        }

        public bool Italic(ref RichEditBox display)
        {
            display.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
            focus(ref display);
            return display.Document.Selection.CharacterFormat.Italic.Equals(FormatEffect.On);
        }

        public bool Underline(ref RichEditBox display)
        {
            display.Document.Selection.CharacterFormat.Underline =
                display.Document.Selection.CharacterFormat.Underline.Equals(UnderlineType.Single) ?
                UnderlineType.None : UnderlineType.Single;
            display.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
            focus(ref display);
            return display.Document.Selection.CharacterFormat.Underline.Equals(UnderlineType.Single);
        }

        public bool Left(ref RichEditBox display)
        {
            display.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            focus(ref display);
            return display.Document.Selection.ParagraphFormat.Alignment.Equals(ParagraphAlignment.Left);
        }

        public bool Center(ref RichEditBox display)
        {
            display.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            focus(ref display);
            return display.Document.Selection.ParagraphFormat.Alignment.Equals(ParagraphAlignment.Center);
        }

        public bool Right(ref RichEditBox display)
        {
            display.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            focus(ref display);
            return display.Document.Selection.ParagraphFormat.Alignment.Equals(ParagraphAlignment.Right);
        }

        public void Size(ref RichEditBox display, ref ComboBox value)
        {
            if (display != null && value != null)
            {
                string selected = ((ComboBoxItem)value.SelectedItem).Tag.ToString();
                display.Document.Selection.CharacterFormat.Size = float.Parse(selected);
                focus(ref display);
            }
        }

        public void ApplyColor(ref RichEditBox display, ref ComboBox value)
        {
            if (display != null && value != null)
            {
                string selected = ((ComboBoxItem)value.SelectedItem).Tag.ToString();
                display.Document.Selection.CharacterFormat.ForegroundColor = Color.FromArgb(
                    Byte.Parse(selected.Substring(0, 2), NumberStyles.HexNumber),
                    Byte.Parse(selected.Substring(2, 2), NumberStyles.HexNumber),
                    Byte.Parse(selected.Substring(4, 2), NumberStyles.HexNumber),
                    Byte.Parse(selected.Substring(6, 2), NumberStyles.HexNumber));
                focus(ref display);
            }
        }
    }
}
