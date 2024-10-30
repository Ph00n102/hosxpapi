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
                return Json(query.Take(50));
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
            // select o.vn, o.hn, o.an, v.pdx, i.name, o.vstdate, o.vsttime, o.doctor, d.name as ชื่อแพทย์, o.ovstist, o2.name as ประเภทการมา, o.ovstost, o3.name as สถานะ, o.pttype, p.name as สิทธิ์การรักษา, o.spclty, s.name, o.diag_text, o.main_dep, k.department from ovst o

            // left outer join doctor d on d.code = o.doctor
            // left outer join ovstist o2 on o2.ovstist = o.ovstist
            // left outer join ovstost o3 on o3.ovstost = o.ovstost 
            // left outer join kskdepartment k on k.depcode = o.main_dep
            // left outer join pttype p on p.pttype = o.pttype 
            // left outer join spclty s on s.spclty = o.spclty
            // left outer join vn_stat v on v.vn = o.vn 
            // left outer join icd101 i on i.code = v.pdx

            // where o.vn = "671029131510"

            // limit 90

            var query =
                from o in db.Ovsts
                join v in db.VnStats on o.Vn equals v.Vn into x1
                from v1 in x1.DefaultIfEmpty()
                join i in db.Icd101s on v1.Pdx equals i.Code into x2
                from i1 in x2.DefaultIfEmpty()
                join d in db.Doctors on o.Doctor equals d.Code into x3
                from d1 in x3.DefaultIfEmpty()
                join o2 in db.Ovstists on o.Ovstist equals o2.Ovstist1 into x4
                from o3 in x4.DefaultIfEmpty()
                join o4 in db.Ovstosts on o.Ovstost equals o4.Ovstost1 into x5
                from o5 in x5.DefaultIfEmpty()
                join p in db.Pttypes on o.Pttype equals p.Pttype1 into x6
                from p1 in x6.DefaultIfEmpty()
                join s in db.Spclties on o.Spclty equals s.Spclty1 into x7
                from s1 in x7.DefaultIfEmpty()
                join k in db.Kskdepartments on o.MainDep equals k.Depcode into x8
                from k1 in x8.DefaultIfEmpty()
                
                
                where o.Hn == _hn orderby o.Vn descending
                 select new
                {
                    o.Vn, o.Hn, o.An, v1.Pdx, Pdxn = i1.Name, o.Vstdate, o.Vsttime, o.Doctor, d1.Name, o.Ovstist, Ovstistn = o3.Name, o.Ovstost, Ovstostn = o5.Name, o.Pttype, Pttypen = p1.Name, o.Spclty, Spcltyn = s1.Name, o.DiagText, o.MainDep, k1.Department 
                };
                return Json(query.Take(50)); 
        }


        [HttpGet]
        public IActionResult getopdvisitbyhndr(string _hn, string _doctor)
        {
            var query =
                from o in db.Ovsts
                join v in db.VnStats on o.Vn equals v.Vn into x1
                from v1 in x1.DefaultIfEmpty()
                join i in db.Icd101s on v1.Pdx equals i.Code into x2
                from i1 in x2.DefaultIfEmpty()
                join d in db.Doctors on o.Doctor equals d.Code into x3
                from d1 in x3.DefaultIfEmpty()
                join o2 in db.Ovstists on o.Ovstist equals o2.Ovstist1 into x4
                from o3 in x4.DefaultIfEmpty()
                join o4 in db.Ovstosts on o.Ovstost equals o4.Ovstost1 into x5
                from o5 in x5.DefaultIfEmpty()
                join p in db.Pttypes on o.Pttype equals p.Pttype1 into x6
                from p1 in x6.DefaultIfEmpty()
                join s in db.Spclties on o.Spclty equals s.Spclty1 into x7
                from s1 in x7.DefaultIfEmpty()
                join k in db.Kskdepartments on o.MainDep equals k.Depcode into x8
                from k1 in x8.DefaultIfEmpty()
                
                
                where o.Hn == _hn && o.Doctor == _doctor orderby o.Vn descending
                 select new
                {
                    o.Vn, o.Hn, o.An, v1.Pdx, Pdxn = i1.Name, o.Vstdate, o.Vsttime, o.Doctor, d1.Name, o.Ovstist, Ovstistn = o3.Name, o.Ovstost, Ovstostn = o5.Name, o.Pttype, Pttypen = p1.Name, o.Spclty, Spcltyn = s1.Name, o.DiagText, o.MainDep, k1.Department 
                };
                return Json(query.Take(50));  
        }

        [HttpGet]
        public IActionResult getopdvisitbyvn(string _vn)
        {
            var query =
                from o in db.Ovsts
                join v in db.VnStats on o.Vn equals v.Vn into x1
                from v1 in x1.DefaultIfEmpty()
                join i in db.Icd101s on v1.Pdx equals i.Code into x2
                from i1 in x2.DefaultIfEmpty()
                join d in db.Doctors on o.Doctor equals d.Code into x3
                from d1 in x3.DefaultIfEmpty()
                join o2 in db.Ovstists on o.Ovstist equals o2.Ovstist1 into x4
                from o3 in x4.DefaultIfEmpty()
                join o4 in db.Ovstosts on o.Ovstost equals o4.Ovstost1 into x5
                from o5 in x5.DefaultIfEmpty()
                join p in db.Pttypes on o.Pttype equals p.Pttype1 into x6
                from p1 in x6.DefaultIfEmpty()
                join s in db.Spclties on o.Spclty equals s.Spclty1 into x7
                from s1 in x7.DefaultIfEmpty()
                join k in db.Kskdepartments on o.MainDep equals k.Depcode into x8
                from k1 in x8.DefaultIfEmpty()
                
                
                where o.Vn == _vn orderby o.Vn descending
                 select new
                {
                    o.Vn, o.Hn, o.An, v1.Pdx, Pdxn = i1.Name, o.Vstdate, o.Vsttime, o.Doctor, d1.Name, o.Ovstist, Ovstistn = o3.Name, o.Ovstost, Ovstostn = o5.Name, o.Pttype, Pttypen = p1.Name, o.Spclty, Spcltyn = s1.Name, o.DiagText, o.MainDep, k1.Department 
                };
                return Json(query.Take(1)); 
        }

    }
}