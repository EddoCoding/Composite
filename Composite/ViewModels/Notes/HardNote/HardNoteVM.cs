using System.Collections.ObjectModel;
using System.Windows;
using Composite.Models.Notes.HardNote;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class HardNoteVM : NoteBaseVM
    {
        public ObservableCollection<CompositeBaseVM> Composites { get; set; }

        public HardNoteVM()
        {
            Id = Guid.NewGuid();
            Composites = new();

            Composites.Add(new TextCompositeVM());
        }

        public TextCompositeVM AddTextComposite(CompositeBaseVM current, int caretIndex)
        {
            var newItem = new TextCompositeVM { Text = string.Empty };
            if (current is TextCompositeVM textComposite)
            {
                int index = Composites.IndexOf(textComposite);
                //Если каретка в начале
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
                //Если каретка в конце
                else if (caretIndex == textComposite.Text.Length)
                {
                    Composites.Insert(index + 1, newItem);
                    return newItem;
                }
                //Если каретка между началом и концом
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
        public CompositeBaseVM CreateComposite(string value, CompositeBaseVM compositeBaseVM, int currentIndex)
        {
            string trimmedValue = value.Trim();
            switch (trimmedValue)
            {
                case "/Header":
                    int index = Composites.IndexOf(compositeBaseVM);
                    DeleteTextComposite(compositeBaseVM as TextCompositeVM);
                    var headerComposite = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 24
                    };
                    Composites.Insert(index, headerComposite);
                    return headerComposite;

                default: return null;
            }
        }
        public void DeleteTextComposite(TextCompositeVM textComposite) => Composites.Remove(textComposite);
    }
}