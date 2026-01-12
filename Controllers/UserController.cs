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
        public IActionResult Signup([FromBody] SignupDto dto)
        {
            var connStr = _configuration.GetConnectionString("PostgresConnection");

            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                // 1️⃣ Check if email already exists
                var emailExists = conn.ExecuteScalar<int>(
                    "SELECT COUNT(1) FROM users WHERE email = @Email",
                    new { dto.Email },
                    tx
                );

                if (emailExists > 0)
                    return BadRequest("Email already registered");

                // 2️⃣ Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // 3️⃣ Insert user (ALL COLUMNS)
                var userSql = @"
                    INSERT INTO users (
                        first_name, last_name, email, phone, profile_image,
                        hourly_rate, sales_target,
                        facebook, linkedin, skype, email_signature, direction,
                        twilio_phone, twilio_whatsapp_enabled,
                        is_staff, is_admin,
                        password_hash, password_changed_at,
                        total_logged_time, last_month_logged_time,
                        this_month_logged_time, last_week_logged_time,
                        this_week_logged_time,
                        last_active_at, created_at, updated_at
                    )
                    VALUES (
                        @FirstName, @LastName, @Email, @Phone, @ProfileImage,
                        @HourlyRate, @SalesTarget,
                        @Facebook, @Linkedin, @Skype, @EmailSignature, @Direction,
                        @TwilioPhone, @TwilioWhatsappEnabled,
                        @IsStaff, @IsAdmin,
                        @PasswordHash, NOW(),
                        INTERVAL '0 hours', INTERVAL '0 hours',
                        INTERVAL '0 hours', INTERVAL '0 hours',
                        INTERVAL '0 hours',
                        NOW(), NOW(), NOW()
                    )
                    RETURNING id;
                ";

                var userId = conn.ExecuteScalar<int>(
                    userSql,
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
                        PasswordHash = passwordHash
                    },
                    tx
                );

                // 4️⃣ Insert departments
                if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
                {
                    const string deptSql = @"
                        INSERT INTO user_departments (user_id, department_id)
                        VALUES (@UserId, @DepartmentId);
                    ";

                    foreach (var deptId in dto.DepartmentIds)
                    {
                        conn.Execute(
                            deptSql,
                            new { UserId = userId, DepartmentId = deptId },
                            tx
                        );
                    }
                }

                tx.Commit();
                return Ok(new
                {
                    message = "User registered successfully",
                    userId
                });
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return BadRequest(ex.Message);
            }
        }
    }
}
