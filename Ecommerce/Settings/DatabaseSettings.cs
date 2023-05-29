namespace Ecommerce.Settings;

public class DatabaseSettings
{
    public string UsedDatabase { get; set; } = default!;
    public Mongo Mongo { get; set; } = default!;
    public Cassandra Cassandra { get; set; } = default!;
    public Postgres Postgres { get; set; } = default!;
}

public class Mongo
{
    public string Connection { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
}

public class Cassandra
{
    public string ContactPoint { get; set; } = default!;
    public string Keyspace { get; set; } = default!;
}

public class Postgres
{
    public string Connection { get; set; } = default!;
}