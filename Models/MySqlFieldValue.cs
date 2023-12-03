using MySqlConnector;

namespace Nexd.MySQL
{
    public class MySqlFieldValue : Dictionary<string, string?>
    {
        public MySqlFieldValue()
            { }

        public MySqlFieldValue Add(string column, string? value, bool escape = true)
        {
            if (value != null && escape)
            {
                value = MySqlHelper.EscapeString(value);
            }

            base.Add(column, value);
            return this;
        }
    }
}
