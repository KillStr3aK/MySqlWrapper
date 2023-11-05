namespace Nexd.MySQL
{
    public class MySqlQueryValue : MySqlFieldValue
    {
        public MySqlQueryValue()
            { }

        public MySqlQueryValue(string column, string value)
            => this.Add(column, value);

        public new MySqlQueryValue Add(string column, string value)
        {
            base.Add(column, value);
            return this;
        }
    }
}
