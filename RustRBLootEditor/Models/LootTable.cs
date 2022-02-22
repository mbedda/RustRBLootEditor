using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RustRBLootEditor.Models
{
    public class LootTableFile : BindableBase
    {
        public LootTableFile()
        {
            LootItems = new List<LootItem>();
        }

        private List<LootItem> lootItems;
        public List<LootItem> LootItems
        {
            get { return lootItems; }
            set { SetProperty(ref lootItems, value); }
        }
    }

    public class LootItem : BindableBase
    {
        public LootItem()
        {
            skin = 0;
            amount = 0;
            amountMin = 0;
            category = "Misc";
        }

        private string _shortname;
        public string shortname
        {
            get { return _shortname; }
            set { SetProperty(ref _shortname, value); }
        }

        private long _amount;
        public long amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private long _skin;
        public long skin
        {
            get { return _skin; }
            set { SetProperty(ref _skin, value); }
        }

        private long _amountMin;
        public long amountMin
        {
            get { return _amountMin; }
            set { SetProperty(ref _amountMin, value); }
        }

        [IgnoreDataMember]
        public string category { get; set; }
        [IgnoreDataMember]
        public string displayName { get; set; }
    }

}
