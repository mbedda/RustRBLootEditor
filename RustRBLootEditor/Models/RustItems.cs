using HtmlAgilityPack;
using Prism.Mvvm;
using RustRBLootEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RustRBLootEditor.Models
{
    [DataContract]
    public class RustItems
    {
        public RustItems()
        {
            Items = new List<RustItem>(); 
            Load();
        }

        [DataMember]
        public List<RustItem> Items { get; set; }

        public RustItem GetRustItem(string shortname)
        {
            return Items.FirstOrDefault(s => s.shortName == shortname);
        }

        public void Load()
        {
            Items = Common.LoadJsonResource<List<RustItem>>("RustRBLootEditor.Assets.items.json");
        }

        public RustItem GetItemDetailsByShortname(string shortname)
        {
            return Items.FirstOrDefault(s => s.shortName == shortname);
        }
    }

    [DataContract]
    public class RustItem
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public Uri image { get; set; }
        [DataMember]
        public string displayName { get; set; }
        [DataMember]
        public string shortName { get; set; }
        [DataMember]
        public string category { get; set; }
    }
}
