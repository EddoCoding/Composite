using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Composite.ViewModels.Notes.HardNote;
using Composite.ViewModels.Notes.Note;

namespace Composite.Views.Notes
{
    public partial class AddHardNoteView : UserControl
    {
        public AddHardNoteView() => InitializeComponent();


        //Создание объекта по нажатию Enter и перенос каретки
        void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox)
            {
                int caretIndex = textBox.CaretIndex;
                if (textBox.DataContext is CompositeBase currentComposite)
                {
                    var listView = FindParent<ListView>(textBox);
                    if (listView?.DataContext is AddHardNoteViewModel viewModel)
                    {
                        var newItem = viewModel.HardNoteVM.AddTextComposite(currentComposite, caretIndex);
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var container = listView.ItemContainerGenerator.ContainerFromItem(newItem) as ListViewItem;
                            if (container != null)
                            {
                                var newTextBox = FindChild<TextBox>(container);
                                if (newTextBox != null)
                                {
                                    newTextBox.Focus();
                                    newTextBox.CaretIndex = 0;
                                }
                            }
                        }), DispatcherPriority.Background);
                    }
                }
                e.Handled = true;
            }
        }

        //Проход по элементам ListView стрелочками и перенос каретки в конец значения
        void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down) return;
            if (Keyboard.FocusedElement is not TextBox currentTextBox) return;
            if (currentTextBox.DataContext is not CompositeBase currentItem) return;
            if (listComposite.ItemsSource is not IList<CompositeBase> items) return;

            int index = items.IndexOf(currentItem);
            if (index == -1) return;

            int targetIndex = e.Key == Key.Up ? index - 1 : index + 1;

            if (targetIndex < 0 || targetIndex >= items.Count) return;

            var targetItem = items[targetIndex];

            listComposite.Dispatcher.BeginInvoke(new Action(() =>
            {
                var container = listComposite.ItemContainerGenerator.ContainerFromItem(targetItem) as ListViewItem;
                if (container != null)
                {
                    var targetTextBox = FindChild<TextBox>(container);
                    if (targetTextBox != null)
                    {
                        targetTextBox.Focus();
                        targetTextBox.CaretIndex = targetTextBox.Text.Length;
                    }
                }
            }), DispatcherPriority.Background);

            e.Handled = true;
        }


        T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (parentObject is T parent) return parent;
            else return FindParent<T>(parentObject);
        }
        T? FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild) return typedChild;

                var result = FindChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
