using System.Runtime.CompilerServices;
using hosxpapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HosApi.Controllers;

[Route("/api/[controller]/[action]")]
    public class HosController : Controller
    {
        private readonly ApplicationDbContext db;
        public HosController(ApplicationDbContext db)
        {
            this.db = db;
        }


        [HttpGet]       
        public IActionResult getost(string _para)
        {
            DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Now);
            // select i.hn,i.vstdate,p.pname,p.fname,p.lname from ovst i
            // inner join patient p on p.hn  = i.hn
            // where i.vstdate= DATE(NOW()) and i.oqueue = '2284'
            var x = _para;
            if (x.Length == 4)
            {
                var query1 = 
                from a in db.Ovsts
                join b in db.Patients on  a.Hn equals b.Hn
                where a.Vstdate == dateOnly && Convert.ToString( a.Oqueue ) == _para 
                select new
                {
                    a.Hn, b.Pname, b.Fname, b.Lname,a.Vstdate
                };
                return Json(query1.Take(50));
            }
            else if (x.Length == 7)
            {
                var query2 = 
                from a in db.Ovsts
                join b in db.Patients on a.Hn equals b.Hn  
                where a.Hn == _para 
                select new
                {
                    b.Pname, b.Fname, b.Lname
                };
                return Json(query2.First());
            }
            else
            {
                return NotFound("patient not found");
            }
            
        }
      
        [HttpGet]
        public IActionResult getHosdb (string paraHN)
        { 
            var query =
            from a in db.Patients
            join b in db.Ovsts on a.Hn equals b.Hn  

            where a.Hn == paraHN 
            // group a by a.Hn  into newGroup
            select new
            {
                b.Hn,
                a.Cid,
                a.Pname,
                a.Fname,
                a.Lname
                // c.Name,
                // c.OldCode
            };
            return Json(query.Take(50));
        }
        [HttpPost]
        public async Task<ActionResult<List<Ward>>> AddWard(Ward ward)
        {
            db.Wards.Add(ward);
            await db.SaveChangesAsync();
                
            return Ok(await db.Wards.ToListAsync());
        }
        [HttpPut]
        public async Task<ActionResult<List<Ward>>> UpdateWard(Ward updateward)
        {
            var dbward = await db.Wards.FindAsync(updateward.Ward1);
            if (dbward is null)
                return NotFound("Ward not found");
            dbward.Name = updateward.Name;

            await db.SaveChangesAsync();
                
            return Ok(await db.Wards.ToListAsync());
        }

         [HttpDelete]
        public async Task<ActionResult<List<Ward>>> DeleteWard(int id)
        {
            var dbward = await db.Wards.FindAsync(id);
            if (dbward is null)
                return NotFound("Ward not found");
            
            db.Wards.Remove(dbward);
            await db.SaveChangesAsync();
                
            return Ok(await db.Wards.ToListAsync());
        }
    }