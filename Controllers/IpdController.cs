using hosxpapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace hosxpapi.Controllers
{
    [Route("/api/[controller]/[action]")]
    public class IpdController : Controller
    {
        private readonly ApplicationDbContext db;
        
        public IpdController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]       
        public IActionResult getward()
        {
            var query =
                from a in db.Wards where !a.Name.Contains("(เลิกใช้)") orderby a.Ward1
                select new
                {
                    a.Ward1, a.Name 
                };
                return Json(query.Take(100));
        }

        [HttpGet]
        public IActionResult getpatient(string _wardno)
        {
            var query = 
                from a in db.Ipts 
                join b in db.Patients on a.Hn equals b.Hn
                join c in db.AnStats on a.An equals c.An
                join d in db.PatientImages on a.Hn equals d.Hn
                join e in db.Iptadms on a.An equals e.An
                where a.Ward == _wardno && b.Admit == "Y" && a.Dchdate == null
                select new
                {
                    a.Ward, e.Bedno, e.Indate, a.An, a.Vn, b.Pname, b.Fname, b.Lname, c.AgeY, d.Image
                };
                return Json(query.Take(50));
        }
    }
}