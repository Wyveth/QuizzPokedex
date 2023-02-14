using SQLite;

namespace QuizzPokedex.Interfaces
{
    public interface ISqliteConnectionService
    {
        SQLiteAsyncConnection GetAsyncConnection();
    }
}
