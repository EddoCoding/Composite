using Composite.Models;
using Composite.ViewModels;

namespace Composite.Common.Mappers
{
    public class SongMap : ISongMap
    {
        public Song MapToModel(SongVM songVM) => new Song()
        {
            Id = songVM.Id.ToString(),
            Title = songVM.Title,
            Data = songVM.Data
        };
        public SongVM MapToViewModel(Song song) => new SongVM()
        {
            Id = Guid.Parse(song.Id),
            Title = song.Title,
            Data = song.Data
        };
        public SongVM MapToViewModelWithoutData(Song song) => new SongVM()
        {
            Id = Guid.Parse(song.Id),
            Title = song.Title
        };
    }
}