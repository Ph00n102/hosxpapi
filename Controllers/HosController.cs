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
        public IActionResult getHos (string paraHN)
        { 
            //  var query = Ovst  Patient
            // from a in db.Wards
            if (paraHN.Length == 7)
                {
                var queryHN =
                from a in db.Patients
                join b in db.Ovsts on a.Hn equals b.Hn  
                where a.Hn == paraHN 
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
                    return Json(queryHN.Take(50));
                    
                }
                DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);
                var queryQN = 
                from a in db.Ovsts
                join b in db.Patients on  a.Hn equals b.Hn
                where a.Vstdate == dateNow && Convert.ToString( a.Oqueue ) == paraHN 
                select new
                {
                    a.Hn, 
                    b.Pname, 
                    b.Fname, 
                    b.Lname,
                    a.Vstdate
                };
                return Json(queryQN.Take(50));
                
            // group a by a.Hn  into newGroup
           
            
        }
         [HttpGet]       
        public IActionResult getQNdb(string paraQN)
        {
            DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);
            var query = 
            from a in db.Ovsts
            join b in db.Patients on  a.Hn equals b.Hn
            where a.Vstdate == dateNow && Convert.ToString( a.Oqueue ) == paraQN 
            select new
            {
                a.Hn, 
                b.Pname, 
                b.Fname, 
                b.Lname,
                a.Vstdate
            };
            return Json(query.Take(50));
        }
    }