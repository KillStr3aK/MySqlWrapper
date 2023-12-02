# C# Sync & Async MySQL (MySqlConnector library) Wrapper

# Installation

Available on [NuGet](https://www.nuget.org/packages/Nexd.MySQL/)
[![NuGet version (Nexd.Reflection)](https://img.shields.io/nuget/v/Nexd.MySQL.svg?style=flat-square)](https://www.nuget.org/packages/Nexd.MySQL/)

```
dotnet add package Nexd.MySQL --version 1.0.1
```

### Initialization
```c#
// import using
using Nexd.MySQL;

MySqlDb MySql = new MySqlDb("localhost", "root", "password", "database");
```

### ExecuteQuery & ExecuteNonQuery usage (unsafe)

These functions are not safe in the context of SQL injection. Make sure to escape your values correctly before executing. (MySqlHelper.EscapeString(value);)

```c#
MySql.ExecuteQuery($"SELECT * FROM `Players` WHERE Name = '{name}';", name); make sure that the name is escaped.

MySql.ExecuteNonQuery("DELETE * FROM `Players`;");
```

### Where condition
```c#
// WITH WRAPPER CLASSES
MySqlQueryCondition conditions = new MySqlQueryCondition()
    .Add("ID", ">", "1002")
    .Add("ID", "<=", "1008");

MySqlQueryResult result = MySql.Table("players").Where(conditions).Select();
for(int i = 0; i < result.Rows; i++)
{
    Console.WriteLine($"{result.Get<int>(i, "ID")} {result.Get<string>(i, "Name")} {result.Get<string>(i, "Identifier")}");
}

// WITHOUT
MySqlQueryResult result = MySql.Table("players").Where("ID > 1002 AND ID <= 1008").Select();
for(int i = 0; i < result.Rows; i++)
{
    Console.WriteLine($"{result.Get<int>(i, "ID")} {result.Get<string>(i, "Name")} {result.Get<string>(i, "Identifier")}");
}
```

### Insert Query
```c#
MySqlQueryValue values = new MySqlQueryValue()
    .Add("Name", "Player Name #1") // DB column is 'Name', insert data 'Player Name #1'
    .Add("Identifier", "uniqueidentifier"); // DB column is 'Identifier', insert data 'uniqueidentifier'

// assume there is a table named 'players'
MySql.Table("players").Insert(values);
```

### Update Query
```c#
MySqlQueryValue values = new MySqlQueryValue()
    .Add("Name", "Player Name #2");

// with wrapper classes
int rowsAffected = MySql.Table("players").Where(MySqlQueryCondition.New("Identifier", "=", "uniqueidentifier")).Update(values);

// without
int rowsAffected = MySql.Table("players").Where("Identifier = 'uniqueidentifier'").Update(values);
int rowsAffected = MySql.Table("players").Where("Identifier = 'uniqueidentifier' AND ID > 0").Update(values); // random example that shows you can define your where statement just as you like
```

### Select Query
```c#
// with wrapper classes
MySqlQueryResult result = MySql.Table("players").Where(MySqlQueryCondition.New("Identifier", "=", "uniqueidentifier")).Select();

// without
MySqlQueryResult result = MySql.Table("players").Where("Identifier = 'uniqueidentifier'").Select(); // you can specify the fields (columns) you want to select, also LIMIT the amount of results by passing them to the .Select() as parameters

// then
int playerId = result.Get<int>(0, "ID"); // 0 is the row, "ID" is the column name in the results
DateTime registeredAt = result.Get<DateTime>(0, "CreationDate"); // 0 is the row, "CreationDate" is the column name in the results
```

### Delete Query
```c#
// with wrapper classes
int rowsAffected = MySql.Table("players").Where(MySqlQueryCondition.New("Identifier", "=", "uniqueidentifier")).Delete();

// without
int rowsAffected = MySql.Table("players").Where("Identifier = 'uniqueidentifier'").Delete();
```

### Async
Note: that every method has its `Async` version implemented

Documentation is currently unavailable, but its self explanatory and has the same syntax with the sync version, just with `Async` method names.

### TODO / Missing
- Joins
- Subqueries
- Group by?
- Order by?
