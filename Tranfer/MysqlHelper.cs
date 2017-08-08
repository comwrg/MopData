using MySql.Data;
using MySql.Data.MySqlClient;

namespace Tranfer
{
    public class MysqlHelper
    {
        public MySqlConnection Conn { get; set; }
        public MysqlHelper()
        {
            string server = "rm-wz93r1ao4i4png9g8o.mysql.rds.aliyuncs.com";
            string database = "mopinfo";
            string user = "dev";
            string pwd = "123456aB";
            Conn = new MySqlConnection($"SERVER={server};DATABASE={database};UID={user};PASSWORD={pwd}; convert zero datetime=True;default command timeout=0");
            Conn.Open();
        }

    }
}