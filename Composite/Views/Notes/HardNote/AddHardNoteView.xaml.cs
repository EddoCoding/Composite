﻿using Composite.ViewModels.Notes.HardNote;
using Composite.ViewModels.Notes.Note;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Composite.Views.Notes
{
    public partial class AddHardNoteView : UserControl
    {
        public AddHardNoteView()
        {
            InitializeComponent();
            Loaded += (s, e) => titleTextBox.Focus();
        }

        void PositionPopupType(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) PopupType.PlacementTarget = button;
        }

        T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (parentObject is T parent) return parent;
            else return FindParent<T>(parentObject);
        }
        T? FindChildInColumn<T>(DependencyObject parent, int column) where T : UIElement
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is UIElement uiElement && Grid.GetColumn(uiElement) == column)
                {
                    if (child is T typedChild) return typedChild;

                    var innerResult = FindChildRecursive<T>(child);
                    if (innerResult != null) return innerResult;
                }

                var result = FindChildInColumn<T>(child, column);
                if (result != null) return result;
            }
            return null;
        }
        T? FindChildRecursive<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild) return typedChild;

                var result = FindChildRecursive<T>(child);
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
                        var targetTextBox = FindChildInColumn<TextBox>(container, 2);
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
                    viewModel.HardNoteVM.InsertComposite(0, newItem);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var container = listComposite.ItemContainerGenerator.ContainerFromItem(newItem) as ListViewItem;
                        if (container != null)
                        {
                            var newTextBox = FindChildInColumn<TextBox>(container, 2);
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

            int targetIndex = FindNextTextBoxIndex(items, index, e.Key == Key.Up);

            if (targetIndex == -1)
            {
                if (e.Key == Key.Up)
                {
                    FocusTitleTextBox();
                    e.Handled = true;
                }
                return;
            }

            var targetItem = items[targetIndex];
            listComposite.Dispatcher.BeginInvoke(new Action(() =>
            {
                var container = listComposite.ItemContainerGenerator.ContainerFromItem(targetItem) as ListViewItem;
                if (container != null)
                {
                    var targetTextBox = FindChildInColumn<TextBox>(container, 2);
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
                        var textBox = FindChildInColumn<TextBox>(container, 2);
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
                                    var newTextBox = FindChildInColumn<TextBox>(container, 2);
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

                        var newItem = viewModel.HardNoteVM.AddComposite(currentComposite, caretIndex);
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var container = listView.ItemContainerGenerator.ContainerFromItem(newItem) as ListViewItem;
                            if (container != null)
                            {
                                var newTextBox = FindChildInColumn<TextBox>(container, 2);
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
                if (currentComposite == null) return;
                var listView = FindParent<ListView>(textBox2);
                if (listView?.DataContext is AddHardNoteViewModel viewModel)
                {
                    int currentIndex = viewModel.HardNoteVM.GetIndexComposite(currentComposite);

                    if (string.IsNullOrEmpty(textBox2.Text))
                    {
                        DeleteComposite(viewModel, currentComposite);

                        if (currentIndex == 0) FocusTitleTextBox();
                        else if (viewModel.HardNoteVM.Composites.Count > 0)
                        {
                            int previousTextBoxIndex = FindPreviousTextBoxIndex(viewModel.HardNoteVM.Composites, currentIndex);
                            if (previousTextBoxIndex != -1) MoveFocusToTextBox(previousTextBoxIndex);
                            else FocusTitleTextBox();
                        }
                        else FocusTitleTextBox();
                        e.Handled = true;
                    }
                    else if (textBox2.CaretIndex == 0 && currentIndex > 0 && currentComposite is TextCompositeVM currentTextComposite)
                    {
                        var previousTextComposite = FindPreviousTextComposite(viewModel.HardNoteVM.Composites, currentIndex);
                        if (previousTextComposite != null)
                        {
                            int originalCaretPosition = previousTextComposite.Text.Length;
                            previousTextComposite.Text += currentTextComposite.Text;
                            viewModel.HardNoteVM.DeleteComposite(currentTextComposite);

                            int previousIndex = viewModel.HardNoteVM.GetIndexComposite(previousTextComposite);
                            textBox2.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                MoveFocusToTextBox(previousIndex);
                                var container = listComposite.ItemContainerGenerator.ContainerFromItem(previousTextComposite) as ListViewItem;
                                if (container != null)
                                {
                                    var targetTextBox = FindChildInColumn<TextBox>(container, 2);
                                    if (targetTextBox != null) targetTextBox.CaretIndex = originalCaretPosition;
                                }
                            }), DispatcherPriority.Input);
                            e.Handled = true;
                        }
                    }
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
                    int currentIndex = viewModel.HardNoteVM.GetIndexComposite(textComposite);
                    if (string.IsNullOrEmpty(textBox3.Text))
                    {
                        DeleteComposite(viewModel, textComposite);

                        int nextTextBoxIndex = FindNextTextBoxIndex(viewModel.HardNoteVM.Composites, currentIndex);
                        if (nextTextBoxIndex == -1) nextTextBoxIndex = FindPreviousTextBoxIndex(viewModel.HardNoteVM.Composites, currentIndex);
                        if (nextTextBoxIndex != -1) MoveFocusToTextBox(nextTextBoxIndex);

                        e.Handled = true;
                    }
                    else if (textBox3.CaretIndex == textBox3.Text.Length && currentIndex + 1 < viewModel.HardNoteVM.Composites.Count)
                    {
                        var nextTextComposite = FindNextTextComposite(viewModel.HardNoteVM.Composites, currentIndex);
                        if (nextTextComposite != null)
                        {
                            if (string.IsNullOrEmpty(nextTextComposite.Text))
                            {
                                DeleteComposite(viewModel, nextTextComposite);
                                e.Handled = true;
                            }
                            else
                            {
                                int originalCaretPosition = textComposite.Text.Length;
                                textComposite.Text += nextTextComposite.Text;
                                DeleteComposite(viewModel, nextTextComposite);
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

        void DeleteComposite(AddHardNoteViewModel viewModel, CompositeBaseVM composite) => viewModel.HardNoteVM.DeleteComposite(composite);

        int FindNextTextBoxIndex(IList<CompositeBaseVM> items, int currentIndex, bool goingUp)
        {
            int direction = goingUp ? -1 : 1;
            int targetIndex = currentIndex + direction;

            while (targetIndex >= 0 && targetIndex < items.Count)
            {
                var item = items[targetIndex];
                if (item is not LineCompositeVM && item is not ImageCompositeVM && item is not RefCompositeVM) return targetIndex;
                targetIndex += direction;
            }

            return -1;
        }
        int FindNextTextBoxIndex(IList<CompositeBaseVM> items, int startIndex)
        {
            for (int i = startIndex; i < items.Count; i++)
            {
                var item = items[i];
                if (item is TextCompositeVM || item is HeaderCompositeVM || item is QuoteCompositeVM || item is TaskCompositeVM || item is MarkerCompositeVM
                    || item is NumericCompositeVM || item is CodeCompositeVM) return i;
            }
            return -1;
        }
        int FindPreviousTextBoxIndex(IList<CompositeBaseVM> items, int startIndex)
        {
            for (int i = startIndex - 1; i >= 0; i--)
            {
                var item = items[i];
                if (item is TextCompositeVM || item is HeaderCompositeVM || item is QuoteCompositeVM || item is TaskCompositeVM || item is MarkerCompositeVM
                    || item is NumericCompositeVM || item is CodeCompositeVM) return i;
            }
            return -1;
        }
        TextCompositeVM FindPreviousTextComposite(IList<CompositeBaseVM> items, int startIndex)
        {
            for (int i = startIndex - 1; i >= 0; i--)
            {
                if (items[i] is TextCompositeVM textComp) return textComp;
            }
            return null;
        }
        TextCompositeVM FindNextTextComposite(IList<CompositeBaseVM> items, int startIndex)
        {
            for (int i = startIndex + 1; i < items.Count; i++)
            {
                if (items[i] is TextCompositeVM textComp) return textComp;
            }
            return null;
        }

        void listComposite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            if (listView?.DataContext is not AddHardNoteViewModel viewModel) return;

            var composites = viewModel.HardNoteVM.Composites;
            if (composites.Count > 0)
            {
                var lastElement = composites[composites.Count - 1];
                if (lastElement is TextCompositeVM || lastElement is HeaderCompositeVM || lastElement is QuoteCompositeVM)
                {
                    listView.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MoveFocusToTextBox(composites.Count - 1);
                    }), DispatcherPriority.Input);
                    return;
                }
            }

            viewModel.HardNoteVM.AddTextCompositeVM();

            listView.Dispatcher.BeginInvoke(new Action(() =>
            {
                MoveFocusToTextBox(viewModel.HardNoteVM.Composites.Count - 1);
            }), DispatcherPriority.Input);
        }

        //Для медленной прокрутки listView
        void listComposite_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var scrollViewer = GetScrollViewer(listComposite);
            if (scrollViewer != null)
            {
                double offset = e.Delta / 3.0;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
            }
        }
        ScrollViewer GetScrollViewer(DependencyObject o)
        {
            if (o is ScrollViewer sv) return sv;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}