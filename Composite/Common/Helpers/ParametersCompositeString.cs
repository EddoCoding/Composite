using Composite.ViewModels.Notes.HardNote;
using System.Windows;

namespace Composite.Common.Helpers
{
    public class ParametersCompositeString : Freezable
    {
        public static readonly DependencyProperty CompositeBaseVMProperty =
            DependencyProperty.Register(
                nameof(CompositeBaseVM),
                typeof(CompositeBaseVM),
                typeof(ParametersCompositeString),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(string),
                typeof(ParametersCompositeString),
                new PropertyMetadata(string.Empty));

        public CompositeBaseVM CompositeBaseVM
        {
            get => (CompositeBaseVM)GetValue(CompositeBaseVMProperty);
            set => SetValue(CompositeBaseVMProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        protected override Freezable CreateInstanceCore() => new ParametersCompositeString();
    }
}