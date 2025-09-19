using Composite.Services;

namespace Composite.Common.Factories
{
    public class MediaPlayerFactory(ISettingMediaPlayerService settingMediaPlayerService) : IMediaPlayerFactory
    {
        public IMediaPlayerService Create() => new MediaPlayerService(settingMediaPlayerService);
    }
}