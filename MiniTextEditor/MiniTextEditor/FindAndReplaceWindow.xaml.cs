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

namespace MiniTextEditor
{
    /// <summary>
    /// Interaction logic for FindAndReplaceWindow.xaml
    /// </summary>
    public partial class FindAndReplaceWindow : Window
    {
        public TextRange rtb = null;

        public FindAndReplaceWindow()
        {
            InitializeComponent();
        }


        public FindAndReplaceWindow(TextRange tr)
        {
            InitializeComponent();

            rtb = tr;
        }

        private void buttonNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool validateFind()
        {
            bool retVal = true;

            if (textBoxFind.Text.Equals(""))
            {
                retVal = false;
            }

            return retVal;
        }

        public bool validateReplace()
        {
            bool retVal = true;

            if (textBoxReplace.Text.Equals(""))
            {
                retVal = false;
            }

            return retVal;
        }

        private void buttonYes_Click(object sender, RoutedEventArgs e)
        {
            if (validateFind())
            {
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString("#FF7A695C");
                textBoxFind.BorderBrush = brush;
                textBoxFind.BorderThickness = new Thickness();

                if (validateReplace())
                {
                    var brushR = (Brush)converter.ConvertFromString("#FF7A695C");
                    textBoxReplace.BorderBrush = brushR;
                    textBoxReplace.BorderThickness = new Thickness();

                    string keyword = textBoxFind.Text;
                    string replaceString = textBoxReplace.Text;

                    TextRange text = rtb;
                    TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);

                    while (current != null)
                    {
                        string textInRun = current.GetTextInRun(LogicalDirection.Forward);

                        if (!string.IsNullOrWhiteSpace(textInRun))
                        {
                            int index = textInRun.IndexOf(keyword);
                            if (index != -1)
                            {
                                TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                                TextPointer selectionEnd = selectionStart.GetPositionAtOffset(keyword.Length, LogicalDirection.Forward);
                                TextRange selection = new TextRange(selectionStart, selectionEnd);

                                object fontWeight = selection.GetPropertyValue(Inline.FontWeightProperty);
                                object fontColor = selection.GetPropertyValue(Inline.ForegroundProperty);
                                object fontSize = selection.GetPropertyValue(Inline.FontSizeProperty);
                                object fontFamily = selection.GetPropertyValue(Inline.FontFamilyProperty);
                                object fontStyle = selection.GetPropertyValue(Inline.FontStyleProperty);
                                object fontDecoration = selection.GetPropertyValue(Inline.TextDecorationsProperty);


                                selection.Text = replaceString;

                                selection.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
                                selection.ApplyPropertyValue(TextElement.ForegroundProperty, fontColor);
                                selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                                selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamily);
                                selection.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
                                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, fontDecoration);

                            }
                        }
                        current = current.GetNextContextPosition(LogicalDirection.Forward);
                    }
                }
                else
                {
                    textBoxReplace.BorderBrush = Brushes.Red;
                    textBoxReplace.BorderThickness = new Thickness(2);
                }
            }
            else
            {
                textBoxFind.BorderBrush = Brushes.Red;
                textBoxFind.BorderThickness = new Thickness(2);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
