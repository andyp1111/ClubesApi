using Microsoft.Data.SqlClient;
using ClubesApi.Models;
using ClubesApi.DTOs;
using System.Collections.Generic;
using System;
using System.Data;

namespace ClubesApi.Repositories
{
    public class DirigenteRepository
    {
        private readonly string _connectionString;

        public DirigenteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool DniExists(int dni)
        {
            var sql = "SELECT COUNT(*) FROM dbo.Dirigente WHERE Dni = @Dni";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Dni", dni);
                connection.Open();

                var count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public List<Dirigente> GetAll()
        {
            var dirigentes = new List<Dirigente>();
            var sql = "SELECT DirigenteId, ClubId, Nombre, Apellido, FechaNacimiento, Rol, Dni FROM dbo.Dirigente";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dirigentes.Add(MapReaderToDirigente(reader));
                    }
                }
            }
            return dirigentes;
        }

        public Dirigente? GetById(int id)
        {
            var sql = "SELECT DirigenteId, ClubId, Nombre, Apellido, FechaNacimiento, Rol, Dni FROM dbo.Dirigente WHERE DirigenteId = @Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapReaderToDirigente(reader) : null;
                }
            }
        }
        public Dirigente? Create(DirigenteCreateDto dto)
        {
            var sql = @"
                INSERT INTO dbo.Dirigente (ClubId, Nombre, Apellido, FechaNacimiento, Rol, Dni)
                VALUES (@ClubId, @Nombre, @Apellido, @FechaNacimiento, @Rol, @Dni);
                SELECT SCOPE_IDENTITY();
            ";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClubId", dto.ClubId);
                command.Parameters.AddWithValue("@Nombre", dto.Nombre);
                command.Parameters.AddWithValue("@Apellido", dto.Apellido);
                command.Parameters.AddWithValue("@FechaNacimiento", dto.FechaNacimiento.Date);
                command.Parameters.AddWithValue("@Rol", dto.Rol);
                command.Parameters.AddWithValue("@Dni", dto.Dni);

                connection.Open();
                var newIdObject = command.ExecuteScalar();
                if (newIdObject != null && newIdObject != DBNull.Value)
                {
                    return GetById(Convert.ToInt32(newIdObject));
                }
                return null;
            }
        }

        public bool Update(Dirigente dirigente)
        {
            var sql = @"
                UPDATE dbo.Dirigente 
                SET ClubId = @ClubId, 
                    Nombre = @Nombre, 
                    Apellido = @Apellido, 
                    FechaNacimiento = @FechaNacimiento, 
                    Rol = @Rol, 
                    Dni = @Dni
                WHERE DirigenteId = @Id;
            ";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", dirigente.DirigenteId);
                command.Parameters.AddWithValue("@ClubId", dirigente.ClubId);
                command.Parameters.AddWithValue("@Nombre", dirigente.Nombre);
                command.Parameters.AddWithValue("@Apellido", dirigente.Apellido);
                command.Parameters.AddWithValue("@FechaNacimiento", dirigente.FechaNacimiento.Date);
                command.Parameters.AddWithValue("@Rol", dirigente.Rol);
                command.Parameters.AddWithValue("@Dni", dirigente.Dni);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id)
        {
            var sql = "DELETE FROM dbo.Dirigente WHERE DirigenteId = @Id";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        private Dirigente MapReaderToDirigente(SqlDataReader reader)
        {
            return new Dirigente
            {
                DirigenteId = reader.GetInt32(reader.GetOrdinal("DirigenteId")),
                ClubId = reader.GetInt32(reader.GetOrdinal("ClubId")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
                FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                Rol = reader.GetString(reader.GetOrdinal("Rol")),
                Dni = reader.GetInt32(reader.GetOrdinal("Dni"))
            };
        }
    }
}