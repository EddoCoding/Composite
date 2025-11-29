using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class SongListCompositeVM : CompositeBaseVM, IDisposable
    {
        public string Text { get; set; } = string.Empty;

        public ObservableCollection<SongMiniCompositeVM> Songs { get; set; } = new();

        public SongListCompositeVM() => Id = Guid.NewGuid();

        [RelayCommand] void AddSongComposite() => Songs.Add(new SongMiniCompositeVM());
        [RelayCommand] void DeleteSongComposite(SongMiniCompositeVM song)
        {
            song.Dispose();
            Songs.Remove(song);
        }


        public override object Clone()
        {
            var songList = new SongListCompositeVM()
            {
                Tag = Tag,
                Comment = Comment,
                Text = Text
            };
            
            foreach (var song in Songs) songList.Songs.Add((SongMiniCompositeVM)song.Clone());
            
            return songList;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Text = string.Empty;
                foreach (var songVM in Songs) songVM.Dispose();
                Songs.Clear();
                Songs = null;
            }
            base.Dispose(disposing);
        }
    }
}