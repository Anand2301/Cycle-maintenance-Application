using System.Data;

namespace Cycle_App.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
