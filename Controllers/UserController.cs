using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;
using Sales_CRM.Models.DTOs;
using System;
using System.Linq;

namespace Sales_CRM.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ================= SIGNUP =================
        [HttpPost("signup")]
        public IActionResult Signup(SignupDto dto)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                var exists = conn.ExecuteScalar<int>(
                    "SELECT COUNT(1) FROM users WHERE email=@Email",
                    new { dto.Email }, tx);

                if (exists > 0) return BadRequest("Email already exists");

                var userId = conn.ExecuteScalar<int>(@"
                    INSERT INTO users (
                        first_name,last_name,email,phone,profile_image,
                        hourly_rate,sales_target,
                        facebook,linkedin,skype,email_signature,direction,
                        twilio_phone,twilio_whatsapp_enabled,
                        is_staff,is_admin,
                        password_hash,password_changed_at,
                        total_logged_time,last_month_logged_time,
                        this_month_logged_time,last_week_logged_time,
                        this_week_logged_time,
                        last_active_at,created_at,updated_at
                    )
                    VALUES (
                        @FirstName,@LastName,@Email,@Phone,@ProfileImage,
                        @HourlyRate,@SalesTarget,
                        @Facebook,@Linkedin,@Skype,@EmailSignature,@Direction,
                        @TwilioPhone,@TwilioWhatsappEnabled,
                        @IsStaff,@IsAdmin,
                        @PasswordHash,NOW(),
                        INTERVAL '0',INTERVAL '0',
                        INTERVAL '0',INTERVAL '0',
                        INTERVAL '0',
                        NOW(),NOW(),NOW()
                    ) RETURNING id;",
                    new
                    {
                        dto.FirstName,
                        dto.LastName,
                        dto.Email,
                        dto.Phone,
                        dto.ProfileImage,
                        dto.HourlyRate,
                        dto.SalesTarget,
                        dto.Facebook,
                        dto.Linkedin,
                        dto.Skype,
                        dto.EmailSignature,
                        dto.Direction,
                        dto.TwilioPhone,
                        dto.TwilioWhatsappEnabled,
                        dto.IsStaff,
                        dto.IsAdmin,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                    }, tx);

                if (dto.DepartmentIds != null)
                {
                    foreach (var d in dto.DepartmentIds)
                        conn.Execute(
                            "INSERT INTO user_departments VALUES (@u,@d)",
                            new { u = userId, d }, tx);
                }

                tx.Commit();
                return Ok(new { userId });
            }
            catch (Exception e)
            {
                tx.Rollback();
                return BadRequest(e.Message);
            }
        }

        // ================= GET USER BY ID =================
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);

            var user = conn.Query<UserResponseDto>(@"
                SELECT id,
                       first_name AS FirstName,
                       last_name AS LastName,
                       email,
                       phone,
                       profile_image,
                       hourly_rate,
                       sales_target,
                       facebook,
                       linkedin,
                       skype,
                       email_signature,
                       direction,
                       twilio_phone,
                       twilio_whatsapp_enabled,
                       is_staff,
                       is_admin
                FROM users
                WHERE id=@id",
                new { id }).FirstOrDefault();

            if (user == null) return NotFound();

            user.DepartmentIds = conn.Query<int>(
                "SELECT department_id FROM user_departments WHERE user_id=@id",
                new { id }).ToList();

            return Ok(user);
        }

        // ================= UPDATE USER =================
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, SignupDto dto)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                conn.Execute(@"
                    UPDATE users SET
                        first_name=@FirstName,
                        last_name=@LastName,
                        phone=@Phone,
                        profile_image=@ProfileImage,
                        hourly_rate=@HourlyRate,
                        sales_target=@SalesTarget,
                        facebook=@Facebook,
                        linkedin=@Linkedin,
                        skype=@Skype,
                        email_signature=@EmailSignature,
                        direction=@Direction,
                        twilio_phone=@TwilioPhone,
                        twilio_whatsapp_enabled=@TwilioWhatsappEnabled,
                        is_staff=@IsStaff,
                        is_admin=@IsAdmin,
                        updated_at=NOW()
                    WHERE id=@Id",
                    new
                    {
                        Id = id,
                        dto.FirstName,
                        dto.LastName,
                        dto.Phone,
                        dto.ProfileImage,
                        dto.HourlyRate,
                        dto.SalesTarget,
                        dto.Facebook,
                        dto.Linkedin,
                        dto.Skype,
                        dto.EmailSignature,
                        dto.Direction,
                        dto.TwilioPhone,
                        dto.TwilioWhatsappEnabled,
                        dto.IsStaff,
                        dto.IsAdmin
                    }, tx);

                conn.Execute(
                    "DELETE FROM user_departments WHERE user_id=@id",
                    new { id }, tx);

                if (dto.DepartmentIds != null)
                {
                    foreach (var d in dto.DepartmentIds)
                        conn.Execute(
                            "INSERT INTO user_departments VALUES (@id,@d)",
                            new { id, d }, tx);
                }

                tx.Commit();
                return Ok("Updated");
            }
            catch (Exception e)
            {
                tx.Rollback();
                return BadRequest(e.Message);
            }
        }

        // ================= DEPARTMENTS =================
        [HttpGet("departments")]
        public IActionResult GetDepartments()
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);

            return Ok(conn.Query("SELECT id,name FROM departments ORDER BY name"));
        }

        // ================= NOTES =================
        [HttpPost("notes")]
        public IActionResult AddNote(NoteDto dto)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);

            conn.Execute(
                "INSERT INTO user_notes (user_id,note,added_from) VALUES (@UserId,@Note,@AddedFrom)",
                dto);

            return Ok("Note added");
        }

        [HttpGet("{id}/notes")]
        public IActionResult GetNotes(int id)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);

            return Ok(conn.Query(
                "SELECT note,added_from,created_at FROM user_notes WHERE user_id=@id ORDER BY created_at DESC",
                new { id }));
        }

        // ================= TIMESHEETS =================

        [HttpPost("timesheets/start")]
        public IActionResult StartTimesheet(TimesheetDto dto)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);

            conn.Execute(@"
                INSERT INTO timesheets (user_id, task, start_time)
                VALUES (@UserId, @Task, NOW())",
                dto);

            return Ok("Timer started");
        }

        [HttpPost("timesheets/stop/{userId}")]
        public IActionResult StopTimesheet(int userId)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);

            conn.Execute(@"
                UPDATE timesheets
                SET 
                    end_time = NOW(),
                    worked_minutes = EXTRACT(EPOCH FROM (NOW() - start_time)) / 60
                WHERE user_id = @userId
                  AND end_time IS NULL",
                new { userId });

            return Ok("Timer stopped");
        }

        [HttpGet("{userId}/timesheets")]
        public IActionResult GetTimesheets(int userId)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");
            using var conn = new NpgsqlConnection(connStr);

            var data = conn.Query(@"
                SELECT task, start_time, end_time, worked_minutes
                FROM timesheets
                WHERE user_id = @userId
                ORDER BY start_time DESC",
                new { userId });

            return Ok(data);
        }
    }
}
