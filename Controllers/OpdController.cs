using hosxpapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace hosxpapi.Controllers
{
    [Route("/api/[controller]/[action]")]
    public class OpdController : Controller
    {
        private readonly ApplicationDbContext db;
        
        public OpdController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult getpatient(string _hn)
        {
            var query = 
                from a in db.Patients
                join b in db.PatientImages on a.Hn equals b.Hn
                select new
                {
                    a.Pname, a.Fname, a.Lname, b.Image
                };
                return Json(query.Take(50));
        }
    }
}