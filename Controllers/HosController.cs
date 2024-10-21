using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using hosxpapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using hosxpapi.Services;


namespace HosApi.Controllers;

[Route("/api/[controller]/[action]")]
    public class HosController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly GetSerial _service;
        public HosController(ApplicationDbContext db, GetSerial service)
        {
            this.db = db;
            _service = service;
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

        // ค้นหาชื่อ user ใน hosxp ด้วย username, password ด้วยการ login
        [HttpPost]
        public IActionResult PostUser (string uname,string para)
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

        // generate vn
        [HttpGet]
        public IActionResult GetDateTimeNow()
        {
            var currentDateTime = DateTime.Now;
            var formattedDateTime = currentDateTime.ToString("yyMMddHHmmss");
            return Ok(formattedDateTime);
        }
    

        // เช็ค vn ว่ามีอยู่ใน table ovst หรือไม่
        [HttpGet]
        public IActionResult CheckVn(string _vn)
        {
            
            var query = 
            db.Ovsts.SingleOrDefault(u => u.Vn == _vn);
            if (query == null)
            {
                return StatusCode(404, "vn are not found in ovst table"); 
            }
            return StatusCode(200, "vn are found in ovst table"); 
            
        }

        // เพิ่ม vn ในตาราง ovst
        [HttpPost]
        public async Task<IActionResult> OpenVisit(ClientDto clientDto)
        {
            var otherClient = db.Ovsts.FirstOrDefault(c => c.HosGuid == clientDto.hosGuid);
            if (otherClient != null)
            {
                ModelState.AddModelError("HosGuid", "The HosGuid is already used");
                var validation = new ValidationProblemDetails(ModelState);
                return BadRequest(validation);
            }

            var _Oqueue = await _service.GetSerialNumber();

            var client = new Ovst
            {
                HosGuid = "{"+Guid.NewGuid().ToString()+"}",
                Vn = DateTime.Now.ToString("yyMMddHHmmss", new CultureInfo("th-TH")),
                Hn = clientDto.hn,
                Vstdate = DateOnly.FromDateTime(DateTime.Now),
                Vsttime = TimeOnly.FromDateTime(DateTime.Now),
                Doctor = clientDto.doctor,
                Hospmain = clientDto.hospmain,
                Hospsub = clientDto.hospsub,
                Oqueue = Convert.ToInt32(_Oqueue),
                Ovstist = clientDto.ovstist,
                Ovstost = clientDto.ovstost,
                Pttype = clientDto.pttype,
                Pttypeno = clientDto.pttypeno,
                Rfrocs = clientDto.rfrocs,
                Spclty = clientDto.spclty,
                Hcode = "10734",
                CurDep = clientDto.curDep,
                CurDepBusy = "N",
                LastDep = clientDto.lastDep,
                PtSubtype = 0,
                MainDep = clientDto.mainDep,
                MainDepQueue = 0,
                VisitType = clientDto.visitType,
                NodeId = clientDto.nodeId,
                Waiting = "Y",
                HasInsurance = clientDto.hasInsurance,
                Staff = clientDto.staff,
                PtPriority = 0,
                OvstKey = "9564AB24894CF188CC14EB2D81857600",
            };

            db.Ovsts.Add(client);
            db.SaveChanges();

            return Ok(client);
        }

        

        [HttpPut("{_HosGuid}")]
        public async Task<ActionResult> UpdateOvst(string _HosGuid, Ovst ovst)
        {
            if (_HosGuid != ovst.HosGuid)
            {
                return BadRequest();
            }
            var result = await db.Ovsts.FindAsync(_HosGuid);
            if(result == null)
            {
                return NotFound();
            }

            result.HosGuid = ovst.HosGuid;
            result.Vn = ovst.Vn;
            result.Hn = ovst.Hn;
            result.An = ovst.An;
            result.Vstdate = ovst.Vstdate;
            result.Vsttime = ovst.Vsttime;
            result.Doctor = ovst.Doctor;
            result.Hospmain = ovst.Hospmain;
            result.Hospsub = ovst.Hospsub;
            result.Oqueue = ovst.Oqueue;
            result.Ovstist = ovst.Ovstist;
            result.Ovstost = ovst.Ovstost;
            result.Pttype = ovst.Pttype;
            result.Pttypeno = ovst.Pttypeno;
            result.Rfrics = ovst.Rfrics;
            result.Rfrilct = ovst.Rfrilct;
            result.Rfrocs = ovst.Rfrocs;
            result.Rfrolct = ovst.Rfrolct;
            result.Spclty = ovst.Spclty;
            result.RcptDisease = ovst.RcptDisease;
            result.Hcode = ovst.Hcode;
            result.CurDep = ovst.CurDep;
            result.CurDepBusy = ovst.CurDepBusy;
            result.LastDep = ovst.LastDep;
            result.CurDepTime = ovst.CurDepTime;
            result.RxQueue = ovst.RxQueue;
            result.DiagText = ovst.DiagText;
            result.PtSubtype = ovst.PtSubtype;
            result.MainDep = ovst.MainDep;
            result.MainDepQueue = ovst.MainDepQueue;
            result.FinanceSummaryDate = ovst.FinanceSummaryDate;
            result.VisitType = ovst.VisitType;
            result.NodeId = ovst.NodeId;
            result.ContractId = ovst.ContractId;
            result.Waiting = ovst.Waiting;
            result.RfriIcd10 = ovst.RfriIcd10;
            result.OReferNumber = ovst.OReferNumber;
            result.HasInsurance = ovst.HasInsurance;
            result.IReferNumber = ovst.IReferNumber;
            result.ReferType = ovst.ReferType;
            result.OReferDep = ovst.OReferDep;
            result.Staff = ovst.Staff;
            result.CommandDoctor = ovst.CommandDoctor;
            result.SendPerson = ovst.SendPerson;
            result.PtPriority = ovst.PtPriority;
            result.FinanceLock = ovst.FinanceLock;
            result.FinanceAlient = ovst.FinanceAlient;
            result.Oldcode = ovst.Oldcode;
            result.SignDoctor = ovst.SignDoctor;
            result.AnonymousVisit = ovst.AnonymousVisit;
            result.AnonymousVn = ovst.AnonymousVn;
            result.PtCapabilityTypeId = ovst.PtCapabilityTypeId;
            result.AtHospital = ovst.AtHospital;
            result.OvstKey = ovst.OvstKey;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!OvstExists(_HosGuid))
            {
                return NotFound();
            }      

            return NoContent();
        }


        private bool OvstExists(string _HosGuid)
        {
            return db.Ovsts.Any(e => e.HosGuid == _HosGuid);
        }

        [HttpDelete("{_HosGuid}")]
        public async Task<IActionResult> DeleteOvst(string _HosGuid)
        {
            var result = await db.Ovsts.FindAsync(_HosGuid);
            if (result == null)
            {
                return NotFound();
            }

            db.Ovsts.Remove(result);
            await db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public IActionResult GetOvstByVn (string _vn)
        {
            var query = 
                from a in db.Ovsts 
                where a.Vn == _vn
                select new
                {
                    a.HosGuid, a.Vn, a.Hn, a.An, a.Vstdate, a.Vsttime, a.Doctor, a.Hospmain, a.Hospsub, a.Oqueue, a.Ovstist, a.Ovstost, a.Pttype, a.Pttypeno, a.Rfrics, a.Rfrilct, a.Rfrocs, a.Rfrolct, a.Spclty, a.RcptDisease , a.Hcode, a.CurDep, a.CurDepBusy, a.LastDep, a.CurDepTime, a.RxQueue, a.DiagText, a.PtSubtype, a.MainDep, a.MainDepQueue, a.FinanceSummaryDate, a.VisitType, a.NodeId, a.ContractId, a.Waiting, a.RfriIcd10, a.OReferNumber, a.HasInsurance, a.IReferNumber, a.ReferType, a.OReferDep, a.Staff, a.CommandDoctor, a.SendPerson, a.PtPriority, a.FinanceLock, a.FinanceAlient, a.Oldcode, a.SignDoctor, a.AnonymousVisit, a.AnonymousVn, a.PtCapabilityTypeId, a.AtHospital, a.OvstKey
                };
                return Json(query.Take(1));
        }

         [HttpGet]
        public IActionResult GetOvstByHg (string _HosGuid)
        {
            var query = 
                from a in db.Ovsts 
                where a.HosGuid == _HosGuid
                select new
                {
                    a.HosGuid, a.Vn, a.Hn, a.An, a.Vstdate, a.Vsttime, a.Doctor, a.Hospmain, a.Hospsub, a.Oqueue, a.Ovstist, a.Ovstost, a.Pttype, a.Pttypeno, a.Rfrics, a.Rfrilct, a.Rfrocs, a.Rfrolct, a.Spclty, a.RcptDisease , a.Hcode, a.CurDep, a.CurDepBusy, a.LastDep, a.CurDepTime, a.RxQueue, a.DiagText, a.PtSubtype, a.MainDep, a.MainDepQueue, a.FinanceSummaryDate, a.VisitType, a.NodeId, a.ContractId, a.Waiting, a.RfriIcd10, a.OReferNumber, a.HasInsurance, a.IReferNumber, a.ReferType, a.OReferDep, a.Staff, a.CommandDoctor, a.SendPerson, a.PtPriority, a.FinanceLock, a.FinanceAlient, a.Oldcode, a.SignDoctor, a.AnonymousVisit, a.AnonymousVn, a.PtCapabilityTypeId, a.AtHospital, a.OvstKey
                };
                return Json(query.Take(1));
        }

        [HttpGet]
        public IActionResult GetDoctor()
        {
            //*SELECT * FROM `doctor` where licenseno like "%ว%"
            var query =
                from a in db.Doctors where a.Licenseno.Contains("ว") && !a.Name.Contains("(ยกเลิก)") && !a.Name.Contains("(ยกเลิกการใช้เนื่องจากส่งเบิกไม่ได้)") && !a.Name.Contains("เจ้าหน้าที่") && !a.Name.Contains("พท.ป") && !a.Licenseno.Contains("00000")
                select new 
                {
                    a.Name, a.Licenseno, a.Code
                };
                return Json(query.Take(1300));
        }

        [HttpGet]
        public IActionResult GetDoctorBySearchName(string name)
        {  
            var results = 
                from a in db.Doctors where a.Licenseno.Contains("ว") && !a.Name.Contains("(ยกเลิก)") && !a.Name.Contains("(ยกเลิกการใช้เนื่องจากส่งเบิกไม่ได้)") && !a.Name.Contains("เจ้าหน้าที่") && !a.Name.Contains("พท.ป") && !a.Licenseno.Contains("00000")
                &&  a.Name.Contains(name) || a.Licenseno.Contains(name)
                select new
                {
                    a.Name, a.Code
                };
                
            
            return Ok(results);
                
        }

        [HttpGet]
        public IActionResult GetHospitalSearchName(string name)
        {
            var results =
                from a in db.Hospcodes where a.Name.Contains(name) || a.Hospcode1.Contains(name)
                select new
                {
                    a.Hospcode1, a.Name
                };

            return Ok(results);
        }
         [HttpGet]
        public IActionResult GetOvstistAll()
        {
            var query =
                from a in db.Ovstists orderby a.Ovstist1
                select new
                {
                    a.Ovstist1, a.Name
                };
                return Ok(query.Take(20));
        }

        [HttpGet]
        public IActionResult GetOvstist(string name)
        {
            var query =
                from a in db.Ovstists where a.Name.Contains(name) || a.Ovstist1.Contains(name)
                select new
                {
                    a.Ovstist1, a.Name
                };
                return Ok(query.Take(20));
        }

        [HttpGet]
        public IActionResult GetOvstostAll()
        {
            var query =
                from a in db.Ovstosts orderby a.Ovstost1
                select new
                {
                    a.Ovstost1, a.Name
                };
                return Ok(query.Take(30));
        }

        [HttpGet]
        public IActionResult GetOvstost(string name)
        {
            var query =
                from a in db.Ovstosts where a.Ovstost1.Contains(name) || a.Name.Contains(name)
                select new
                {
                    a.Ovstost1, a.Name
                };
                return Ok(query.Take(30));
        }

        [HttpGet]
        public IActionResult GetPttypeAll()
        {
            var query =
                from a in db.Pttypes where !a.Name.Contains("(ยกเลิก)") orderby a.Pttype1
                select new
                {
                    a.Pttype1, a.Name
                };
                return Ok(query.Take(300));
        }

        [HttpGet]
        public IActionResult GetPttype(string name)
        {
            var query =
                from a in db.Pttypes where a.Pttype1.Contains(name) || a.Name.Contains(name) && !a.Name.Contains("(ยกเลิก)")
                select new
                {
                    a.Pttype1, a.Name
                };
                return Ok(query.Take(300));
        }

        [HttpGet]
        public IActionResult GetSpcltyAll()
        {
            var query =
                from a in db.Spclties orderby a.Spclty1
                select new
                {
                    a.Spclty1, a.Name
                };
                return Ok(query.Take(60));
        }

        [HttpGet]
        public IActionResult GetSpclty(string name)
        {
            var query =
                from a in db.Spclties where a.Spclty1.Contains(name) || a.Name.Contains(name)
                select new
                {
                    a.Spclty1, a.Name
                };
                return Ok(query.Take(60));
        }

        [HttpGet]
        public IActionResult GetOpdDepAll()
        {
            var query =
                from a in db.Kskdepartments where !a.Department.Contains("(ยกเลิก)") && !a.Department.Contains("(เลิกใข้)") orderby a.Depcode
                select new
                {
                    a.Depcode, a.Department
                };
                return Ok(query.Take(600));
        }

        [HttpGet]
        public IActionResult GetOpdDep(string name)
        {
            var query =
                from a in db.Kskdepartments where a.Depcode.Contains(name) || a.Department.Contains(name) && !a.Department.Contains("(ยกเลิก)") && !a.Department.Contains("(เลิกใข้)")
                select new
                {
                    a.Depcode, a.Department
                };
                return Ok(query.Take(60));
        }

         [HttpGet]
        public IActionResult genqn()
        {
            //var name = "ovst-q-630101";
            // DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Now);
            // var name = "ovst-q-" + dateOnly.ToString("yyMMdd");
            // var query =
            //     from a in db.Serials where a.Name.Contains(""+name+"")
            //     select new
            //     {
            //         a.Name, a.SerialNo
            //     };
            //     return Json(query.Take(1));

            var sqlQuery = "select get_serialnumber('ovst-q-611010')";

            var result = db.Serials
                .FromSqlRaw(sqlQuery)
                .Select(e => e.SerialNo)
                .FirstOrDefaultAsync();
            
            return Ok(result);
        }
    }