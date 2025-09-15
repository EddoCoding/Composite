using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

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

        public void AddTextCompositeVM() => Composites.Add(new TextCompositeVM());
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
            if (current is HeaderCompositeVM headerComposite)
            {
                int index = Composites.IndexOf(headerComposite);
                Composites.Insert(index + 1, newItem);
                return newItem;
            }
            if (current is QuoteCompositeVM quoteComposite)
            {
                int index = Composites.IndexOf(quoteComposite);
                Composites.Insert(index + 1, newItem);
                return newItem;
            }

            return null;
        }
        public CompositeBaseVM CreateComposite(string value, CompositeBaseVM compositeBaseVM, int currentIndex)
        {
            string Value = value.Trim().ToLower();
            switch (Value)
            {
                case "/header":
                    int index = Composites.IndexOf(compositeBaseVM);
                    DeleteTextComposite(compositeBaseVM as TextCompositeVM);
                    var headerComposite = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 24
                    };
                    Composites.Insert(index, headerComposite);
                    return headerComposite;
                case "/header1":
                    int index1 = Composites.IndexOf(compositeBaseVM);
                    DeleteTextComposite(compositeBaseVM as TextCompositeVM);
                    var headerComposite1 = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 24
                    };
                    Composites.Insert(index1, headerComposite1);
                    return headerComposite1;
                case "/header2":
                    int index2 = Composites.IndexOf(compositeBaseVM);
                    DeleteTextComposite(compositeBaseVM as TextCompositeVM);
                    var headerComposite2 = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 22
                    };
                    Composites.Insert(index2, headerComposite2);
                    return headerComposite2;
                case "/header3":
                    int index3 = Composites.IndexOf(compositeBaseVM);
                    DeleteTextComposite(compositeBaseVM as TextCompositeVM);
                    var headerComposite3 = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 20
                    };
                    Composites.Insert(index3, headerComposite3);
                    return headerComposite3;
                case "/quote":
                    int indexQuote = Composites.IndexOf(compositeBaseVM);
                    DeleteTextComposite(compositeBaseVM as TextCompositeVM);
                    var quoteComposite = new QuoteCompositeVM();
                    Composites.Insert(indexQuote, quoteComposite);
                    return quoteComposite;
                case "/line":
                    int indexLine = Composites.IndexOf(compositeBaseVM);
                    DeleteTextComposite(compositeBaseVM as TextCompositeVM);
                    var lineComposite = new LineCompositeVM();
                    Composites.Insert(indexLine, lineComposite);
                    var textComposite = new TextCompositeVM();
                    Composites.Insert(indexLine + 1, textComposite);
                    return textComposite;

                default: return null;
            }
        }


        public void DeleteTextComposite(TextCompositeVM textComposite) => Composites.Remove(textComposite);

        public void DeleteHeaderComposite(CompositeBaseVM headerComposite) => Composites.Remove(headerComposite);

        public void DeleteQuoteComposite(CompositeBaseVM quoteComposite) => Composites.Remove(quoteComposite);

        [RelayCommand] void DeleteLineComposite(CompositeBaseVM composite) => Composites.Remove(composite);
    }
}