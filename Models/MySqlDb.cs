namespace Nexd.MySQL
{
    using MySqlConnector;

    using System.Data;
    using System.Data.Common;

    public class MySqlDb
    {
        private string Query { get; set; } = string.Empty;

        private string TableName { get; set; } = string.Empty;

        private string WhereQuery { get; set; } = string.Empty;

        private readonly string ConnectionString = string.Empty;

        public MySqlDb(MySqlConnectionStringBuilder builder)
        {
            this.ConnectionString = builder.ConnectionString;
        }

        public MySqlDb(string input) : this(new MySqlConnectionStringBuilder(input))
            { }

        public MySqlDb(MySqlConfig config) : this(config.ToString())
            { }

        public MySqlDb(string hostname, string username, string password, string database, int port = 3306, string sslmode = "none")
            : this($"Server={hostname};Database={database};port={port};User Id={username};password={password};SslMode={sslmode};")
            { }

        public MySqlDb Table(string name)
        {
            this.TableName = name;
            return this;
        }

        public MySqlDb Where(MySqlQueryCondition condition)
        {
            this.WhereQuery = "WHERE " + string.Join(" AND ", condition.Select(x => string.Join(" OR ", x.Value.Select(y => "`" + x.Key + "` " + y.Key + " '" + y.Value + "'").ToArray())).ToArray());
            return this;
        }

        public MySqlDb Where(string condition)
        {
            this.WhereQuery = "WHERE " + condition;
            return this;
        }

        public void Insert(MySqlQueryValue data)
        {
            this.Query = "INSERT INTO `" + this.TableName + "`(" + string.Join(", ", data.Select(x => x.Key).ToArray()) + ") VALUES (" + string.Join(", ", data.Select(x => "'" + x.Value + "'").ToArray()) + ")";
            this.ExecuteQueryInternal(this.Query);
        }

        public async Task<int> InsertAsync(MySqlQueryValue data)
        {
            this.Query = "INSERT INTO `" + this.TableName + "`(" + string.Join(", ", data.Select(x => x.Key).ToArray()) + ") VALUES (" + string.Join(", ", data.Select(x => "'" + x.Value + "'").ToArray()) + ")";
            return await this.ExecuteNonQueryInternalAsync(this.Query);
        }

        public void InsertIfNotExist(MySqlQueryValue data, string onDuplicateKey = "")
        {
            this.Query = "INSERT INTO `" + this.TableName + "`(" + string.Join(", ", data.Select(x => x.Key).ToArray()) + ") VALUES (" + string.Join(", ", data.Select(x => "'" + x.Value + "'").ToArray()) + ") ON DUPLICATE KEY UPDATE " + onDuplicateKey;
            this.ExecuteQueryInternal(this.Query);
        }

        public async Task<int> InsertIfNotExistAsync(MySqlQueryValue data, string onDuplicateKey = "")
        {
            this.Query = "INSERT INTO `" + this.TableName + "`(" + string.Join(", ", data.Select(x => x.Key).ToArray()) + ") VALUES (" + string.Join(", ", data.Select(x => "'" + x.Value + "'").ToArray()) + ") ON DUPLICATE KEY UPDATE " + onDuplicateKey;
            return await this.ExecuteNonQueryInternalAsync(this.Query);
        }

        public MySqlQueryResult Select(string fields = "*", int rowLimit = 0)
        {
            string limit = rowLimit > 0 ? " LIMIT " + rowLimit : "";
            this.Query = "SELECT " + fields + " FROM `" + this.TableName + "` " + this.WhereQuery + limit;
            return this.ExecuteQueryInternal(this.Query);
        }

        public async Task<MySqlQueryResult> SelectAsync(string fields = "*", int rowLimit = 0)
        {
            string limit = rowLimit > 0 ? " LIMIT " + rowLimit : "";
            this.Query = "SELECT " + fields + " FROM `" + this.TableName + "` " + this.WhereQuery + limit;
            return await this.ExecuteQueryInternalAsync(this.Query);
        }

        public int Update(MySqlQueryValue data)
        {
            this.Query = "UPDATE `" + this.TableName + "` SET " + string.Join(", ", data.Select(x => "`" + x.Key + "` = '" + x.Value + "'").ToArray()) + " " + this.WhereQuery;
            return this.ExecuteNonQueryInternal(this.Query);
        }

        public async Task<int> UpdateAsync(MySqlQueryValue data)
        {
            this.Query = "UPDATE `" + this.TableName + "` SET " + string.Join(", ", data.Select(x => "`" + x.Key + "` = '" + x.Value + "'").ToArray()) + " " + this.WhereQuery;
            return await this.ExecuteNonQueryInternalAsync(this.Query);
        }

        public int Delete()
        {
            this.Query = "DELETE FROM `" + this.TableName + "` " + this.WhereQuery;
            return this.ExecuteNonQueryInternal(this.Query);
        }

        public async Task<int> DeleteAsync()
        {
            this.Query = "DELETE FROM `" + this.TableName + "` " + this.WhereQuery;
            return await this.ExecuteNonQueryInternalAsync(this.Query);
        }

        public int ExecuteNonQuery(string statement)
        {
            return this.ExecuteNonQueryInternal(statement);
        }

        public async Task<int> ExecuteNonQueryAsync(string statement)
        {
            return await this.ExecuteNonQueryInternalAsync(statement);
        }

        public MySqlQueryResult ExecuteQuery(string statement)
        {
            return this.ExecuteQueryInternal(statement);
        }

        public async Task<MySqlQueryResult> ExecuteQueryAsync(string statement)
        {
            return await this.ExecuteQueryInternalAsync(statement);
        }

        private void ResetQuery()
            => this.Query = this.WhereQuery = this.TableName = string.Empty;

        private T CreateContext<T>(Func<MySqlConnection, T> context)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                return context.Invoke(connection);
            }
        }

        private async Task<T> CreateContextAsync<T>(Func<MySqlConnection, Task<T>> asyncContext)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                return await asyncContext.Invoke(connection);
            }
        }

        private MySqlQueryResult ExecuteQueryInternal(string query)
        {
            return this.CreateContext<MySqlQueryResult>((connection) =>
            {
                MySqlQueryResult result = new MySqlQueryResult();

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int rowCol = 0;

                            while (reader.Read())
                            {
                                MySqlFieldValue value = new MySqlFieldValue();

                                for (int col = 0; col < reader.FieldCount; col++)
                                {
                                    value.Add(reader.GetName(col).ToString(), reader.GetValue(col).ToString());
                                }

                                result.Add(rowCol, value);
                                rowCol++;
                            }
                        }
                    }
                }

                this.ResetQuery();
                return result;
            });
        }

        private async Task<MySqlQueryResult> ExecuteQueryInternalAsync(string query)
        {
            return await this.CreateContextAsync<MySqlQueryResult>(async (connection) =>
            {
                MySqlQueryResult result = new MySqlQueryResult();

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            int rowCol = 0;

                            while (reader.Read())
                            {
                                MySqlFieldValue value = new MySqlFieldValue();

                                for (int col = 0; col < reader.FieldCount; col++)
                                {
                                    value.Add(reader.GetName(col).ToString(), reader.GetValue(col).ToString());
                                }

                                result.Add(rowCol, value);
                                rowCol++;
                            }
                        }
                    }
                }

                this.ResetQuery();
                return await Task.FromResult<MySqlQueryResult>(result);
            });
        }

        private int ExecuteNonQueryInternal(string statement)
        {
            return this.CreateContext<int>((connection) =>
            {
                int rowsAffected = 0;

                using (MySqlCommand cmd = new MySqlCommand(statement, connection))
                {
                    rowsAffected = cmd.ExecuteNonQuery();
                }

                this.ResetQuery();
                return rowsAffected;
            });
        }

        private async Task<int> ExecuteNonQueryInternalAsync(string statement)
        {
            return await this.CreateContextAsync<int>(async (connection) =>
            {
                int rowsAffected = 0;

                using (MySqlCommand cmd = new MySqlCommand(statement, connection))
                {
                    rowsAffected = await cmd.ExecuteNonQueryAsync();
                }

                this.ResetQuery();
                return await Task.FromResult<int>(rowsAffected);
            });
        }
    }
}
