using Microsoft.AspNetCore.Mvc;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System;

namespace Sales_CRM.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =====================================================
        // DB Connection
        // =====================================================
        private NpgsqlConnection GetConnection()
        {
            var connectionString = _configuration.GetConnectionString("PostgresConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Database connection string 'PostgresConnection' is missing.");

            return new NpgsqlConnection(connectionString);
        }

        // =====================================================
        // 1. Lead Status Update Reports
        // =====================================================
        [HttpGet("lead-status")]
        public IActionResult GetLeadStatusReport(DateTime? date)
        {
            using var connection = GetConnection();

            var sql = @"
                SELECT
                    status AS Status,
                    COUNT(*) AS Total
                FROM leads
                WHERE
                    CAST(@Date AS date) IS NULL
                    OR created_at::date = CAST(@Date AS date)
                GROUP BY status
                ORDER BY status;
            ";

            var result = connection.Query<StatusCountDto>(
                sql,
                new { Date = date }
            );

            return Ok(result);
        }

        // =====================================================
        // 2. Sales Info (THIS IS SALES PERFORMANCE)
        // =====================================================
        [HttpGet("sales-info")]
        public IActionResult GetSalesInfo(string? department = null, string? staff = null)
        {
            using var connection = GetConnection();

            // -----------------------------
            // 1. Departments dropdown
            // -----------------------------
            var departments = connection.Query<string>(@"
                SELECT name
                FROM departments
                ORDER BY name;
            ");

            // -----------------------------
            // 2. Staff dropdown (filtered by department)
            // -----------------------------
            var staffList = connection.Query<StaffNameDto>(@"
                SELECT DISTINCT
                    (u.first_name || ' ' || u.last_name) AS Name
                FROM users u
                JOIN user_departments ud ON u.id = ud.user_id
                JOIN departments d ON d.id = ud.department_id
                WHERE u.is_staff = true
                  AND (@Department IS NULL OR d.name = @Department)
                ORDER BY Name;
            ", new { Department = department });

            // -----------------------------
            // 3. Department achieved sales
            // -----------------------------
            var departmentAchievedSales = connection.ExecuteScalar<decimal>(@"
                SELECT COALESCE(SUM(l.deposits), 0)
                FROM leads l
                JOIN users u
                  ON l.assigned = (u.first_name || ' ' || u.last_name)
                JOIN user_departments ud ON u.id = ud.user_id
                JOIN departments d ON d.id = ud.department_id
                WHERE (@Department IS NULL OR d.name = @Department);
            ", new { Department = department });

            // -----------------------------
            // 4. Staff-wise achieved sales
            // -----------------------------
            var staffSales = connection.Query<StaffSalesDto>(@"
                SELECT
                    (u.first_name || ' ' || u.last_name) AS StaffName,
                    COALESCE(SUM(l.deposits), 0) AS AchievedSales
                FROM leads l
                JOIN users u
                  ON l.assigned = (u.first_name || ' ' || u.last_name)
                JOIN user_departments ud ON u.id = ud.user_id
                JOIN departments d ON d.id = ud.department_id
                WHERE (@Department IS NULL OR d.name = @Department)
                  AND (@Staff IS NULL OR (u.first_name || ' ' || u.last_name) = @Staff)
                GROUP BY StaffName
                ORDER BY StaffName;
            ", new { Department = department, Staff = staff });

            return Ok(new
            {
                Departments = departments,
                Staff = staffList,
                DepartmentSummary = new
                {
                    Department = department,
                    AchievedSales = departmentAchievedSales,
                    SalesTarget = 0
                },
                StaffSales = staffSales
            });
        }

        // =====================================================
        // 3. Leads Converted Report (By Salesperson)
        // =====================================================
        [HttpGet("leads-converted")]
        public IActionResult GetLeadsConvertedReport(string assigned)
        {
            using var connection = GetConnection();

            var sql = @"
                SELECT 
                    status AS Status,
                    COUNT(*) AS Total
                FROM leads
                WHERE assigned = @Assigned
                GROUP BY status
                ORDER BY status;
            ";

            var result = connection.Query<StatusCountDto>(
                sql,
                new { Assigned = assigned }
            );

            return Ok(result);
        }
    }

    // =====================================================
    // DTOs
    // =====================================================
    public class StatusCountDto
    {
        public string Status { get; set; }
        public int Total { get; set; }
    }

    public class StaffSalesDto
    {
        public string StaffName { get; set; }
        public decimal AchievedSales { get; set; }
    }

    public class StaffNameDto
    {
        public string Name { get; set; }
    }
}
