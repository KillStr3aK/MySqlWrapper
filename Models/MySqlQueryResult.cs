namespace Source2Framework.MySQL
{
    public class MySqlQueryResult : Dictionary<int, MySqlFieldValue>
    {
        public int Rows
            => this.Count;

        public List<string> Columns
            => this[0].Keys.ToList<string>();

        public MySqlQueryResult()
            { }

        public MySqlQueryResult(Dictionary<int, MySqlFieldValue> result)
        {
            foreach (var item in result)
            {
                this[item.Key] = item.Value;
            }
        }

        public T Get<T>(int row, string column)
            => (T)Convert.ChangeType(this[row][column], typeof(T));

        public new void Add(int row, MySqlFieldValue value)
            => base.Add(row, value);
    }
}
