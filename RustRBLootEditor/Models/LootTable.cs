using Newtonsoft.Json;
using Prism.Mvvm;
using System;
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

        public static List<LootItem> FromBoatLootItems(List<BoatLootItem> boatLootItems)
        {
            return boatLootItems.Select(x =>
            {
                LootItem lootItem = new LootItem();
                lootItem.FromBoatLootItem(x);
                return lootItem;
            }).ToList();
        }

        public static List<BoatLootItem> ToBoatLootItems(List<LootItem> lootItems)
        {
            return lootItems.Select(x =>
            {
                BoatLootItem boatLootItem = new BoatLootItem();
                boatLootItem.FromLootItem(x);
                return boatLootItem;
            }).ToList();
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

        public void FromBoatLootItem(BoatLootItem boatLootItem)
        {
            shortname = boatLootItem.shortname;
            amountMin = boatLootItem.amountMin;
            amount = boatLootItem.amountMax;
            probability = MathF.Round(boatLootItem.chance / 100f, 2);
            skin = boatLootItem.skin;
            blueprint = boatLootItem.blueprint;
            name = boatLootItem.name;
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

        private int _amount;
        [JsonProperty(Order = 5)]
        public int amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private int _amountMin;
        [JsonProperty(Order = 6)]
        public int amountMin
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

        private int _stacksize;
        [JsonProperty(Order = 8)]
        public int stacksize
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

    public class BoatLootItem
    {
        public void FromLootItem(LootItem lootItem)
        {
            shortname = lootItem.shortname;
            amountMin = lootItem.amountMin;
            amountMax = lootItem.amount;
            chance = MathF.Round(lootItem.probability * 100f, 2);
            skin = lootItem.skin;
            blueprint = lootItem.blueprint;
            name = lootItem.name;
        }

        [JsonProperty(Order = 0, PropertyName = "Item Shortname")]
        public string shortname { get; set; } = string.Empty;

        [JsonProperty(Order = 1, PropertyName = "Minimum Amount")]
        public int amountMin { get; set; } = 0;

        [JsonProperty(Order = 2, PropertyName = "Maximum Amount")]
        public int amountMax { get; set; } = 0;

        [JsonProperty(Order = 3, PropertyName = "Drop Chance Percent (0 - 100)")]
        public float chance { get; set; } = 100.0f;

        [JsonProperty(Order = 4, PropertyName = "Skin ID (0 = Default)")]
        public ulong skin { get; set; } = 0;

        [JsonProperty(Order = 5, PropertyName = "Is Blueprint")]
        public bool blueprint { get; set; }

        [JsonProperty(Order = 6, PropertyName = "Custom Item Display Name (Leave Empty For Default)")]
        public string name { get; set; }
    }
}
