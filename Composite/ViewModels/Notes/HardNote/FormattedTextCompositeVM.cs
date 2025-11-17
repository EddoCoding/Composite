using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class FormattedTextCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] FlowDocument _document;
        [ObservableProperty] bool _isModified;
        [ObservableProperty] byte[] _xamlPackageContent;

        [ObservableProperty] double _brSize;
        [ObservableProperty] double _brCornerRadius;
        [ObservableProperty] string _brColor = "WhiteSmoke";
        [ObservableProperty] string _bgColor = "WhiteSmoke";

        double _selectedBrSize;
        public double SelectedBrSize
        {
            get => _selectedBrSize;
            set
            {
                SetProperty(ref _selectedBrSize, value);
                ChangeBorderSize(SelectedBrSize);
            }
        }

        double _selectedBrCornerRadius;
        public double SelectedBrCornerRadius
        {
            get => _selectedBrCornerRadius;
            set
            {
                SetProperty(ref _selectedBrCornerRadius, value);
                ChangeBrCornerRadius(SelectedBrCornerRadius);
            }
        }

        string _selectedBrColor = "WhiteSmoke";
        public string SelectedBrColor
        {
            get => _selectedBrColor;
            set
            {
                SetProperty(ref _selectedBrColor, value);
                ChangeBrColor(SelectedBrColor);
            }
        }

        string _selectedBgColor = "WhiteSmoke";
        public string SelectedBgColor
        {
            get => _selectedBgColor;
            set
            {
                SetProperty(ref _selectedBgColor, value);
                ChangeBgColor(SelectedBgColor);
            }
        }

        public List<string> Fonts { get; } //Выборка шрифтов
        public double[] FontSizes { get; set; } //Выборка размера шрифта
        public double[] Borders { get; set; } //Выборка размера рамки
        public double[] Corners { get; set; } //Выборка угла рамки
        public string[] Colors { get; set; } //Выборка цветов для рамки, заднего фона и цвета текса

        public FormattedTextCompositeVM()
        {
            Id = Guid.NewGuid();
            Document = new FlowDocument();
            Fonts = new(System.Windows.Media.Fonts.SystemFontFamilies.Select(x => x.Source).OrderBy(x => x));
            FontSizes = new[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0, 29.0, 30.0 };
            Borders = new[] { 0.0, 0.5, 1.0, 1.5, 2, 2.5, 3.0 };
            Corners = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };

            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
            Colors = colorProperties.Select(prop => prop.Name).OrderBy(name => name).ToArray();
        }

        partial void OnDocumentChanged(FlowDocument value)
        {
            ConvertToXamlPackage();
            IsModified = true;
        }
        public void OnTextChanged(FlowDocument document)
        {
            _document = document;
            ConvertToXamlPackage();
            IsModified = true;
        }
        void ConvertToXamlPackage()
        {
            if (Document == null) return;

            try
            {
                TextRange range = new TextRange(Document.ContentStart, Document.ContentEnd);
                using MemoryStream stream = new MemoryStream();
                range.Save(stream, DataFormats.XamlPackage);
                XamlPackageContent = stream.ToArray();
            }
            catch (Exception ex) { }
        }
        void LoadFromXamlPackage(byte[] package)
        {
            if (package == null || package.Length == 0)
            {
                Document = new FlowDocument(new Paragraph());
                return;
            }

            try
            {
                FlowDocument doc = new FlowDocument();
                TextRange range = new TextRange(doc.ContentStart, doc.ContentEnd);
                using MemoryStream stream = new MemoryStream(package);
                range.Load(stream, DataFormats.XamlPackage);

                Document = doc;
                XamlPackageContent = package;
                IsModified = false;
            }
            catch (Exception ex) { }
        }

        void ChangeBorderSize(double borderSize) => BrSize = borderSize;
        void ChangeBrCornerRadius(double brCornerRadius) => BrCornerRadius = brCornerRadius;
        void ChangeBrColor(string brColor) => BrColor = brColor;
        void ChangeBgColor(string bgColor) => BgColor = bgColor;


        public override object Clone()
        {
            var clone = new FormattedTextCompositeVM()
            {
                Id = Guid.NewGuid(),
                Tag = Tag,
                Comment = Comment,
                IsModified = IsModified,
                SelectedBrSize = SelectedBrSize,
                SelectedBrCornerRadius = SelectedBrCornerRadius,
                SelectedBrColor = SelectedBrColor,
                SelectedBgColor = SelectedBgColor
            };

            if (!XamlPackageContent.Any()) clone.LoadFromXamlPackage(XamlPackageContent);
            else if (Document != null)
            {
                ConvertToXamlPackage();
                clone.LoadFromXamlPackage(XamlPackageContent);
            }

            return clone;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Document.Blocks.Clear();
                _xamlPackageContent = Array.Empty<byte>();
                IsModified = false;
                Fonts.Clear();
                FontSizes = Array.Empty<double>();
                Borders = Array.Empty<double>();
                Corners = Array.Empty<double>();
                Colors = Array.Empty<string>();
            }
            base.Dispose(disposing);
        }
    }
}