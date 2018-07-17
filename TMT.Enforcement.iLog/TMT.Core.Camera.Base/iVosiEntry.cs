using System;

namespace TMT.Core.Camera.Base
{
    public interface IVosiEntry
    {    
        //Actual Line Item
        string LineItem { get; set; }
        bool Extract(out string error);
                
        string Date { get; set; }
        string Time { get; set; }
        string LocationCode { get; set; }
        string VLN { get; set; }
        string Reason { get; set; }
    }
}