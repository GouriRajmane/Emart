using EMart.Models;
using EMart.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EMart.Repositories.Implementation
{

    public class RolesRepository : IRolesRepository
    {
        private readonly IConfiguration _configuration;

        public RolesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DBCS"));
        }

        public List<Roles> GetAll()
        {
            List<Roles> roles = new List<Roles>();

            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Roles", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "SELECTALL");

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    roles.Add(new Roles
                    {
                        RoleId = Convert.ToInt32(reader["RoleId"]),
                        RoleName = reader["RoleName"]?.ToString(),
                        CreatedOn = reader["CreatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedOn"]),
                        UpdatedOn = reader["UpdatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["UpdatedOn"]),
                        CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"]),
                        UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["UpdatedBy"])
                    });
                }

                con.Close();
            }

            return roles;
        }

        public Roles GetById(int id)
        {
            Roles role = new Roles();

            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Roles", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "SELECTBYID");
                cmd.Parameters.AddWithValue("@RoleId", id);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    role.RoleId = Convert.ToInt32(reader["RoleId"]);
                    role.RoleName = reader["RoleName"]?.ToString();
                    role.CreatedOn = reader["CreatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedOn"]);
                    role.UpdatedOn = reader["UpdatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["UpdatedOn"]);
                    role.CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"]);
                    role.UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["UpdatedBy"]);
                }

                con.Close();
            }

            return role;
        }

        public void Insert(Roles model)
        {
            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Roles", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@RoleName", model.RoleName);
                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }
        }

        public void Update(Roles model)
        {
            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Roles", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@RoleId", model.RoleId);
                cmd.Parameters.AddWithValue("@RoleName", model.RoleName);
                cmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Roles", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@RoleId", id);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }
        }
    }
}
