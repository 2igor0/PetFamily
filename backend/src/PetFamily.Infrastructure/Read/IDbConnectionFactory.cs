using System.Data.Common;

namespace PetFamily.Infrastructure.Read;

public interface IDbConnectionFactory
{
    DbConnection Create();
}
