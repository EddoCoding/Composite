using Composite.Services;

namespace Composite.Common.Factories
{
    public interface IMediaPlayerFactory
    {
        IMediaPlayerService Create();
    }
}