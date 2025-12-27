using System.Data;

namespace Composite.DataBase
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}