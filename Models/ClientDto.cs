namespace hosxpapi.Models
{
    public class ClientDto
    {
        public string vn { get; set; }
        public string hn { get; set; }
        public object an { get; set; }
        public string vstdate { get; set; } 
        public string vsttime { get; set; } 
        public string doctor { get; set; }
        public string hospmain { get; set; }
        public string hospsub { get; set; }
        public int? oqueue { get; set; } 
        public string ovstist { get; set; }
        public string ovstost { get; set; }
        public string pttype { get; set; }
        public string pttypeno { get; set; }
        public object rfrics { get; set; }
        public object rfrilct { get; set; } 
        public string rfrocs { get; set; } 
        public string rfrolct { get; set; } 
        public string spclty { get; set; }
        public object rcptDisease { get; set; } 
        public string hcode { get; set; }
        public string curDep { get; set; }
        public string curDepBusy { get; set; }
        public string lastDep { get; set; }
        public string curDepTime { get; set; } 
        public object rxQueue { get; set; } 
        public object diagText { get; set; } 
        public int ptSubtype { get; set; }
        public string mainDep { get; set; }
        public int mainDepQueue { get; set; }
        public object financeSummaryDate { get; set; } 
        public string visitType { get; set; }
        public string nodeId { get; set; } 
        public object contractId { get; set; } 
        public string waiting { get; set; } 
        public object rfriIcd10 { get; set; } 
        public object oReferNumber { get; set; } 
        public string hasInsurance { get; set; } 
        public object iReferNumber { get; set; } 
        public object referType { get; set; } 
        public object oReferDep { get; set; } 
        public string staff { get; set; } 
        public object commandDoctor { get; set; } 
        public object sendPerson { get; set; } 
        public int ptPriority { get; set; } 
        public object financeLock { get; set; } 
        public object financeAlient { get; set; } 
        public object oldcode { get; set; } 
        public object signDoctor { get; set; } 
        public object anonymousVisit { get; set; } 
        public object anonymousVn { get; set; } 
        public object ptCapabilityTypeId { get; set; } 
        public object atHospital { get; set; } 
        public string ovstKey { get; set; }
    }
}