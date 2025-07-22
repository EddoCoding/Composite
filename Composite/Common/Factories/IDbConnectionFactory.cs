using System.Data;

namespace Composite.Common.Factories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}