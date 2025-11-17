using Composite.Services;

namespace Composite.Common.Factories
{
    public class MediaPlayerFactory(ISettingMediaPlayerService settingMediaPlayerService, ICommandService commandService) : IMediaPlayerFactory
    {
        public IMediaPlayerService Create() => new MediaPlayerService(settingMediaPlayerService, commandService);
    }
}