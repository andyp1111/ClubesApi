using Microsoft.Data.SqlClient;
using ClubesApi.Models;
using ClubesApi.DTOs;
using System.Data;

namespace TrabajoProyecto.Data
{
    public class Db
    {
        private readonly string _connectionString;

        public Db(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClubesDBConnection");
        }

        public List<Club> GetAllClubes()
        {
            var clubes = new List<Club>();
            var sql = "SELECT * FROM dbo.Club WHERE IsActive = 1";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clubes.Add(MapReaderToClub(reader));
                    }
                }
            }
            return clubes;
        }

        public Club? CreateClub(ClubCreateDto dto)
        {
            var sql = @"
                INSERT INTO dbo.Club (Nombre, CantidadSocios, CantidadTitulos, FechaFundacion, UbicacionEstadio, NombreEstadio, IsActive)
                VALUES (@Nombre, 0, 0, @FechaFundacion, @UbicacionEstadio, @NombreEstadio, 1);
                SELECT SCOPE_IDENTITY();
            ";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", dto.Nombre);
                command.Parameters.AddWithValue("@FechaFundacion", dto.FechaFundacion.Date);
                command.Parameters.AddWithValue("@UbicacionEstadio", (object)dto.UbicacionEstadio ?? DBNull.Value);
                command.Parameters.AddWithValue("@NombreEstadio", (object)dto.NombreEstadio ?? DBNull.Value);

                connection.Open();
                var newIdObject = command.ExecuteScalar();

                if (newIdObject != null && newIdObject != DBNull.Value)
                {
                    return GetClubById(Convert.ToInt32(newIdObject));
                }
                return null;
            }
        }

        public Club? GetClubById(int id)
        {
            var sql = "SELECT * FROM dbo.Club WHERE ClubId = @Id AND IsActive = 1";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToClub(reader);
                    }
                    return null;
                }
            }
        }

        public bool UpdateClub(Club club)
        {
            var sql = @"
                UPDATE dbo.Club 
                SET Nombre = @Nombre, 
                    CantidadSocios = @CantidadSocios, 
                    CantidadTitulos = @CantidadTitulos, 
                    FechaFundacion = @FechaFundacion, 
                    UbicacionEstadio = @UbicacionEstadio, 
                    NombreEstadio = @NombreEstadio,
                    IsActive = @IsActive
                WHERE ClubId = @ClubId;
            ";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClubId", club.ClubId);
                command.Parameters.AddWithValue("@Nombre", club.Nombre);
                command.Parameters.AddWithValue("@CantidadSocios", club.CantidadSocios);
                command.Parameters.AddWithValue("@CantidadTitulos", club.CantidadTitulos);
                command.Parameters.AddWithValue("@FechaFundacion", club.FechaFundacion.Date);
                command.Parameters.AddWithValue("@UbicacionEstadio", (object)club.UbicacionEstadio ?? DBNull.Value);
                command.Parameters.AddWithValue("@NombreEstadio", (object)club.NombreEstadio ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", club.IsActive);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeactivateClub(int id)
        {
            var sql = "UPDATE dbo.Club SET IsActive = 0 WHERE ClubId = @Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        private Club MapReaderToClub(SqlDataReader reader)
        {
            return new Club
            {
                ClubId = reader.GetInt32(reader.GetOrdinal("ClubId")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                CantidadSocios = reader.GetInt32(reader.GetOrdinal("CantidadSocios")),
                CantidadTitulos = reader.GetInt32(reader.GetOrdinal("CantidadTitulos")),
                FechaFundacion = reader.GetDateTime(reader.GetOrdinal("FechaFundacion")),
                UbicacionEstadio = reader.IsDBNull(reader.GetOrdinal("UbicacionEstadio")) ? null : reader.GetString(reader.GetOrdinal("UbicacionEstadio")),
                NombreEstadio = reader.IsDBNull(reader.GetOrdinal("NombreEstadio")) ? null : reader.GetString(reader.GetOrdinal("NombreEstadio")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}