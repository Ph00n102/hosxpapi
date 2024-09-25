using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using hosxpapi.Models;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace HosApi.Controllers;

[Route("/api/[controller]/[action]")]
    public class HosController : Controller
    {
        private readonly ApplicationDbContext db;
        public HosController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // ค้นหาชื่อคนไข้ด้วย QN หรือ HN
        [HttpGet]       
        public IActionResult getost(string _para)
        {
            DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Now);
            // select i.hn,i.vstdate,p.pname,p.fname,p.lname from ovst i
            // inner join patient p on p.hn  = i.hn
            // where i.vstdate= DATE(NOW()) and i.oqueue = '2284'
            string x = _para;
            if (x.Length < 5)
            {
                var query1 = 
                from a in db.Ovsts
                join b in db.Patients on  a.Hn equals b.Hn
                where a.Vstdate == dateOnly && Convert.ToString( a.Oqueue ) == _para 
                select new
                {
                    a.Hn, b.Pname, b.Fname, b.Lname, a.Vstdate
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
                    a.Hn, b.Pname, b.Fname, b.Lname,a.Vstdate
                };
                return Json(query2.Take(1));
            }
            else
            {
                return StatusCode(200, "patient not found");
            }
            
        }

        // ค้นหาชื่อคนไข้ด้วย QN หรือ HN
        [HttpPost]       
        public IActionResult getost2(string _para)
        {
            DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Now);
            // select i.hn,i.vstdate,p.pname,p.fname,p.lname from ovst i
            // inner join patient p on p.hn  = i.hn
            // where i.vstdate= DATE(NOW()) and i.oqueue = '2284'
            var x = _para;
            if (x.Length <= 4)
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
                    a.Hn, b.Pname, b.Fname, b.Lname,a.Vstdate
                };
                return Json(query2.Take(1));
            }
            else
            {
                return StatusCode(200, "patient not found");
            }
            
        }

        // ค้นหาชื่อ user ใน hosxp ด้วย username, password ด้วยการ login
        [HttpGet]
        public IActionResult getUser (string uname,string para)
        { 
           var x = MD5Hash(para);
           {
                return Json((from a in db.Opdusers
                         where a.Loginname == uname && a.Passweb == x 

                         select new
                         {
                             a.Loginname,
                             a.Passweb
                            

                         }).FirstOrDefault());
           }
            
        }

        // เปลี่ยน password --> hash5 ก่อนใช้ login ข้างบน
        public static string MD5Hash(string input)
        {
        StringBuilder hash = new StringBuilder();
        MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
        byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

        for (int i = 0; i < bytes.Length; i++)
        {
            hash.Append(bytes[i].ToString("x2"));
        }
        return hash.ToString();
        }


        // ค้นหาชื่อคนไข้ด้วย QN หรือ HN
        [HttpGet]       
        public IActionResult getlabcu(string _hn, string _labname)
        {
            //select lh.order_date, lo.lab_items_name_ref, lo.lab_order_result from lab_head lh	
            //left outer join lab_order lo on lo.lab_order_number = lh.lab_order_number
            //where hn = "0022200" AND lab_items_name_ref = "Hb A1C" order by lab_items_name_ref
                var query = 
                from a in db.LabHeads
                join b in db.LabOrders on a.LabOrderNumber equals b.LabOrderNumber  
                where a.Hn == _hn && b.LabItemsNameRef == _labname
                select new
                {
                    a.OrderDate, b.LabItemsNameRef, b.LabOrderResult
                };
                return Json(query.Take(200));
           
            
        }

        [HttpGet]
        public IActionResult GetDateTimeNow()
        {
            var currentDateTime = DateTime.Now;
            var formattedDateTime = currentDateTime.ToString("yyMMddHHmmss");
            return Ok(formattedDateTime);
        }
    

        
    }