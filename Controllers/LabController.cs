using hosxpapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace hosxpapi.Controllers
{
    [Route("/api/[controller]/[action]")]
    public class LabController : Controller
    {
        private readonly ApplicationDbContext db;
        
        public LabController(ApplicationDbContext db)
        {
            this.db = db;
        }

        //         select lh.order_date, lh.form_name, lo.lab_order_number, (SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "144"
        //     LIMIT 1) AS BUN, (SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "209"
        //     LIMIT 1) AS Cr, (SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "1073"
        //     LIMIT 1) AS eGFR,	(SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "482"
        //     LIMIT 1) AS Na, (SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "430"
        //     LIMIT 1) AS K, (SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "178"
        //     LIMIT 1) AS Cl, (SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "197"
        //     LIMIT 1) AS CO2, (SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "133"
        //     LIMIT 1) AS FBS, (SELECT lo2.lab_order_result 
        //     FROM lab_order lo2 
        //     WHERE lo2.lab_order_number = lh.lab_order_number 
        //     AND lo2.lab_items_code = "892"
        //     LIMIT 1) AS HbA1C


        // from patient p

        // left outer join lab_head lh on lh.hn = p.hn
        // left outer join lab_order lo on lo.lab_order_number = lh.lab_order_number

        // where lh.confirm_report = "Y" and p.hn = "0810661" and lo.lab_items_code = "144" GROUP BY lh.lab_order_number order by lh.order_date  desc

        // limit 30

        [HttpGet]       
        public IActionResult getlab()
        {
            var result = from p in db.Patients
              join lh in db.LabHeads on p.Hn equals lh.Hn into lhGroup
              from lh in lhGroup.DefaultIfEmpty()
              join lo in db.LabOrders on lh.LabOrderNumber equals lo.LabOrderNumber into loGroup
              from lo in loGroup.DefaultIfEmpty()
              where lh.ConfirmReport == "Y" && p.Hn == "0810661"
              group lo by new { lh.OrderDate, lh.FormName, lo.LabOrderNumber } into g
              orderby g.Key.OrderDate descending
              select new
              {
                  OrderDate = g.Key.OrderDate,
                  FormName = g.Key.FormName,
                  LabOrderNumber = g.Key.LabOrderNumber,
                  BUN = g.Where(x => Convert.ToString(x.LabItemsCode) == "144").Select(x => x.LabOrderResult).FirstOrDefault(),
                  Cr = g.Where(x => Convert.ToString(x.LabItemsCode) == "209").Select(x => x.LabOrderResult).FirstOrDefault(),
                  eGFR = g.Where(x => Convert.ToString(x.LabItemsCode) == "1073").Select(x => x.LabOrderResult).FirstOrDefault(),
                  Na = g.Where(x => Convert.ToString(x.LabItemsCode) == "482").Select(x => x.LabOrderResult).FirstOrDefault(),
                  K = g.Where(x => Convert.ToString(x.LabItemsCode) == "430").Select(x => x.LabOrderResult).FirstOrDefault(),
                  Cl = g.Where(x => Convert.ToString(x.LabItemsCode) == "178").Select(x => x.LabOrderResult).FirstOrDefault(),
                  CO2 = g.Where(x => Convert.ToString(x.LabItemsCode) == "197").Select(x => x.LabOrderResult).FirstOrDefault(),
                  FBS = g.Where(x => Convert.ToString(x.LabItemsCode) == "133").Select(x => x.LabOrderResult).FirstOrDefault(),
                  HbA1C = g.Where(x => Convert.ToString(x.LabItemsCode) == "892").Select(x => x.LabOrderResult).FirstOrDefault()
              };
            //   .Take(30)
            //   .ToList();
            return Json(result.Take(30));
        }
    }
}