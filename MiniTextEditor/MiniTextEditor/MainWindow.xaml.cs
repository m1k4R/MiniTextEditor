using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniTextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool changed = false;
        int wordNumber;

        public MainWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);  //u fontfamily comboBox u item source uzmi fontove koji postoje u sistemu racunara i poredaj ih sistemski kao u wordu i drugim programima
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            WordNumber.Text = "0";
            DataContext = this;
            rtbEditor.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);  // hocu da mi se u object uzme iz selektovanog teksta koja mu je vrijednost propertija, nakon toga cu da vidim u zavisnosti da li mi se properti vezan za ovo polje ima zadatu vrijednost ili nema i da je ona konkretno bold, i stavim u batton kao da je cekirano
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            if (btnBold.IsChecked == true)
            {
                btnBold.Background.Opacity = 1;              
            }
            else
            {
                btnBold.Background.Opacity = 0.5;
            }

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            if (btnItalic.IsChecked == true)
            {
                btnItalic.Background.Opacity = 1;
            }
            else
            {
                btnItalic.Background.Opacity = 0.5;
            }

            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));
            if (btnUnderline.IsChecked == true)
            {
                btnUnderline.Background.Opacity = 1;
            }
            else
            {
                btnUnderline.Background.Opacity = 0.5;
            }

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty); //kada selektujemo tekst bude napisano koji je to tekst u combo boxu
            cmbFontFamily.SelectedItem = temp;

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            try
            {
                Int32.Parse(temp.ToString());
                cmbFontSize.Text = temp.ToString();
            }
            catch (Exception exc)
            {

            }

            var boja = rtbEditor.Selection.GetPropertyValue(Inline.ForegroundProperty);
            try
            {
                btnColor.Background = (Brush)boja;
            }
            catch (Exception exc)
            {

            }

            changed = true;
            wordNumber = 0;
            string text = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd).Text;
            var txt = text.Trim();
            int wordCounter = 0;
            int index = 0;

            while (index < txt.Length)
            {
                // check if current char is part of a word
                while (index < txt.Length && !char.IsWhiteSpace(txt[index]))
                    index++;

                wordCounter++;

                // skip whitespace until next word
                while (index < txt.Length && char.IsWhiteSpace(txt[index]))
                    index++;
            }
            wordNumber = wordCounter;
            WordNumber.Text = wordCounter.ToString();
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
            }
            rtbEditor.Focus();
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
               Int32.Parse(cmbFontSize.Text.Trim());

                if (Int32.Parse(cmbFontSize.Text.Trim()) < 1000)
                {

                    rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
                }
                rtbEditor.Focus();
            }
            catch (Exception exc)
            {
             
                MessageBox.Show("This is not a valid number.", "Mini Text Editor", MessageBoxButton.OK, MessageBoxImage.Warning);
                rtbEditor.Focus();
                object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
                try
                {
                    Int32.Parse(temp.ToString());
                    cmbFontSize.Text = temp.ToString();
                }
                catch (Exception ex)
                {

                }
            }
            
        }

        private void buttonIzlaz_Click(object sender, RoutedEventArgs e)
        {
            if (wordNumber == 0 || changed == false)
            {
                this.Close();
            }
            else
            {
                var result = MessageBox.Show("Do you want to save?", "Mini Text Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                    if (dlg.ShowDialog() == true)
                    {
                        FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                        range.Save(fileStream, DataFormats.Rtf);
                        fileStream.Close();
                        this.Close();
                    }

                }
                else
                {
                    this.Close();
                }
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (wordNumber == 0 && changed == false)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                if (dlg.ShowDialog() == true)
                {
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                    TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    range.Load(fileStream, DataFormats.Rtf);
                    fileStream.Close();
                    changed = false;
                }
            }
            else
            {
                var result = MessageBox.Show("Do you want to save?", "Mini Text Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                    if (dlg.ShowDialog() == true)
                    {
                        FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                        range.Save(fileStream, DataFormats.Rtf);
                        fileStream.Close();
                    }

                    OpenFileDialog dlg1 = new OpenFileDialog();
                    dlg1.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                    if (dlg1.ShowDialog() == true)
                    {
                        FileStream fileStream = new FileStream(dlg1.FileName, FileMode.Open);
                        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                        range.Load(fileStream, DataFormats.Rtf);
                        fileStream.Close();
                        WordNumber.Text = "0";
                        wordNumber = 0;
                        changed = false;
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    OpenFileDialog dlg1 = new OpenFileDialog();
                    dlg1.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                    if (dlg1.ShowDialog() == true)
                    {
                        FileStream fileStream = new FileStream(dlg1.FileName, FileMode.Open);
                        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                        range.Load(fileStream, DataFormats.Rtf);
                        fileStream.Close();
                        WordNumber.Text = "0";
                        wordNumber = 0;
                        changed = false;
                    }
                }
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
                changed = false;
            }
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
                      
                if (wordNumber == 0 && changed == false)
                {
                    rtbEditor.Document.Blocks.Clear();
                    changed = false;
                }
                else
                {
                    var result = MessageBox.Show("Do you want to save?", "Mini Text Editor", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SaveFileDialog dlg = new SaveFileDialog();
                        dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                        if (dlg.ShowDialog() == true)
                        {
                            FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                            range.Save(fileStream, DataFormats.Rtf);
                            fileStream.Close();
                            rtbEditor.Document.Blocks.Clear();
                            WordNumber.Text = "0";
                            wordNumber = 0;
                            changed = false;
                        }
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        rtbEditor.Document.Blocks.Clear();
                        WordNumber.Text = "0";
                        wordNumber = 0;
                        changed = false;
                    }
                }
        }

        private void fontcolor(RichTextBox rc)
        {
            
            var colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var wpfcolor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                btnColor.Background = new SolidColorBrush(wpfcolor);
                TextRange range = new TextRange(rc.Selection.Start, rc.Selection.End);
                range.ApplyPropertyValue(FlowDocument.ForegroundProperty, new SolidColorBrush(wpfcolor));
                rtbEditor.Selection.ApplyPropertyValue(Inline.ForegroundProperty, new SolidColorBrush(wpfcolor));
                rtbEditor.Focus();
                
            }
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            changed = true;
            fontcolor(rtbEditor);
        }

        private void Word_Counter(object sender, TextChangedEventArgs e)
        {
            changed = true;
            wordNumber = 0;
            string text = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd).Text;
            var txt = text.Trim();
            int wordCounter = 0;
            int index = 0;

            while (index < txt.Length)
            {
                // check if current char is part of a word
                while (index < txt.Length && !char.IsWhiteSpace(txt[index]))
                    index++;

                wordCounter++;

                // skip whitespace until next word
                while (index < txt.Length && char.IsWhiteSpace(txt[index]))
                    index++;
            }
            wordNumber = wordCounter;
            WordNumber.Text = wordCounter.ToString();

        }

        private void btnFR_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

            FindAndReplaceWindow FR = new FindAndReplaceWindow(range);
            FR.ShowDialog();
        }

        private void btnDateTime_Click(object sender, RoutedEventArgs e)
        {
            rtbEditor.CaretPosition.InsertTextInRun(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        }

        private void rtbEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                rtbEditor.CaretPosition.InsertTextInRun(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            }
        }

        private void btnBold_Checked(object sender, RoutedEventArgs e)
        {
            changed = true;
            if (btnBold.IsChecked == true)
            {
                btnBold.Background.Opacity = 1;
            }
            else
            {
                btnBold.Background.Opacity = 0.5;
            }
        }

        private void btnItalic_Checked(object sender, RoutedEventArgs e)
        {
            changed = true;
            if (btnItalic.IsChecked == true)
            {
                btnItalic.Background.Opacity = 1;
            }
            else
            {
                btnItalic.Background.Opacity = 0.5;
            }
        }

        private void btnUnderline_Checked(object sender, RoutedEventArgs e)
        {
            changed = true;
            if (btnUnderline.IsChecked == true)
            {
                btnUnderline.Background.Opacity = 1;
            }
            else
            {
                btnUnderline.Background.Opacity = 0.5;
            }
        }
    }
}
