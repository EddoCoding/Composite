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

        //Нажатия клавиш в TextBox
        void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox1)
            {
                int caretIndex = textBox1.CaretIndex;
                if (textBox1.DataContext is CompositeBaseVM currentComposite)
                {
                    var listView = FindParent<ListView>(textBox1);
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
                    e.Handled = true;
                }

            }
            if (e.Key == Key.Back)
            {
                var textBox2 = sender as TextBox;
                var textComposite = textBox2.DataContext as TextCompositeVM;

                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    var listView = FindParent<ListView>(textBox2);
                    if (listView?.DataContext is AddHardNoteViewModel viewModel)
                    {
                        int currentIndex = viewModel.HardNoteVM.Composites.IndexOf(textComposite);
                        viewModel.HardNoteVM.DeleteTextComposite(textComposite);

                        if (currentIndex > 0 && viewModel.HardNoteVM.Composites.Count > 0)
                        {
                            int previousIndex = currentIndex - 1;
                            MoveFocusToTextBox(previousIndex);
                        }
                    }
                    e.Handled = true;
                }
            }
            if (e.Key == Key.Delete)
            {
                var textBox3 = sender as TextBox;
                var textComposite = textBox3.DataContext as TextCompositeVM;

                if (textComposite == null) return;

                var listView = FindParent<ListView>(textBox3);

                if (listView?.DataContext is AddHardNoteViewModel viewModel)
                {
                    int currentIndex = viewModel.HardNoteVM.Composites.IndexOf(textComposite);

                    if (string.IsNullOrEmpty(textBox3.Text))
                    {
                        viewModel.HardNoteVM.DeleteTextComposite(textComposite);

                        TextCompositeVM nextTextComposite = null;
                        int nextTextIndex = -1;

                        for (int i = currentIndex; i < viewModel.HardNoteVM.Composites.Count; i++)
                        {
                            if (viewModel.HardNoteVM.Composites[i] is TextCompositeVM textComp)
                            {
                                nextTextComposite = textComp;
                                nextTextIndex = i;
                                break;
                            }
                        }

                        if (nextTextComposite == null)
                        {
                            for (int i = Math.Min(currentIndex - 1, viewModel.HardNoteVM.Composites.Count - 1); i >= 0; i--)
                            {
                                if (viewModel.HardNoteVM.Composites[i] is TextCompositeVM textComp)
                                {
                                    nextTextComposite = textComp;
                                    nextTextIndex = i;
                                    break;
                                }
                            }
                        }
                        if (nextTextComposite != null) MoveFocusToTextBox(nextTextIndex);

                        e.Handled = true;
                    }
                    else if (textBox3.CaretIndex == textBox3.Text.Length && currentIndex + 1 < viewModel.HardNoteVM.Composites.Count)
                    {
                        var nextElement = viewModel.HardNoteVM.Composites[currentIndex + 1];

                        if (nextElement is TextCompositeVM nextComposite)
                        {
                            if (string.IsNullOrEmpty(nextComposite.Text))
                            {
                                viewModel.HardNoteVM.DeleteTextComposite(nextComposite);
                                e.Handled = true;
                            }
                            else
                            {
                                int originalCaretPosition = textComposite.Text.Length;
                                textComposite.Text += nextComposite.Text;
                                viewModel.HardNoteVM.DeleteTextComposite(nextComposite);

                                textBox3.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    textBox3.CaretIndex = originalCaretPosition;
                                }), DispatcherPriority.Input);

                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }

        //Проход по элементам ListView стрелочками и перенос каретки в конец значения
        void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down) return;
            if (Keyboard.FocusedElement is not TextBox currentTextBox) return;
            if (currentTextBox.DataContext is not CompositeBaseVM currentItem) return;
            if (listComposite.ItemsSource is not IList<CompositeBaseVM> items) return;

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

        void MoveFocusToTextBox(int index)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var listViewItem = listComposite.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;

                if (listViewItem != null)
                {
                    var textBox = FindChild<TextBox>(listViewItem);
                    if (textBox != null)
                    {
                        textBox.Focus();
                        textBox.CaretIndex = textBox.Text.Length;
                    }
                }
            }), DispatcherPriority.Input);
        }
    }
}
