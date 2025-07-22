﻿using CommunityToolkit.Mvvm.ComponentModel;
using Composite.Services.TabService;

namespace Composite.ViewModels
{
    public partial class CompositeMainViewModel(ITabService tabService) : ObservableObject
    {
        [ObservableProperty] ITabService _tabService = tabService;
    }
}