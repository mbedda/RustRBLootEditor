using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            LootItems = new ObservableCollection<LootItem>();
        }

        internal void DoSort()
        {
            LootItems = new ObservableCollection<LootItem>(LootItems.OrderBy(x => x.displayName));
        }

        private ObservableCollection<LootItem> lootItems;
        public ObservableCollection<LootItem> LootItems
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
            probability = 1.0f;
            stacksize = -1;
        }

        private string _shortname;
        public string shortname
        {
            get { return _shortname; }
            set { SetProperty(ref _shortname, value); }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private long _amount;
        public long amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private ulong _skin;
        public ulong skin
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

        private float _probability;
        public float probability
        {
            get { return _probability; }
            set { SetProperty(ref _probability, value); }
        }

        private long _stacksize;
        public long stacksize
        {
            get { return _stacksize; }
            set { SetProperty(ref _stacksize, value); }
        }

        [IgnoreDataMember]
        private string _category;
        [IgnoreDataMember]
        public string category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        [IgnoreDataMember]
        private string _displayName; 
        [IgnoreDataMember]
        public string displayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }
    }

}
