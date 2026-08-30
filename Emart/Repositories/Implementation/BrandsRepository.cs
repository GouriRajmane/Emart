using EMart.Models;
using EMart.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EMart.Repositories.Implementation
{
    public class BrandsRepository : IBrandsRepository
    {
        private readonly IConfiguration _configuration;

        public BrandsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DBCS"));
        }

        public List<Brands> GetAll()
        {
            List<Brands> brands = new List<Brands>();

            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Brands", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "SELECTALL");

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    brands.Add(new Brands
                    {
                        BrandId = Convert.ToInt32(reader["BrandId"]),
                        BrandName = reader["BrandName"]?.ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedOn = reader["CreatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedOn"]),
                        UpdatedOn = reader["UpdatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["UpdatedOn"]),
                        CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"]),
                        UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["UpdatedBy"]),
                        LogoPath = reader["LogoPath"] == DBNull.Value ? null : reader["LogoPath"].ToString()
                    });
                }

                con.Close();
            }

            return brands;
        }

        public Brands GetById(int id)
        {
            Brands brand = new Brands();

            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Brands", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "SELECTBYID");
                cmd.Parameters.AddWithValue("@BrandId", id);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    brand.BrandId = Convert.ToInt32(reader["BrandId"]);
                    brand.BrandName = reader["BrandName"]?.ToString();
                    brand.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    brand.CreatedOn = reader["CreatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedOn"]);
                    brand.UpdatedOn = reader["UpdatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["UpdatedOn"]);
                    brand.CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"]);
                    brand.UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["UpdatedBy"]);
                    brand.LogoPath = reader["LogoPath"] == DBNull.Value ? null : reader["LogoPath"].ToString();

                }

                con.Close();
            }

            return brand;
        }

        public void Insert(Brands model)
        {
            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Brands", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@BrandName", model.BrandName);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                cmd.Parameters.AddWithValue("@LogoPath", model.LogoPath);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void Update(Brands model)
        {
            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Brands", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@BrandId", model.BrandId);
                cmd.Parameters.AddWithValue("@BrandName", model.BrandName);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy);
                cmd.Parameters.AddWithValue("@LogoPath", model.LogoPath);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection con = GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_Brands", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@BrandId", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}