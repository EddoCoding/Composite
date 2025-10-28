﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class CodeCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] string _text = string.Empty;

        public CodeCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new CodeCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text };
    }
}