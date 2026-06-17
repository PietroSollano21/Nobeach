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
    {
        return new MySqlConnection(_connectionString);
    }
}