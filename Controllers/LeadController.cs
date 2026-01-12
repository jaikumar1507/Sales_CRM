using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Sales_CRM.Models;
using Sales_CRM.Models.DTOs;

namespace Sales_CRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LeadsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =====================================================
        // 1️⃣ LEADS LIST (WITH FILTERS)
        // =====================================================
        [HttpGet("list")]
        public ActionResult<List<LeadListDto>> GetLeadsList(
            [FromQuery] string? status,
            [FromQuery] string? assigned,
            [FromQuery] string? source
        )
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            var leads = new List<LeadListDto>();

            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            string sql = @"
                SELECT 
                    id,
                    name,
                    email,
                    phone,
                    deposits,
                    comments,
                    assigned,
                    status,
                    source,
                    tags,
                    whatsapp_enable,
                    last_contact,
                    created_at
                FROM leads
                WHERE 1=1
            ";

            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            if (!string.IsNullOrWhiteSpace(status))
            {
                sql += " AND status = @status";
                cmd.Parameters.AddWithValue("@status", status);
            }

            if (!string.IsNullOrWhiteSpace(assigned))
            {
                sql += " AND assigned = @assigned";
                cmd.Parameters.AddWithValue("@assigned", assigned);
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                sql += " AND source = @source";
                cmd.Parameters.AddWithValue("@source", source);
            }

            sql += " ORDER BY id DESC;";
            cmd.CommandText = sql;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                leads.Add(new LeadListDto
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Deposits = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    Comments = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Assigned = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Status = reader.IsDBNull(7) ? null : reader.GetString(7),
                    Source = reader.IsDBNull(8) ? null : reader.GetString(8),
                    Tags = reader.IsDBNull(9) ? null : reader.GetString(9),
                    WhatsappEnable = !reader.IsDBNull(10) && reader.GetBoolean(10),
                    LastContact = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                    CreatedAt = reader.GetDateTime(12)
                });
            }

            return Ok(leads);
        }

        // =====================================================
        // 2️⃣ CREATE NEW LEAD
        // =====================================================
        [HttpPost]
        public IActionResult CreateLead([FromBody] Lead lead)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");

            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            string sql = @"
                INSERT INTO leads
                (status, source, assigned, name, position, email, website, phone,
                 deposits, comments, description, address, city, state, country, zipcode)
                VALUES
                (@status, @source, @assigned, @name, @position, @email, @website, @phone,
                 @deposits, @comments, @description, @address, @city, @state, @country, @zipcode);
            ";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@status", (object?)lead.Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@source", (object?)lead.Source ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@assigned", (object?)lead.Assigned ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name", lead.Name);
            cmd.Parameters.AddWithValue("@position", (object?)lead.Position ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)lead.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@website", (object?)lead.Website ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", (object?)lead.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@deposits", (object?)lead.Deposits ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@comments", (object?)lead.Comments ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)lead.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@address", (object?)lead.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@city", (object?)lead.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@state", (object?)lead.State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@country", (object?)lead.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@zipcode", (object?)lead.Zipcode ?? DBNull.Value);

            cmd.ExecuteNonQuery();

            return Ok("New Lead created successfully");
        }

        // =====================================================
        // 3️⃣ VIEW LEAD DETAILS (CLICK LEAD NAME)
        // =====================================================
        [HttpGet("details")]
        public IActionResult GetLeadDetails([FromQuery] int leadId)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");

            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            string sql = "SELECT * FROM leads WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", leadId);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return NotFound("Lead not found");

            var lead = new
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader["name"] as string,
                Status = reader["status"] as string,
                Source = reader["source"] as string,
                Assigned = reader["assigned"] as string,
                Position = reader["position"] as string,
                Email = reader["email"] as string,
                Website = reader["website"] as string,
                Phone = reader["phone"] as string,
                Deposits = reader["deposits"] as decimal?,
                Comments = reader["comments"] as string,
                Description = reader["description"] as string,
                Address = reader["address"] as string,
                City = reader["city"] as string,
                State = reader["state"] as string,
                Country = reader["country"] as string,
                Zipcode = reader["zipcode"] as string,
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };

            return Ok(lead);
        }

        // =====================================================
        // 4️⃣ EDIT LEAD (FROM VIEW PAGE)
        // =====================================================
        [HttpPut("update")]
        public IActionResult UpdateLead([FromBody] Lead lead)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");

            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            string sql = @"
                UPDATE leads SET
                    status=@status,
                    source=@source,
                    assigned=@assigned,
                    name=@name,
                    position=@position,
                    email=@email,
                    website=@website,
                    phone=@phone,
                    deposits=@deposits,
                    comments=@comments,
                    description=@description,
                    address=@address,
                    city=@city,
                    state=@state,
                    country=@country,
                    zipcode=@zipcode
                WHERE id=@id
            ";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", lead.Id);
            cmd.Parameters.AddWithValue("@status", (object?)lead.Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@source", (object?)lead.Source ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@assigned", (object?)lead.Assigned ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name", lead.Name);
            cmd.Parameters.AddWithValue("@position", (object?)lead.Position ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)lead.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@website", (object?)lead.Website ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", (object?)lead.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@deposits", (object?)lead.Deposits ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@comments", (object?)lead.Comments ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)lead.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@address", (object?)lead.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@city", (object?)lead.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@state", (object?)lead.State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@country", (object?)lead.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@zipcode", (object?)lead.Zipcode ?? DBNull.Value);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound("Lead not found");

            return Ok("Lead updated successfully");
        }

        // =====================================================
        // 5️⃣ DELETE LEAD (FROM VIEW PAGE)
        // =====================================================
        [HttpDelete("delete")]
        public IActionResult DeleteLead([FromQuery] int leadId)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");

            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            string sql = "DELETE FROM leads WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", leadId);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound("Lead not found");

            return Ok("Lead deleted successfully");
        }
    }
}
