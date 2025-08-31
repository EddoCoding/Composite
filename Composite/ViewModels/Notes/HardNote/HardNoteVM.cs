using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class HardNoteVM : NoteBaseVM
    {
        public ObservableCollection<CompositeBaseVM> Composites { get; set; }

        public HardNoteVM()
        {
            Id = Guid.NewGuid();
            Composites = new();

            Composites.Add(new HeaderCompositeVM());
            Composites.Add(new TextCompositeVM() { Text = "Проверка текста1"});
        }

        public TextCompositeVM AddTextComposite(CompositeBaseVM current, int caretIndex)
        {
            var newItem = new TextCompositeVM { Text = string.Empty };
            if (current is HeaderCompositeVM headerComposite)
            {
                int index = Composites.IndexOf(headerComposite);
                Composites.Insert(index + 1, newItem);
                return newItem;
            }
            else if (current is TextCompositeVM textComposite)
            {
                int index = Composites.IndexOf(textComposite);
                if (caretIndex == 0)
                {
                    if (string.IsNullOrEmpty(textComposite.Text))
                    {
                        Composites.Insert(index + 1, newItem);
                        return newItem;
                    }
                    else
                    {
                        Composites.Insert(index, newItem);
                        return newItem;
                    }
                }
                else if (caretIndex == textComposite.Text.Length)
                {
                    Composites.Insert(index + 1, newItem);
                    return newItem;
                }
                else
                {
                    if (caretIndex >= 0 && caretIndex < textComposite.Text.Length)
                    {
                        var textAfter = textComposite.Text.Substring(caretIndex);
                        newItem.Text = textAfter;
                        textComposite.Text = textComposite.Text.Substring(0, caretIndex);
                        Composites.Insert(index + 1, newItem);
                        return newItem;
                    }
                    else
                    {
                        Composites.Insert(index + 1, newItem);
                        return newItem;
                    }
                }
            }
            return null;
        }
        public void DeleteTextComposite(TextCompositeVM textComposite) => Composites.Remove(textComposite);
    }
}