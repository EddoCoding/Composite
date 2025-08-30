using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class HardNoteVM : NoteBaseVM
    {
        public ObservableCollection<CompositeBase> Composites { get; set; }

        public HardNoteVM()
        {
            Composites = new();

            Composites.Add(new HeaderComposite());
            Composites.Add(new TextComposite() { Text = "Проверка текста1"});
        }

        public TextComposite AddTextComposite(CompositeBase current, int caretIndex)
        {
            var newItem = new TextComposite { Text = string.Empty };
            if (current is HeaderComposite headerComposite)
            {
                int index = Composites.IndexOf(headerComposite);
                Composites.Insert(index + 1, newItem);
                return newItem;
            }
            else if (current is TextComposite textComposite)
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
                    var textAfter = textComposite.Text.Substring(caretIndex);
                    newItem.Text = textAfter;
                    textComposite.Text = textComposite.Text.Substring(0, caretIndex);
                    Composites.Insert(index + 1, newItem);
                    return newItem;
                }
            }
            return null;
        }
        public void DeleteTextComposite(TextComposite textComposite) => Composites.Remove(textComposite);
    }
}