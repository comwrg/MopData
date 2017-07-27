using System.Data.SQLite;
namespace Tranfer
{
    public class SqliteHelper
    {
        public SQLiteConnection Conn { get; set; }

        public SqliteHelper(string path)
        {
            Conn = new SQLiteConnection($"Data Source={path}");
            Conn.Open();
        }
    }
}