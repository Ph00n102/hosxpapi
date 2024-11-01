using System.Text.RegularExpressions;
using hosxpapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hosxpapi.Controllers;
[Route("/api/[controller]/[action]")]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext db;
        public AppointmentController(ApplicationDbContext db)
        {
            this.db = db;
        }
         [HttpGet]
        public ActionResult GetAppointment(string _number)
        {
            var query =
                from o in db.Oapps
                join p in db.Patients on  o.Hn equals p.Hn
                join d in db.Doctors on o.Doctor equals d.Code
                join v in db.Ovsts on o.Hn equals v.Hn
                join k in db.Kskdepartments on o.Depcode equals k.Depcode
                join p2 in db.Pttypes on p.Pttype equals p2.Pttype1
                join c in db.Clinics on o.Clinic equals c.Clinic1
                join s in db.Spclties on o.Spclty equals s.Spclty1
                
                where p.Cid == _number || o.Hn == _number
                group new { o, p, d, k, v, p2 } by new
                {
                    o.OappId,
                    o.Hn,
                    o.Doctor,
                    d.Name,
                    o.Vstdate,
                    o.Nextdate,
                    o.Nexttime,
                    o.Vn,
                    o.Depcode,
                    k.Department,
                    o.Spclty,
                    Spcltyname = s.Name,
                    o.Clinic,
                    Clinicname = c.Name,
                    p.Pname,p.Fname,p.Lname,
                    p.Pttype,
                    Pttname = p2.Name,
                    o.AppCause,
                    o.Note,
                    o.Note1,
                    o.Note2,
                    o.LabListText,
                    p.Hometel,
                    p.Informtel
                } into grouped
                select new
                {
                    grouped.Key.OappId,
                    grouped.Key.Hn,
                    grouped.Key.Doctor,
                    grouped.Key.Name,
                    grouped.Key.Vstdate,
                    grouped.Key.Nextdate,
                    grouped.Key.Nexttime,
                    grouped.Key.Vn,
                    grouped.Key.Depcode,
                    grouped.Key.Department,
                    grouped.Key.Spclty,
                    grouped.Key.Spcltyname,
                    grouped.Key.Clinic,
                    grouped.Key.Clinicname,
                    grouped.Key.Pname,
                    grouped.Key.Fname,
                    grouped.Key.Lname,
                    grouped.Key.Pttype,
                    grouped.Key.Pttname,
                    grouped.Key.AppCause,
                    grouped.Key.Note,
                    grouped.Key.Note1,
                    grouped.Key.Note2,
                    grouped.Key.LabListText,
                    grouped.Key.Hometel,
                    grouped.Key.Informtel,
                };

                    
            return Json(query.Take(50).OrderByDescending(x => x.Nextdate));
        }

        [HttpGet()]
        public IActionResult CheckAppointment (int _OappId)
        {
            var query = db.Oapps.Find(_OappId);
            if (query == null)
            {
                return NotFound();
            }
                
                return Ok(query);
        }
        [HttpGet()]
        public IActionResult getAuthenAll (string _cid)
        {
            var query = db.Patients.FirstOrDefault(x => x.Cid == _cid);
            if (query == null)
            {
                return NotFound();
            }
                
                return Ok(query);
        }
        // ค้นห้าข้อมูลที่จะส่ง authen แบบ Object
        [HttpGet()]
        public async Task<IActionResult> getDataAuthenObj (string _cid)
        {
            var query = await db.Patients.FirstOrDefaultAsync(p => p.Cid == _cid);
            if (query == null)
            {
                return Ok(new {hn = "", hcode = "", hometel = "", informtel = "", Pname = "", Fname = "", Lname = "", Birthday = ""});
            }
                
                return Ok(new {query.Hn, query.Hcode, query.Hometel, query.Informtel, query.Pname , query.Fname , query.Lname, Age = CalculateAge(query.Birthday.GetValueOrDefault(DateOnly.FromDateTime(DateTime.Today))) });
              
        }
        [HttpGet]
        public IActionResult getpatient(string _cid)
        {
            DateTime today = DateTime.Today; 
            var query = 
                from a in db.Patients
                where a.Cid == _cid
                select new
                {
                    a.Hn, a.Hcode, a.Hometel, a.Informtel,a.Pname, a.Fname, a.Lname, Age = CalculateAge(a.Birthday.GetValueOrDefault(DateOnly.FromDateTime(DateTime.Today)))
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
        [HttpGet("/app/{_Cid}")]
        public async Task<ActionResult<PatientDto>> GetAppointmentsByIdAsync(string _Cid)
        {
            DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Now);
            var query = 
                from o in db.Oapps
                join p in db.Patients on o.Hn equals p.Hn
                join c in db.Clinics on o.Clinic equals c.Clinic1
                join v in db.Ovsts on o.Hn equals v.Hn
                join d in db.Doctors on o.Doctor equals d.Code
                join k in db.Kskdepartments on o.Depcode equals k.Depcode
                where p.Cid == _Cid //&& o.Nextdate == dateOnly
                group new { o, p, d, k } by new
                {
                    o.OappId,
                    o.Hn,
                    o.Doctor,
                    d.Name,
                    o.Vstdate,
                    o.Nextdate,
                    o.Nexttime,
                    o.Vn,
                    k.Department,
                    p.Pname,
                    p.Fname,
                    p.Lname,
                    p.Cid,
                    p.Hometel,
                    p.Birthday
                } into grouped
                select new PatientDto // Change this to your PatientDto structure
                {
                    OappId = grouped.Key.OappId,
                    Hn = grouped.Key.Hn,
                    Doctor = grouped.Key.Doctor,
                    DoctorName = grouped.Key.Name,
                    Vstdate = grouped.Key.Vstdate,
                    NextDate = grouped.Key.Nextdate,
                    NextTime = grouped.Key.Nexttime,
                    Vn = grouped.Key.Vn,
                    Department = grouped.Key.Department,
                    Pname = grouped.Key.Pname,
                    Fname = grouped.Key.Fname,
                    Lname = grouped.Key.Lname,
                    Cid = grouped.Key.Cid,
                    Hometel = grouped.Key.Hometel,
                    
                };

            return Ok(query.FirstOrDefault());
        }

    }