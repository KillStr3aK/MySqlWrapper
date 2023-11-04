namespace Source2Framework.MySQL
{
    public class MySqlConfig
    {
        public string Hostname { get; set; } = "localhost";

        public string Database { get; set; } = string.Empty;

        public string Username { get; set; } = "root";

        public string Password { get; set; } = "";

        public int Port { get; set; } = 3306;

        public string SSLMode { get; set; } = "none";

        public string AllowPublicKeyRetrieval { get; set; } = "True";

        public MySqlConfig()
            { }

        public MySqlConfig(string hostname, string database, string username, string password)
        {
            this.Hostname = hostname;
            this.Database = database;
            this.Username = username;
            this.Password = password;
        }

        public MySqlConfig(string hostname, string database, string username, string password, int port) : this(hostname, database, username, password)
        {
            this.Port = port;
        }

        public MySqlConfig(string hostname, string database, string username, string password, int port, string sslMode) : this(hostname, database, username, password, port)
        {
            this.SSLMode = sslMode;
        }

        public MySqlConfig(string hostname, string database, string username, string password, int port, string sslMode, string allowPublicKeyRetrieval) : this(hostname, database, username, password, port, sslMode)
        {
            this.AllowPublicKeyRetrieval = allowPublicKeyRetrieval;
        }

        public override string ToString()
            => $"Server={this.Hostname};Database={this.Database};port={this.Port};User Id={this.Username};password={this.Password};SslMode={this.SSLMode};AllowPublicKeyRetrieval={this.AllowPublicKeyRetrieval}";
    }
}
