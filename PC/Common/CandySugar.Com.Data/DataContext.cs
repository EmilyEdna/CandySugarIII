using FreeSql;
using System;
using System.IO;

namespace CandySugar.Com.Data
{
    public class DataContext
    {
        private static string ConnectionString
        {
            get
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBase");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var dbFile = Path.Combine(dir, "CandySugar.db3");
                if (!File.Exists(dbFile))
                    File.Create(dbFile).Dispose();
                return dbFile;
            }
        }
        public static IFreeSql Sqlite => new Lazy<IFreeSql>(() => new FreeSqlBuilder().UseConnectionString(DataType.Sqlite, $"DataSource={ConnectionString}").UseAutoSyncStructure(true).Build()).Value;
    }
}
