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
            if (listComposite.ItemsSource is IList<CompositeBaseVM> items && index >= 0 && index < items.Count)
            {
                var targetItem = items[index];
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
            }
            else FocusTitleTextBox();
        }
        void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down) return;
            if (Keyboard.FocusedElement is TextBox focusedTextBox)
            {
                if (focusedTextBox == titleTextBox) HandleTitleTextBoxNavigation(e);
                else if (IsTextBoxInListView(focusedTextBox)) HandleListViewNavigation(focusedTextBox, e);
            }
        }
        void TitleTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is AddHardNoteViewModel viewModel)
                {
                    var newItem = new TextCompositeVM { Text = string.Empty };
                    viewModel.HardNoteVM.Composites.Insert(0, newItem);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var container = listComposite.ItemContainerGenerator.ContainerFromItem(newItem) as ListViewItem;
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
            else if (e.Key == Key.Down)
            {
                FocusFirstListViewItem();
                e.Handled = true;
            }
        }
        void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down) return;
            if (Keyboard.FocusedElement is not TextBox currentTextBox) return;
            if (currentTextBox.DataContext is not CompositeBaseVM currentItem) return;
            if (listComposite.ItemsSource is not IList<CompositeBaseVM> items) return;

            int index = items.IndexOf(currentItem);
            if (index == -1) return;
            if (e.Key == Key.Up && index == 0)
            {
                FocusTitleTextBox();
                e.Handled = true;
                return;
            }

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
        void HandleTitleTextBoxNavigation(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                FocusFirstListViewItem();
                e.Handled = true;
            }
        }
        void HandleListViewNavigation(TextBox focusedTextBox, KeyEventArgs e)
        {
            if (focusedTextBox.DataContext is not CompositeBaseVM currentItem) return;
            if (listComposite.ItemsSource is not IList<CompositeBaseVM> items) return;

            int index = items.IndexOf(currentItem);
            if (index == -1) return;
            if (e.Key == Key.Up && index == 0)
            {
                FocusTitleTextBox();
                e.Handled = true;
            }
        }
        void FocusTitleTextBox()
        {
            titleTextBox.Focus();
            titleTextBox.CaretIndex = titleTextBox.Text.Length;
        }
        void FocusFirstListViewItem()
        {
            if (listComposite.ItemsSource is IList<CompositeBaseVM> items && items.Count > 0)
            {
                var firstItem = items[0];
                listComposite.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var container = listComposite.ItemContainerGenerator.ContainerFromItem(firstItem) as ListViewItem;
                    if (container != null)
                    {
                        var textBox = FindChild<TextBox>(container);
                        if (textBox != null)
                        {
                            textBox.Focus();
                            textBox.CaretIndex = textBox.Text.Length;
                        }
                    }
                }), DispatcherPriority.Background);
            }
        }
        bool IsTextBoxInListView(TextBox textBox)
        {
            DependencyObject parent = textBox;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent == listComposite) return true;
                if (parent == titleTextBox) return false;
            }
            return false;
        }

        void ListView_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox1)
            {
                int caretIndex = textBox1.CaretIndex;
                string textValue = textBox1.Text;

                if (textBox1.DataContext is CompositeBaseVM currentComposite)
                {
                    var listView = FindParent<ListView>(textBox1);
                    if (listView?.DataContext is AddHardNoteViewModel viewModel)
                    {
                        var createdComposite = viewModel.HardNoteVM.CreateComposite(textValue, currentComposite, caretIndex);

                        if (createdComposite != null)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                var container = listView.ItemContainerGenerator.ContainerFromItem(createdComposite) as ListViewItem;
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

                            e.Handled = true;
                            return;
                        }

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
                var currentComposite = textBox2.DataContext as CompositeBaseVM;

                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    var listView = FindParent<ListView>(textBox2);
                    if (listView?.DataContext is AddHardNoteViewModel viewModel)
                    {
                        int currentIndex = viewModel.HardNoteVM.Composites.IndexOf(currentComposite);

                        DeleteComposite(viewModel, currentComposite);

                        if (currentIndex == 0) FocusTitleTextBox();
                        else if (currentIndex > 0 && viewModel.HardNoteVM.Composites.Count > 0)
                        {
                            int previousIndex = currentIndex - 1;
                            MoveFocusToTextBox(previousIndex);
                            //MoveFocusToTextBox(previousIndex, viewModel);
                        }
                        else FocusTitleTextBox();
                    }
                    e.Handled = true;
                }
            }
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                if (Keyboard.FocusedElement is not TextBox currentTextBox) return;
                if (currentTextBox.DataContext is not CompositeBaseVM currentItem) return;
                if (listComposite.ItemsSource is not IList<CompositeBaseVM> items) return;

                int index = items.IndexOf(currentItem);
                if (index == -1) return;

                if (e.Key == Key.Up && index == 0)
                {
                    FocusTitleTextBox();
                    e.Handled = true;
                    return;
                }

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

        void DeleteComposite(AddHardNoteViewModel viewModel, CompositeBaseVM composite)
        {
            switch (composite)
            {
                case TextCompositeVM textComposite:
                    viewModel.HardNoteVM.DeleteTextComposite(textComposite);
                    break;

                case HeaderCompositeVM headerComposite:
                    viewModel.HardNoteVM.DeleteCheck(headerComposite);
                    break;
            }
        }
    }
}
