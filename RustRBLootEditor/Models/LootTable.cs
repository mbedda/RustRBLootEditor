using Newtonsoft.Json;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

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

        private ArmorSlots _slots;
        [JsonProperty(Order = 0, PropertyName = "armor module slots", NullValueHandling = NullValueHandling.Ignore)]
        public ArmorSlots slots
        {
            get { return _slots; }
            set { SetProperty(ref _slots, value); }
        }

        private string _shortname;
        [JsonProperty(Order = 1)]
        public string shortname
        {
            get { return _shortname; }
            set { SetProperty(ref _shortname, value); }
        }

        private string _name;
        [JsonProperty(Order = 2)]
        public string name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private bool _blueprint;
        [JsonProperty(Order = 3)]
        public bool blueprint
        {
            get { return _blueprint; }
            set { SetProperty(ref _blueprint, value); }
        }

        private ulong _skin;
        [JsonProperty(Order = 4)]
        public ulong skin
        {
            get { return _skin; }
            set { SetProperty(ref _skin, value); }
        }

        private long _amount;
        [JsonProperty(Order = 5)]
        public long amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private long _amountMin;
        [JsonProperty(Order = 6)]
        public long amountMin
        {
            get { return _amountMin; }
            set { SetProperty(ref _amountMin, value); }
        }

        private float _probability;
        [JsonProperty(Order = 7)]
        public float probability
        {
            get { return _probability; }
            set { SetProperty(ref _probability, value); }
        }

        private long _stacksize;
        [JsonProperty(Order = 8)]
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

        [IgnoreDataMember]
        private bool _isDLC;
        [IgnoreDataMember]
        public bool isDLC
        {
            get { return _isDLC; }
            set { SetProperty(ref _isDLC, value); }
        }

        public class ArmorSlots : BindableBase
        {
            public ArmorSlots()
            {
                min = 1;
                max = 1;
            }

            private int _min;
            [JsonProperty(Order = 1)]
            public int min
            {
                get { return _min; }
                set { SetProperty(ref _min, value); }
            }

            private int _max;
            [JsonProperty(Order = 2)]
            public int max
            {
                get { return _max; }
                set { SetProperty(ref _max, value); }
            }
        }
    }
}
