using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

public class Conexao
{
    private readonly string _connectionString;

    public Conexao(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public MySqlConnection GetConnection()
    {try
{
    using var conn = new MySqlConnection(_connectionString);
    conn.Open();
    Console.WriteLine("Conectou!");
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
        Console.WriteLine(_connectionString);
    return new MySqlConnection(_connectionString);
        ///return new MySqlConnection(_connectionString);
    }
}