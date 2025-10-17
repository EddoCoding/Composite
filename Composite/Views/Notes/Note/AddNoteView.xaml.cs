using System.Windows.Controls;

namespace Composite.Views.Notes
{
    public partial class AddNoteView : UserControl
    {
        public AddNoteView()
        {
            InitializeComponent();
            Loaded += (s, e) => titleTextBox.Focus();
        }
    }
}
