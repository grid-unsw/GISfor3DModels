using Npgsql;
using UnityEditor;
using UnityEngine.PlayerLoop;

public static class DbCommonFunctions
{
    public static string GetNpgsqlConnectionString()
    {
        var voxelEditor = (DBEditor)EditorWindow.GetWindow(typeof(DBEditor));

        var connectionString = voxelEditor.GetConnectionString();
        voxelEditor.Close();

        return connectionString;
    }

    public static NpgsqlConnection GetNpgsqlConnection()
    {
        var voxelEditor = (DBEditor)EditorWindow.GetWindow(typeof(DBEditor));

        var connectionString = voxelEditor.GetConnectionString();
        var connection = new NpgsqlConnection(connectionString);
        voxelEditor.Close();

        return connection;
    }

    private static void CreateTable(string tableName, NpgsqlConnection connection, string fields)
    {
        var sql = $"CREATE TABLE {tableName} {fields};";
        var cmd = EstablishConnectionWithQuery(connection, sql);
        cmd.ExecuteNonQuery();
    }

    public static bool CheckIfTableExist(string tableName, NpgsqlConnection connection)
    {
        var sql = $"SELECT EXISTS ( SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = '{tableName}' );";
        var cmd = EstablishConnectionWithQuery(connection, sql);
        return (bool)cmd.ExecuteScalar();
    }

    private static void TruncateTable(string tableName, NpgsqlConnection connection)
    {
        var sql = $"TRUNCATE TABLE {tableName};";
        var cmd = EstablishConnectionWithQuery(connection, sql);
        cmd.ExecuteNonQuery();
    }

    public static void CreateTableIfNotExistOrTruncate(string tableName, NpgsqlConnection connection, string fields, bool truncate = false)
    {
        tableName = tableName.ToLower();

        if (!CheckIfTableExist(tableName, connection))
        {
            CreateTable(tableName, connection, fields);
        }
        else
        {
            if (truncate)
            {
                TruncateTable(tableName, connection);
            }
        }
    }

    public static NpgsqlCommand EstablishConnectionWithQuery(NpgsqlConnection connection, string sql)
    {
        var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = sql;
        return cmd;
    }
}
