using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstDayPractice.Day4
{

    public class AllDataViewModel
    {
        public ObservableCollection<BonusDataModel> BonusList { get; set; }
        public ObservableCollection<KitDataModel> KitList { get; set; }

        public AllDataViewModel()
        {
            BonusList = new ObservableCollection<BonusDataModel>();
            //BonusList = new ObservableCollection<BonusDataModel>
            //{
            //    new BonusDataModel
            //    {
            //        DateFrom = DateTime.Now,
            //        DateTo = DateTime.Now.AddDays(10),
            //        InActive = false,
            //        SlabWise = true,
            //        Quantity = 5.0,
            //        BonusFree = 1.0
            //    }
            //};

            KitList = new ObservableCollection<KitDataModel>();
            //KitList = new ObservableCollection<KitDataModel>
            //{
            //    new KitDataModel
            //    {
            //        Item = "fadsf1",
            //        Godown = "1afdas",
            //        AltQty = 0.0,
            //        UOM1 = "fadsf1",
            //        Qty = 0.0,
            //        UOM2 = "1af",
            //        Description = "1asdf"


            //    }
            //};
        }
    }
}
