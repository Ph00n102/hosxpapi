using System.Text.RegularExpressions;
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
            DateTime today = DateTime.Today; 
            var query = 
                from a in db.Patients
                join b in db.PatientImages on a.Hn equals b.Hn
                where a.Hn == _hn
                select new
                {
                    a.Pname, a.Fname, a.Lname, b.Image, Age = CalculateAge(a.Birthday.GetValueOrDefault(DateOnly.FromDateTime(DateTime.Today)))
                };
                return Json(query.Take(1));
        }

        private static int CalculateAge(DateOnly birthDate)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            var match = Regex.Match(birthDate.ToString(), @"\b\d{4}\b");

            int age = (today.Year + 543) - int.Parse(match.Value);

            return age;
        }

        [HttpGet]
        public IActionResult getopdvisitbyhn(string _hn)
        {
            var query =
                from o in db.Ovsts
                join d in db.Doctors on o.Doctor equals d.Code
                join o2 in db.Ovstists on o.Ovstist equals o2.Ovstist1
                join o3 in db.Ovstosts on o.Ovstost equals o3.Ovstost1
                join k in db.Kskdepartments on o.MainDep equals k.Depcode
                join p in db.Pttypes on o.Pttype equals p.Pttype1
                join s in db.Spclties on o.Spclty equals s.Spclty1
                join v in db.VnStats on o.Vn equals v.Vn
                join i in db.Icd101s on v.Pdx equals i.Code
                where o.Hn == _hn orderby o.Vstdate descending
                 select new
                {
                    o.Vn, o.Hn, o.An, v.Pdx, Pdxn = i.Name, o.Vstdate, o.Vsttime, o.Doctor, d.Name, o.Ovstist, Ovstistn = o2.Name, o.Ovstost, Ovstostn = o3.Name, o.Pttype, Pttypen = p.Name, o.Spclty, Spcltyn = s.Name, o.DiagText, o.MainDep, k.Department 
                };
                return Json(query.Take(1)); 
        }

        [HttpGet]
        public IActionResult getopdvisitbyvn(string _vn)
        {
            var query =
                from o in db.Ovsts
                join d in db.Doctors on o.Doctor equals d.Code
                join o2 in db.Ovstists on o.Ovstist equals o2.Ovstist1
                join o3 in db.Ovstosts on o.Ovstost equals o3.Ovstost1
                join k in db.Kskdepartments on o.MainDep equals k.Depcode
                join p in db.Pttypes on o.Pttype equals p.Pttype1
                join s in db.Spclties on o.Spclty equals s.Spclty1
                join v in db.VnStats on o.Vn equals v.Vn
                join i in db.Icd101s on v.Pdx equals i.Code
                where o.Vn == _vn orderby o.Vn descending
                 select new
                {
                    o.Vn, o.Hn, o.An, v.Pdx, Pdxn = i.Name, o.Vstdate, o.Vsttime, o.Doctor, d.Name, o.Ovstist, Ovstistn = o2.Name, o.Ovstost, Ovstostn = o3.Name, o.Pttype, Pttypen = p.Name, o.Spclty, Spcltyn = s.Name, o.DiagText, o.MainDep, k.Department 
                };
                return Json(query.Take(1)); 
        }

    }
}




