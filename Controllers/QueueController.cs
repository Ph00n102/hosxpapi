using System.Security.Cryptography.X509Certificates;
using hosxpapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace hosxpapi.Controllers
{
    [Route("/api/[controller]/[action]")]
    public class QueueController : Controller
    {
        private readonly ApplicationDbContext db;
        public QueueController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult genqn()
        {
            var currentDateTime = DateTime.Now;
            var formattedDateTime = "ovst-q-" + currentDateTime.ToString("yyMMdd");
            var query =
                from a in db.Serials where a.Name == formattedDateTime
                select new
                {
                    a.Name, a.SerialNo
                };
                return Json(query.Take(1));
        }

        [HttpGet]
        public IActionResult GetDateTimeNow()
        {
            var currentDateTime = DateTime.Now;
            var formattedDateTime = "ovst-q-" + currentDateTime.ToString("yyMMdd");

            return Ok(formattedDateTime);
        }

    
    }
}