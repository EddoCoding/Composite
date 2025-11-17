using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Windows.Media;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class LineCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] double _lineSize = 1.0;
        [ObservableProperty] string _lineColor = "LightGray";

        double _selectedLineSize = 1.0;
        public double SelectedLineSize
        {
            get => _selectedLineSize;
            set
            {
                SetProperty(ref _selectedLineSize, value);
                ChangeLineSize(SelectedLineSize);
            }
        }

        string _selectedLineColor = "LightGray";
        public string SelectedLineColor
        {
            get => _selectedLineColor;
            set
            {
                SetProperty(ref _selectedLineColor, value);
                ChangeLineColor(SelectedLineColor);
            }
        }

        public double[] Lines { get; set; }
        public string[] Colors { get; set; }

        public LineCompositeVM()
        {
            Id = Guid.NewGuid();

            Lines = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };

            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
            Colors = colorProperties.Select(prop => prop.Name).OrderBy(name => name).ToArray();
        }

        void ChangeLineSize(double lineSize) => LineSize = lineSize;
        void ChangeLineColor(string lineColor) => LineColor = lineColor;


        public override object Clone() => new LineCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment };
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Lines = Array.Empty<double>();
                Colors = Array.Empty<string>();
            }
            base.Dispose(disposing);
        }
    }
}