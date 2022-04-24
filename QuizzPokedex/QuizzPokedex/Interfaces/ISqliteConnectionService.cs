using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Interfaces
{
    public interface ISqliteConnectionService
    {
        SQLiteAsyncConnection GetAsyncConnection();
    }
}
