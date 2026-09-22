using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstDayPractice.Day4
{
    public class BonusDataModel
    {
        public DateTime? DateFrom { get; set; } = null;
        public DateTime? DateTo { get; set; } = null;
        public bool InActive { get; set; } = false;
        public bool SlabWise { get; set; } = false;
        public double Quantity { get; set; } = 0.00;
        public double BonusFree { get; set; } = 0.00;
    }

    public class KitDataModel
    {
        public string Item { get; set; }
        public string Godown { get; set; }
        public double AltQty { get; set; }
        public string UOM1 { get; set; }
        public double Qty { get; set; }
        public string UOM2 { get; set; }
        public string Description { get; set; }


    }
}
