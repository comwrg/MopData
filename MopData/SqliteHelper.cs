using System;
using System.Data.SQLite;
using System.IO;

namespace MopData
{
    public class SqliteHelper
    {
        public SqliteHelper()
        {
            string filename = $"{DateTime.Now:yyyyMMddHHmmss}.sqlite";
            lock (this)
            {
                bool exist = File.Exists(filename);
                if (!exist)
                    SQLiteConnection.CreateFile(filename);
                Conn = new SQLiteConnection($"Data Source={filename}");
                Conn.Open();
                Cmd = new SQLiteCommand(Conn);
                Create();
            }
        }

        public SQLiteConnection Conn { get; set; }
        public SQLiteCommand Cmd { get; set; }

        public void Create()
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS MobileInfo(
                手机号 CHAR(11) PRIMARY KEY,
                姓名 CHAR,
                归属 CHAR,
                在用套餐 CHAR,
                用户状态 CHAR,
                开打时间 CHAR,
                出账类型 CHAR,
                集团信息 CHAR,
                最晚捆绑时间 CHAR,

                业务信息 CHAR, 

                一月消费 DOUBLE ,
                一月流量 DOUBLE ,
                二月消费 DOUBLE ,
                二月流量 DOUBLE ,
                三月消费 DOUBLE ,
                三月流量 DOUBLE ,
                四月消费 DOUBLE ,
                四月流量 DOUBLE ,
                五月消费 DOUBLE ,
                五月流量 DOUBLE ,
                六月消费 DOUBLE ,
                六月流量 DOUBLE ,
                七月消费 DOUBLE ,
                七月流量 DOUBLE ,
                八月消费 DOUBLE ,
                八月流量 DOUBLE ,
                九月消费 DOUBLE ,
                九月流量 DOUBLE ,
                十月消费 DOUBLE ,
                十月流量 DOUBLE ,
                十一月消费 DOUBLE ,
                十一月流量 DOUBLE ,
                十二月消费 DOUBLE ,
                十二月流量 DOUBLE ,
          
                推荐信息 VARCHAR 
            )";

            Cmd.CommandText = sql;
            Cmd.ExecuteNonQuery();
        }

        
        

    }
}