using System;

namespace Kapsch.ITS.Reports.Operational.Models
{
    public class NoticeBeforeSummonsModelAG
    {
        public string regulation { get; set; }
        //public string Off_Code_Regulation { get; set; }
        public DateTime Off_Reg_Infringement_Date { get; set; }

        public string Location_Description { get; set; }
        public string Off_Reg_Ticket_No { get; set; }
        public string Location_Code { get; set; }
        public string CamReference { get; set; }
        public string Ticket_type { get; set; }
        public string Person_Name { get; set; }
        public string Company_text { get; set; }
        public object Charge_decription { get; set; }
        public DateTime Off_Reg_Guilt_Fine_Exp_Date { get; set; }
        public string Fine_amount { get; set; }
        public string Court_Name { get; set; }
        public string Payment_ref { get; set; }
        public string Regulation_text { get; set; }
        public int Charge_code { get; set; }
        public string Insp_no { get; set; }
        public string Issued_by_line1 { get; set; }
        public string Issued_by_line2 { get; set; }
        public string Issued_by_line3 { get; set; }
        public DateTime Issued_date { get; set; }
        public string Personal_details1 { get; set; }
        public string Personal_details2 { get; set; }
        public string Personal_details3 { get; set; }
        public string Bank_for_payments { get; set; }
        public string Account_name { get; set; }
        public long Acc_number { get; set; }
        public DateTime Date_Of_Offence { get; set; }
        public string Time_Of_Offence { get; set; }
        public string Location { get; set; }
        public string Zone { get; set; }
        public string Speed { get; set; }
        public string Veh_Registration { get; set; }
        public long ID_No { get; set; }
        public string Officer { get; set; }
        public string Vehicle_Brand { get; set; }
        public string Vehicle_Type { get; set; }
        public byte[] Vehicle_Image { get; set; }
        public byte[] Vehicle_number_plate { get; set; }
        public byte[] qr_code { get; set; }
    }
}
