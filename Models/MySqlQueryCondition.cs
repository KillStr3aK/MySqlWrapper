namespace Source2Framework.MySQL
{
    public class MySqlQueryCondition : Dictionary<string, MySqlFieldValue>
    {
        public MySqlQueryCondition()
            { }

        public MySqlQueryCondition(string column, string expression, string value)
            => this.Add(column, expression, value);

        public MySqlQueryCondition(MySqlQueryCondition condition)
        {
            foreach (var item in condition)
            {
                this[item.Key] = item.Value;
            }
        }

        public MySqlQueryCondition Add(string column, string expression, string value)
        {
            if (!this.ContainsKey(column))
            {
                this.Add(column, new MySqlFieldValue { [expression] = value });
            }
            else
            {
                if (!this[column].ContainsKey(expression))
                {
                    this[column].Add(expression, value);
                }
                else
                {
                    throw new ArgumentException($"A condition with the same expression has already been added. Condition: {column} {expression} {value}");
                }
            }

            return this;
        }

        public static MySqlQueryCondition New(string column, string expression, string value)
            => new MySqlQueryCondition(column, expression, value);
    }
}
