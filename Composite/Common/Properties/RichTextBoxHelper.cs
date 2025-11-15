using Composite.Common.Helpers;
using Composite.ViewModels.Notes.HardNote;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Composite.Common.Properties
{
    public class RichTextBoxHelper
    {
        private static bool _isUpdating = false;

        //Для биндинга RichTextBox.Document к FlowDocumewwnt во ViewModel
        public static readonly DependencyProperty BindToViewModelProperty =
            DependencyProperty.RegisterAttached("BindToViewModel", typeof(bool), typeof(RichTextBoxHelper), new PropertyMetadata(false, OnBindToViewModelChanged));
        public static bool GetBindToViewModel(DependencyObject obj) => (bool)obj.GetValue(BindToViewModelProperty);
        public static void SetBindToViewModel(DependencyObject obj, bool value) => obj.SetValue(BindToViewModelProperty, value);
        static void OnBindToViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBox rtb) return;

            if ((bool)e.NewValue)
            {
                rtb.DataContextChanged += OnDataContextChanged;
                rtb.Loaded += OnRichTextBoxLoaded;
            }
            else
            {
                rtb.DataContextChanged -= OnDataContextChanged;
                rtb.Loaded -= OnRichTextBoxLoaded;
                rtb.TextChanged -= OnTextChanged;
            }
        }
        static void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is RichTextBox rtb && e.NewValue is FormattedTextCompositeVM vm) LoadDocument(rtb, vm);
        }
        static void OnRichTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            var rtb = (RichTextBox)sender;

            rtb.TextChanged -= OnTextChanged;
            rtb.TextChanged += OnTextChanged;

            if (rtb.DataContext is FormattedTextCompositeVM vm) LoadDocument(rtb, vm);
        }
        static void LoadDocument(RichTextBox rtb, FormattedTextCompositeVM vm)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                rtb.Document = vm.Document ?? new FlowDocument(new Paragraph { LineHeight = 7 });
            }
            catch { }
            finally { _isUpdating = false; }
        }
        static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating || sender is not RichTextBox rtb) return;

            _isUpdating = true;
            try
            {
                if (rtb.DataContext is FormattedTextCompositeVM vm) vm.OnTextChanged(rtb.Document);
            }
            finally { _isUpdating = false; }
        }

        //Автоматичесий показ Popup при выборке фрагмента текста
        public static readonly DependencyProperty EnableFormattingPopupProperty =
            DependencyProperty.RegisterAttached("EnableFormattingPopup", typeof(bool), typeof(RichTextBoxHelper), new PropertyMetadata(false, OnEnableFormattingPopupChanged));
        public static bool GetEnableFormattingPopup(DependencyObject obj) => (bool)obj.GetValue(EnableFormattingPopupProperty);
        public static void SetEnableFormattingPopup(DependencyObject obj, bool value) => obj.SetValue(EnableFormattingPopupProperty, value);
        static void OnEnableFormattingPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox rtb)
            {
                if ((bool)e.NewValue)
                {
                    rtb.SelectionChanged += RichTextBox_SelectionChanged;
                    rtb.PreviewMouseDown += RichTextBox_PreviewMouseDown;
                    rtb.LostFocus += RichTextBox_LostFocus;
                }
                else
                {
                    rtb.SelectionChanged -= RichTextBox_SelectionChanged;
                    rtb.PreviewMouseDown -= RichTextBox_PreviewMouseDown;
                    rtb.LostFocus -= RichTextBox_LostFocus;
                }
            }
        }
        static void RichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            var popup = FindPopup(rtb);
            if (popup == null) return;

            rtb.Dispatcher.BeginInvoke(new Action(() =>
            {
                var focusedElement = Keyboard.FocusedElement as DependencyObject;

                if (focusedElement != null && IsElementInsidePopup(focusedElement, popup)) return;

                popup.IsOpen = false;
                if (!rtb.Selection.IsEmpty) rtb.Selection.Select(rtb.Document.ContentStart, rtb.Document.ContentStart);
            }), DispatcherPriority.Background);
        }
        static bool IsElementInsidePopup(DependencyObject element, Popup popup)
        {
            if (element == null || popup == null) return false;

            DependencyObject current = element;
            while (current != null)
            {
                if (current == popup || current == popup.Child) return true;
                current = VisualTreeHelper.GetParent(current);
            }
            return false;
        }
        static void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var rtb = sender as RichTextBox;
            var popup = FindPopup(rtb);
            if (popup == null) return;

            if (!rtb.Selection.IsEmpty)
            {
                TextPointer start = rtb.Selection.Start;
                Rect rect = start.GetCharacterRect(LogicalDirection.Forward);

                popup.PlacementRectangle = new Rect(rect.X, rect.Y - 50, 0, 0);
                popup.IsOpen = true;
            }
            else popup.IsOpen = false;
        }
        static void RichTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var rtb = sender as RichTextBox;

            rtb.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (rtb.Selection.IsEmpty)
                {
                    var popup = FindPopup(rtb);
                    if (popup != null) popup.IsOpen = false;
                }
            }), DispatcherPriority.Background);
        }
        static Popup FindPopup(RichTextBox rtb)
        {
            DependencyObject parent = rtb.Parent;
            if (parent == null) return null;
            if (parent is Border) parent = VisualTreeHelper.GetParent(parent);
            if (parent is Panel panel)
            {
                foreach (var child in panel.Children)
                    if (child is Popup popup && popup.Name == "formattingPopup") return popup;
            }

            return FindPopupInVisualTree(parent);
        }
        static Popup FindPopupInVisualTree(DependencyObject parent)
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Popup popup && popup.Name == "formattingPopup") return popup;

                var found = FindPopupInVisualTree(child);
                if (found != null) return found;
            }

            return null;
        }

        //Фичи: зачеркивание, черта сверху, выборка шрифта.
        public static readonly DependencyProperty EnableTextFormattingProperty =
            DependencyProperty.RegisterAttached("EnableTextFormatting", typeof(bool), typeof(RichTextBoxHelper), new PropertyMetadata(false, OnEnableChanged));
        public static void SetEnableTextFormatting(DependencyObject d, bool value) => d.SetValue(EnableTextFormattingProperty, value);
        public static bool GetEnableTextFormatting(DependencyObject d) => (bool)d.GetValue(EnableTextFormattingProperty);
        static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBox rtb) return;

            if ((bool)e.NewValue)
            {
                AddCommandBinding(rtb, FormattedTextCommands.ToggleStrikethrough, TextDecorationLocation.Strikethrough);             //Для зачеркивания
                AddCommandBinding(rtb, FormattedTextCommands.ToggleOverline, TextDecorationLocation.OverLine);                       //Для Надчеркивания
                rtb.CommandBindings.Add(new CommandBinding(FormattedTextCommands.ChangeFont, OnChangeFontExecuted));                 //Для выборки шрифта
                rtb.CommandBindings.Add(new CommandBinding(FormattedTextCommands.ChangeSize, OnChangeSizeExecuted));                 //Для выборки размера
                rtb.CommandBindings.Add(new CommandBinding(FormattedTextCommands.ChangeTextColor, OnChangeTextColorExecuted));       //Для выборки цвета текста
                rtb.CommandBindings.Add(new CommandBinding(FormattedTextCommands.ChangeBgTextColor, OnChangeBgTextColorExecuted));   //Для выборки заднего фона текста
            }
            else
            {
                var cmds = new[] { FormattedTextCommands.ToggleStrikethrough, FormattedTextCommands.ToggleOverline };
                var toRemove = rtb.CommandBindings.OfType<CommandBinding>().Where(cb => cmds.Contains(cb.Command)).ToList();
                foreach (var cb in toRemove) rtb.CommandBindings.Remove(cb);
            }
        }
        static void AddCommandBinding(RichTextBox rtb, RoutedUICommand command, TextDecorationLocation location)
        {
            if (rtb.CommandBindings.OfType<CommandBinding>().Any(cb => cb.Command == command)) return;

            var cb = new CommandBinding(command, (s, e) => ApplyDecoration(rtb, location), (s, e) => e.CanExecute = !rtb.Selection.IsEmpty);
            rtb.CommandBindings.Add(cb);
        }
        static void ApplyDecoration(RichTextBox rtb, TextDecorationLocation location)
        {
            var sel = rtb.Selection;
            if (sel == null || sel.IsEmpty) return;

            var current = sel.GetPropertyValue(Inline.TextDecorationsProperty);
            if (current == DependencyProperty.UnsetValue || current == null)
            {
                sel.ApplyPropertyValue(Inline.TextDecorationsProperty, new TextDecorationCollection { new TextDecoration(location, null, 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended) });
                return;
            }

            var decorations = (current as TextDecorationCollection)?.CloneCurrentValue() ?? new TextDecorationCollection();
            var existing = decorations.FirstOrDefault(d => d.Location == location);

            if (existing != null) decorations.Remove(existing);
            else decorations.Add(new TextDecoration(location, null, 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended));

            sel.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations.Count > 0 ? decorations : null);
        }
        static void OnChangeFontExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is not RichTextBox rtb) return;

            string fontName = e.Parameter as string ?? "Trebuchet MS, Times New Roman, Arial";
            ApplyFont(rtb, fontName);
        }
        static void ApplyFont(RichTextBox rtb, string fontFamilyName)
        {
            var sel = rtb.Selection;
            if (sel == null || sel.IsEmpty) return;

            try
            {
                sel.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamilyName);
            }
            catch
            {
                MessageBox.Show($"The font '{fontFamilyName}' was not found in the system.");
            }
        }
        static void OnChangeSizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is not RichTextBox rtb) return;

            if (double.TryParse(e.Parameter.ToString(), out double fontSize)) ApplySize(rtb, fontSize);
            else ApplySize(rtb, 16);
        }
        static void ApplySize(RichTextBox rtb, double fontSize)
        {
            var sel = rtb.Selection;
            if (sel == null || sel.IsEmpty) return;

            try
            {
                if (fontSize > 0) sel.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying font size.");
            }
        }


        static void OnChangeTextColorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is not RichTextBox rtb) return;

            string textColor = e.Parameter as string ?? "Black";
            ApplyTextColor(rtb, textColor);
        }
        static void ApplyTextColor(RichTextBox rtb, string textColor)
        {
            var sel = rtb.Selection;
            if (sel == null || sel.IsEmpty) return;

            try
            {
                sel.ApplyPropertyValue(TextElement.ForegroundProperty, textColor);
            }
            catch
            {
                MessageBox.Show($"The color '{textColor}' was not found in the system.");
            }
        }


        static void OnChangeBgTextColorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is not RichTextBox rtb) return;

            string bgTextColor = e.Parameter as string ?? "White";
            ApplyBgTextColor(rtb, bgTextColor);
        }
        static void ApplyBgTextColor(RichTextBox rtb, string bgTextColor)
        {
            var sel = rtb.Selection;
            if (sel == null || sel.IsEmpty) return;

            try
            {
                sel.ApplyPropertyValue(TextElement.BackgroundProperty, bgTextColor);
            }
            catch
            {
                MessageBox.Show($"The color '{bgTextColor}' was not found in the system.");
            }
        }
    }
}