using Microsoft.Data.SqlClient;
using System.Data;

namespace YurtOtomasyon.Data;

public class Db
{
    private readonly string _connectionString;

    public Db(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public List<T> Query<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] parameters)
    {
        var list = new List<T>();
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(map(reader));
        }
        return list;
    }

    public void Execute(string sql, params SqlParameter[] parameters)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Procedure(string name, params SqlParameter[] parameters)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(name, connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddRange(parameters);
        connection.Open();
        command.ExecuteNonQuery();
    }
}
