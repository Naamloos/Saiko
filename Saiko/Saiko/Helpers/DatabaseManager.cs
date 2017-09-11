using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace Saiko.Helpers
{
    public class DatabaseManager
    {
        string Host;
        int Port;
        string Database;
        string User;
        string Password;
        string ConnectionString;
        NpgsqlConnection Connection;

        public DatabaseManager (string host, int port, string database, string username, string password)
        {
            Host = host;
            Port = port;
            Database = database;
            User = username;
            Password = password;

            var csb = new NpgsqlConnectionStringBuilder()
            {
                Host = host,
                Port = port,
                Database = database,
                Username = User,
                Password = password,
                Pooling = true,

                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            ConnectionString = csb.ConnectionString;
            Connection = new NpgsqlConnection(ConnectionString);
            Initialize();
        }

        void Initialize()
        {
            Connection.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = "CREATE SEQUENCE IF NOT EXISTS tags_id_seq;" +
                    "\nCREATE TABLE IF NOT EXISTS TAGS(" +
                    "\nID BIGINT PRIMARY KEY DEFAULT NEXTVAL('tags_id_seq')," +
                    "\nNAME TEXT UNIQUE," +
                    "\nCONTENTS TEXT," +
                    "\nATTACHMENT TEXT," +
                    "\nOWNERID BIGINT" +
                    "\n);" +
                    "\nALTER SEQUENCE tags_id_seq OWNED BY TAGS.ID;";
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        public async Task DeleteTag(string name, ulong ownerid)
        {
            await Connection.OpenAsync();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = "DELETE FROM TAGS WHERE OWNERID=@owner AND NAME=@name";
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, name);
                cmd.Parameters.AddWithValue("owner", NpgsqlDbType.Bigint, (long)ownerid);
                cmd.Prepare();
                await cmd.ExecuteNonQueryAsync();
                cmd.Dispose();
            }
        }

        public async Task<List<SaikoTag>> GetTags()
        {
            await Connection.OpenAsync();
            List<SaikoTag> results = new List<SaikoTag>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = $"SELECT * FROM TAGS ORDER BY ID ASC;";
                cmd.Prepare();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var t = new SaikoTag();
                        t.ID = reader.GetInt64(0);
                        t.Name = reader.GetString(1);
                        t.Contents = reader.GetString(2);
                        t.Attachment = reader.GetString(3);
                        t.Owner = (ulong)reader.GetInt64(4);
                        results.Add(t);
                    }
                    reader.Close();
                }
                cmd.Dispose();
            }
            return results;
        }


        public async Task SetTag(SaikoTag t)
        {
            await Connection.OpenAsync();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = "INSERT INTO TAGS(NAME, CONTENTS, ATTACHMENT, OWNERID) VALUES(@name, @contents, @attachment, @owner) ON CONFLICT(NAME) DO UPDATE SET CONTENTS=EXCLUDED.CONTENTS, ATTACHMENT=EXCLUDED.ATTACHMENT WHERE TAGS.OWNERID=EXCLUDED.OWNERID AND TAGS.NAME=EXCLUDED.NAME;";
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, t.Name);
                cmd.Parameters.AddWithValue("contents", NpgsqlDbType.Text, t.Contents);
                cmd.Parameters.AddWithValue("attachment", NpgsqlDbType.Text, t.Attachment);
                cmd.Parameters.AddWithValue("owner", NpgsqlDbType.Bigint, (long)t.Owner);
                cmd.Prepare();
                await cmd.ExecuteNonQueryAsync();
                cmd.Dispose();
            }
        }

        public async Task<SaikoTag> GetTag(string name)
        {
            await Connection.OpenAsync();
            var t = new SaikoTag();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = Connection;
                cmd.CommandText = $"SELECT * FROM TAGS WHERE NAME = @name;";
                cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, name);
                cmd.Prepare();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        t.ID = reader.GetInt64(0);
                        t.Name = reader.GetString(1);
                        t.Contents = reader.GetString(2);
                        t.Attachment = reader.GetString(3);
                        t.Owner = (ulong)reader.GetInt64(4);
                    }
                    reader.Close();
                }
                cmd.Dispose();
            }
            return t;
        }
    }
}
