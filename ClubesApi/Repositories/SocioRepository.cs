using Microsoft.Data.SqlClient;
using ClubesApi.Models;
using ClubesApi.DTOs;
using System.Collections.Generic;
using System;
using System.Data;

namespace ClubesApi.Repositories
{
    public class SocioRepository
    {
        private readonly string _connectionString;

        public SocioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool DniExists(int dni)
        {
            var sql = "SELECT COUNT(*) FROM dbo.Socio WHERE Dni = @Dni";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Dni", dni);
                connection.Open();

                var count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public Socio? Create(SocioCreateDto dto)
        {
            var sql = @"
                INSERT INTO dbo.Socio (ClubId, Nombre, Apellido, FechaNacimiento, FechaAsociado, Dni, CantidadAsistencias)
                VALUES (@ClubId, @Nombre, @Apellido, @FechaNacimiento, @FechaAsociado, @Dni, 0); 
                SELECT SCOPE_IDENTITY();
            ";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClubId", dto.ClubId);
                command.Parameters.AddWithValue("@Nombre", dto.Nombre);
                command.Parameters.AddWithValue("@Apellido", dto.Apellido);
                command.Parameters.AddWithValue("@FechaNacimiento", dto.FechaNacimiento.Date);
                command.Parameters.AddWithValue("@FechaAsociado", dto.FechaAsociado.Date);
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
        public Socio? GetById(int id)
        {
            var sql = "SELECT SocioId, ClubId, Nombre, Apellido, FechaNacimiento, FechaAsociado, Dni, CantidadAsistencias FROM dbo.Socio WHERE SocioId = @Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapReaderToSocio(reader) : null;
                }
            }
        }
        public List<Socio> GetAll()
        {
            var socios = new List<Socio>();
            var sql = "SELECT SocioId, ClubId, Nombre, Apellido, FechaNacimiento, FechaAsociado, Dni, CantidadAsistencias FROM dbo.Socio";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        socios.Add(MapReaderToSocio(reader));
                    }
                }
            }
            return socios;
        }

        public bool Update(Socio socio)
        {
            var sql = @"
                UPDATE dbo.Socio 
                SET ClubId = @ClubId, 
                    Nombre = @Nombre, 
                    Apellido = @Apellido, 
                    FechaNacimiento = @FechaNacimiento, 
                    FechaAsociado = @FechaAsociado, 
                    Dni = @Dni, 
                    CantidadAsistencias = @CantidadAsistencias
                WHERE SocioId = @SocioId;
            ";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@SocioId", socio.SocioId);
                command.Parameters.AddWithValue("@ClubId", socio.ClubId);
                command.Parameters.AddWithValue("@Nombre", socio.Nombre);
                command.Parameters.AddWithValue("@Apellido", socio.Apellido);
                command.Parameters.AddWithValue("@FechaNacimiento", socio.FechaNacimiento.Date);
                command.Parameters.AddWithValue("@FechaAsociado", socio.FechaAsociado.Date);
                command.Parameters.AddWithValue("@Dni", socio.Dni);
                command.Parameters.AddWithValue("@CantidadAsistencias", socio.CantidadAsistencias);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
        public bool Delete(int id)
        {
            var sql = "DELETE FROM dbo.Socio WHERE SocioId = @Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
        private Socio MapReaderToSocio(SqlDataReader reader)
        {
            return new Socio
            {
                SocioId = reader.GetInt32(reader.GetOrdinal("SocioId")),
                ClubId = reader.GetInt32(reader.GetOrdinal("ClubId")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
                FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                FechaAsociado = reader.GetDateTime(reader.GetOrdinal("FechaAsociado")),
                Dni = reader.GetInt32(reader.GetOrdinal("Dni")),
                CantidadAsistencias = reader.GetInt32(reader.GetOrdinal("CantidadAsistencias"))
            };
        }
    }
}