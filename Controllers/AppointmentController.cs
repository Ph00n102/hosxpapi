using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
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

        // select o.oapp_id, o.nextdate, o.nexttime, o.hn, o.vn, p.cid, concat(p.pname, p.fname,' ',p.lname) as ptname, p.pttype, p2.name as namepttype, o.doctor, d.name as namedoctor, o.depcode, k.department, o.spclty, s.name as namespclty, o.clinic, c.name as nameclinic, o.vstdate, o.note, p.hometel, p.informtel from oapp o

        // left outer join patient p on p.hn = o.hn
        // left outer join doctor d on d.code = o.doctor
        // left outer join kskdepartment k on k.depcode = o.depcode
        // left outer join spclty s on s.spclty = o.spclty
        // left outer join clinic c on c.clinic = o.clinic
        // left outer join pttype p2 on p2.pttype = p.pttype

        // where o.hn = "0022000" order by vn desc

        // limit 10

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

        // ค้นหาข้อมูลที่จะส่ง authen แบบ Array
        [HttpGet]       
        public IActionResult getDataAuthen(string _cid)
        {
                var query = 
                from a in db.Patients
                where a.Cid == _cid
                select new
                {
                    a.Hn, a.Hcode, a.Hometel, a.Informtel
                };
                return Ok(query);
           
            
        }

        // ค้นห้าข้อมูลที่จะส่ง authen แบบ Object
        [HttpGet()]
        public async Task<IActionResult> getDataAuthenObj (string _cid)
        {
            var query = await db.Patients
                .FirstOrDefaultAsync(p => p.Cid == _cid);
            if (query == null)
            {
                return Ok(new {hn = "", hcode = "", hometel = "", informtel= ""});
            }
                
                return Ok(new {query.Hn, query.Hcode, query.Hometel, query.Informtel});
              
        }

    }