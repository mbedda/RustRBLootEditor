using Prism.Mvvm;
using RustRBLootEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace RustRBLootEditor.Models
{
    public class RustItems : BindableBase
    {
        public RustItems()
        {
            Items = new ObservableCollection<RustItem>(); 
        }

        private ObservableCollection<RustItem> _Items;
        public ObservableCollection<RustItem> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }

        public DLCsData DLCsData { get; set; }

        public RustItem GetRustItem(string shortname)
        {
            return Items.FirstOrDefault(s => s.shortName == shortname);
        }

        public async Task<bool> GetRustDLCs()
        {
            DLCsData = new DLCsData();

            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dlcsPath = Path.Combine(appPath, "Assets", "dlcs.json");

            DateTime dt = File.GetLastWriteTime(dlcsPath);

            bool testAPI = false;

            if (testAPI || (DateTime.Now - dt).TotalMinutes > 30 || !File.Exists(dlcsPath))
            {
                var dlcs = await SteamApi.GetDLCs();

                if (dlcs != null && dlcs.meta.statusCode == 200 && dlcs.data != null && dlcs.data.Count > 0)
                {
                    DLCsData.Data = new List<RustDLCData>(dlcs.data);
                    await Common.SaveJsonNewtonAsync(dlcs.data, dlcsPath, null, false);
                }
            }

            if (DLCsData.Data == null && File.Exists(dlcsPath))
                DLCsData.Data = await Common.LoadJsonAsync<List<RustDLCData>>(dlcsPath);

            foreach (var dlc in DLCsData.Data)
            {
                if ((!dlc.workshopId.HasValue || dlc.workshopId == 0) 
                    && !string.IsNullOrEmpty(dlc.itemShortName) 
                    && !DLCsData.DLCItems.Contains(dlc.itemShortName) && !_notDLCs.Contains(dlc.itemShortName))
                {
                    DLCsData.DLCItems.Add(dlc.itemShortName);
                }
                else if (dlc.workshopId.HasValue && dlc.workshopId != 0 && !DLCsData.ProhibitedSkins.Contains(dlc.workshopId.Value))
                {
                    DLCsData.ProhibitedSkins.Add(dlc.workshopId.Value);
                }
            }

            if(testAPI)
            {
                List<string> notInFPList = new List<string>();
                foreach (var carbonDLC in _carbonDLCShortnames)
                {
                    if (!DLCsData.DLCItems.Contains(carbonDLC))
                    {
                        notInFPList.Add(carbonDLC);
                    }
                }
                List<string> notInCarbonList = new List<string>();
                foreach (var fpDLC in DLCsData.DLCItems)
                {
                    if (!_carbonDLCShortnames.Contains(fpDLC))
                    {
                        notInCarbonList.Add(fpDLC);
                    }
                }
            }

            if (DLCsData.DLCItems == null || DLCsData.DLCItems.Count == 0 || DLCsData.DLCItems.Count < _carbonDLCShortnames.Count)
                DLCsData.DLCItems = _carbonDLCShortnames;

            return true;
        }

        public async Task Load(string steamPath)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string jsonPath = Path.Combine(appPath, "Assets", "items.json");

            List<RustItem> items = new();

            if (File.Exists(jsonPath))
                items = await Common.LoadJsonAsync<List<RustItem>>(jsonPath);

            await FetchNewItems(appPath, jsonPath, steamPath, items);

            if (DLCsData != null)
            {
                foreach (var item in items)
                {
                    if(DLCsData.DLCItems.Contains(item.shortName))
                        item.isDLC = true;
                }
            }

            await LoadImages(appPath, steamPath, items);

            await CheckForArmorSlotsSupport(items);

            Items = new ObservableCollection<RustItem>(items.OrderBy(x => x.displayName));
        }

        private readonly List<string> _blacklistedItems = new()
        {
            "ammo.snowballgun", "spraycandecal", "workcart", "wagon", "trike", "snowmobiletomaha", "submarinesolo", "snowmobile", "scraptransportheli.repair",
            "motorbike_sidecar", "motorbike", "mlrs", "minihelicopter.repair", "locomotive", "habrepair", "submarineduo", "blueprintbase",
            "bicycle", "attackhelicopter", "vehicle.chassis.2mod", "vehicle.chassis.3mod", "vehicle.chassis.4mod", "weaponrack.doublelight", "weaponrack.light",
            "oubreak_scientist", "gates.external.high.frontier", "wall.external.high.frontier", "clothing.mannequin"
        };

        private readonly List<string> _notDLCs = new()
        {
            "mask.bandana", "hat.cap", "shoes.boots", "deer.skull.mask", "hat.beenie", "mask.balaclava", "hoodie", "jacket", "tshirt.long",
            "metal.facemask", "pants", "jacket.snow", "box.wooden.large", "tshirt", "underwear", "spraycan", "buildingskin", "wallpaper.wall",
            "wallpaper.pack1", "gestures", "spraycandecal", "wallpaper.flooring", "wallpaper.ceiling", "floor.ceiling.wallpaper.pack1"
        };

        //should be removed when https://api.rusthelp.com/v1/facepunch/skins properly includes dlc items
        private readonly List<string> _carbonDLCShortnames = new()
        {
            "discord.trophy", "fogmachine", "strobelight", "snowmobiletomaha", "gates.external.high.adobe", "gates.external.high.legacy", "wall.external.high.adobe", "wall.external.high.legacy", "cocoknight.armor.gloves",
            "cocoknight.armor.helmet", "cocoknight.armor.pants", "cocoknight.armor.torso", "boots.frog", "draculacape", "draculamask", "frankensteinmask", "mummymask", "metal.facemask.hockey",
            "clatter.helmet", "knightsarmour.helmet", "hat.wellipets", "metal.facemask.icemask", "attire.ninja.suit", "knightsarmour.skirt", "hazmatsuit.arcticsuit", "hazmatsuit.diver", "hazmatsuit.frontier",
            "hazmatsuit.lumberjack", "hazmatsuit.nomadsuit", "hazmatsuit.spacesuit", "knighttorso.armour", "metal.plate.torso.icevest", "barricade.medieval", "chair.icethrone", "firework.boomer.blue",
            "firework.boomer.champagne", "firework.boomer.green", "firework.boomer.orange", "firework.boomer.pattern", "firework.boomer.red", "firework.boomer.violet", "firework.romancandle.blue", "firework.romancandle.green", "firework.romancandle.red",
            "firework.romancandle.violet", "firework.volcano", "firework.volcano.red", "firework.volcano.violet", "half.bamboo.shelves", "hazmat.plushy", "jackolantern.angry", "jackolantern.happy", "abyss.barrel.horizontal",
            "abyss.barrel.vertical", "wicker.barrel", "bamboo.barrel", "medieval.box.wooden.large", "legacyfurnace", "wall.frame.lunar2025_a", "wall.frame.lunar2025_b", "wall.frame.lunar2025_c", "sculpture.ice",
            "secretlabchair", "salvaged.bamboo.shelves", "sign.hanging.banner.large", "sign.hanging", "sign.hanging.ornate", "sign.pictureframe.landscape", "sign.pictureframe.portrait", "sign.pictureframe.tall", "sign.pictureframe.xl",
            "sign.pictureframe.xxl", "sign.pole.banner.large", "sign.post.double", "sign.post.single", "sign.post.town", "sign.post.town.roof", "sofa", "sofa.pattern", "cupboard.tool.retro",
            "cupboard.tool.shockbyte", "single.shallow.wall.shelves", "gunrack.horizontal", "gunrack.single.1.horizontal", "gunrack.single.2.horizontal", "gunrack.single.3.horizontal", "gunrack_stand", "gunrack_tall.horizontal", "gunrack_wide.horizontal",
            "hazmatyoutooz", "heavyscientistyoutooz", "fun.bass", "fun.cowbell", "drumkit", "fun.flute", "fun.jerrycanguitar", "piano", "fun.tambourine",
            "fun.trumpet", "fun.tuba", "xylophone", "chineselantern", "chineselanternwhite", "dragondoorknocker", "hat.dragonmask", "newyeargong", "hat.oxmask",
            "hat.rabbitmask", "hat.ratmask", "skylantern", "skylantern.skylantern.green", "skylantern.skylantern.orange", "skylantern.skylantern.purple", "skylantern.skylantern.red", "hat.snakemask", "hat.tigermask",
            "arcade.machine.chippy", "door.double.hinged.bardoors", "bathtub.planter", "fishtrophy", "huntingtrophylarge", "huntingtrophysmall", "minecart.planter", "rail.road.planter", "triangle.rail.road.planter",
            "rockingchair", "rockingchair.rockingchair2", "rockingchair.rockingchair3", "knife.skinning", "storage_barrel_a", "storage_barrel_b", "storage_barrel_c", "torchholder", "wantedposter.wantedposter2",
            "wantedposter.wantedposter3", "wantedposter.wantedposter4", "wantedposter", "hat.bunnyhat", "chicken.costume", "easterdoorwreath", "attire.egg.suit", "rustige_egg_a", "rustige_egg_b",
            "rustige_egg_c", "rustige_egg_d", "rustige_egg_e", "rustige_egg_f", "rustige_egg_g", "horse.costume", "attire.nesthat", "largecandles", "smallcandles",
            "carvable.pumpkin", "cursedcauldron", "scarecrow", "skullspikes.candles", "skullspikes.pumpkin", "skullspikes", "skulldoorknocker", "skull_fire_pit", "spookyspeaker",
            "halloween.surgeonsuit", "skull.trophy.jar", "skull.trophy.jar2", "skull.trophy.table", "skull.trophy", "medieval.door.double.hinged.metal", "medieval.door.hinged.metal", "movembermoustachecard", "movembermoustache",
            "factorydoor", "industrial.wall.light.blue", "industrial.wall.light.green", "industrial.wall.light", "industrial.wall.light.red", "abovegroundpool", "beachchair", "beachparasol", "beachtable",
            "beachtowel", "boogieboard", "innertube", "innertube.horse", "innertube.unicorn", "tool.instant_camera", "paddlingpool", "photoframe.landscape", "photoframe.large",
            "photoframe.portrait", "sunglasses02black", "sunglasses02camo", "sunglasses02red", "sunglasses03black", "sunglasses03chrome", "sunglasses03gold", "sunglasses", "gun.water",
            "pistol.water", "trophy", "trophy2023", "twitchsunglasses", "twitch.headset", "hobobarrel", "door.hinged.industrial.a", "twitchrivals2023desk", "xmas.lightstring",
            "xmas.door.garland", "xmas.double.door.garland", "giantcandycanedecor", "giantlollipops", "sign.neon.125x125", "sign.neon.125x215.animated", "sign.neon.125x215", "sign.neon.xl.animated", "sign.neon.xl",
            "xmas.lightstring.advanced", "snowmachine", "snowman", "santabeard", "attire.snowman.helmet", "xmas.window.garland", "xmasdoorwreath", "concretehatchet", "concretepickaxe",
            "lumberjack.hatchet", "lumberjack.pickaxe", "vehicle.car_radio", "boombox", "fun.boomboxportable", "cassette", "cassette.medium", "cassette.short", "fun.casetterecorder",
            "discoball", "discofloor", "discofloor.largetiles", "connected.speaker", "laserlight", "megaphone", "microphonestand", "mobilephone", "soundlight",
            "rifle.ak.diver", "rifle.ak.ice", "rifle.ak.jungle", "rifle.ak.med", "blunderbuss", "knife.bone.obsidian", "spear.cny", "diverhatchet", "diverpickaxe",
            "divertorch", "frontier_hatchet", "mace.baseballbat", "torch.torch.skull", "skull", "sunken.knife", "legacy bow", "jungle.rock", "rocket.launcher.dragon",
            "toolgun", "hazmatsuit.pilot", "pilot.hazmat.box.wooden", "lock.code.a.pilot", "chair.ejector.seat", "pistol.semiauto.a.m15"
        };


        private async Task FetchNewItems(string appPath, string jsonPath, string steamPath, List<RustItem> currentItems)
        {
            if (string.IsNullOrEmpty(steamPath)) return;

            string itemsDirectory = Path.Combine(steamPath, "steamapps\\common\\Rust\\Bundles\\items");

            if (!Directory.Exists(itemsDirectory)) return;

            var itemFiles = Directory.EnumerateFiles(itemsDirectory, "*.png");

            bool newItemsFound = false;

            foreach (var item in itemFiles)
            {
                string shortname = Path.GetFileNameWithoutExtension(item);
                
                if(_blacklistedItems.Contains(shortname)) continue;

                if (!currentItems.Any(s => s.shortName == shortname) && File.Exists(item.Replace(".png", ".json")))
                {
                    BundleItem bundleItem = await Common.LoadJsonAsync<BundleItem>(item.Replace(".png", ".json"));

                    if (string.IsNullOrEmpty(bundleItem.Name) || bundleItem.ItemType == "Liquid") continue;
                    
                    await ResizeAndSaveImageFromSteam(appPath, shortname, steamPath);

                    currentItems.Add(new RustItem()
                    {
                        shortName = bundleItem.shortname,
                        category = bundleItem.Category,
                        displayName = bundleItem.Name
                    });

                    newItemsFound = true;
                }
            }

            if (newItemsFound)
            {
                await Common.SaveJsonNewtonAsync(currentItems, jsonPath);
            }
        }

        private async Task ResizeAndSaveImageFromSteam(string appPath, string shortname, string steamPath)
        {
            string itempath = Path.Combine(appPath, "Assets", "RustItems", $"{shortname}.png");
            if (File.Exists(itempath)) return;

            string itemSteamPath = Path.Combine(steamPath, $"steamapps\\common\\Rust\\Bundles\\items\\{shortname}.png");
            if (!File.Exists(itemSteamPath)) return;

            await using FileStream fs = new FileStream(itemSteamPath, FileMode.Open);
            using Image source = new Bitmap(fs);
            using Image destination = new Bitmap(100, 100);

            using (var g = Graphics.FromImage(destination))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(source, new Rectangle(0, 0, destination.Width, destination.Height), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }
            destination.Save(itempath, ImageFormat.Png);
        }

        private async Task LoadImages(string appPath, string steampath, List<RustItem> currentItems)
        {
            foreach (var item in currentItems)
            {
                string itemImagePath = Path.Combine(appPath, "Assets", "RustItems", $"{item.shortName}.png");

                BitmapImage? noImage = new BitmapImage(new Uri("/RustRBLootEditor;component/Assets/unavailable.png", UriKind.Relative));

                if (!File.Exists(itemImagePath))
                {
                    await ResizeAndSaveImageFromSteam(appPath, item.shortName, steampath);
                }

                if (File.Exists(itemImagePath))
                {
                    item.ImageSource = await Task.Run(() =>
                    {
                        var fileStream = new FileStream(itemImagePath, FileMode.Open, FileAccess.Read);
                        try
                        {
                            return BitmapFrame.Create(fileStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        }
                        finally
                        {
                            fileStream.Dispose();
                        }
                    });
                }
                else
                {
                    item.ImageSource = noImage;
                }
            }
        }

        private async Task CheckForArmorSlotsSupport(List<RustItem> currentItems)
        {
            foreach (var item in currentItems)
            {
                if(ArmorSlotsCompatabileItems.TryGetValue(item.shortName, out RustItem.ArmorSlots armorSlots))
                    item.Slots = armorSlots;
                else
                    item.Slots = null;
            }
        }

        [IgnoreDataMember]
        public static readonly Dictionary<string, RustItem.ArmorSlots> ArmorSlotsCompatabileItems = new()
        {
            { "cocoknight.armor.pants", new(1, 3) },
            { "cocoknight.armor.torso", new(1, 3) },
            { "metal.facemask.hockey", new(1, 1) },
            { "bucket.helmet", new(1, 3) },
            { "coffeecan.helmet", new(1, 1) },
            { "deer.skull.mask", new(1, 1) },
            { "knightsarmour.helmet", new(1, 1) },
            { "riot.helmet", new(2, 3) },
            { "bone.armor.suit", new(1, 3) },
            { "jacket", new(1, 3) },
            { "wood.armor.jacket", new(1, 3) },
            { "metal.facemask", new(1, 1) },
            { "metal.facemask.icemask", new(1, 1) },
            { "knightsarmour.skirt", new(1, 3) },
            { "roadsign.kilt", new(1, 3) },
            { "wood.armor.pants", new(1, 3) },
            { "attire.hide.poncho", new(1, 3) },
            { "hazmatsuit.arcticsuit", new(1, 3) },
            { "hazmatsuit.diver", new(1, 3) },
            { "hazmatsuit.frontier", new(1, 3) },
            { "hazmatsuit", new(1, 3) },
            { "hazmatsuit.lumberjack", new(1, 3) },
            { "hazmatsuit.nomadsuit", new(1, 3) },
            { "hazmatsuit.spacesuit", new(1, 3) },
            { "knighttorso.armour", new(1, 3) },
            { "metal.plate.torso", new(1, 3) },
            { "metal.plate.torso.icevest", new(1, 3) },
            { "roadsign.jacket", new(1, 3) }
        };
    }

    public class DLCsData
    {
        public List<RustDLCData> Data { get; set; }
        public List<string> DLCItems { get; set; } = new List<string>();
        public List<ulong> ProhibitedSkins { get; set; } = new List<ulong>();
    }

    public class RustItem : BindableBase
    {
        private string _displayName;
        public string displayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        private string _shortName;
        public string shortName
        {
            get { return _shortName; }
            set { SetProperty(ref _shortName, value); }
        }

        private string _category;
        public string category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        [IgnoreDataMember]
        public bool isDLC { get; set; }

        [IgnoreDataMember]
        public ImageSource? ImageSource { get; set; }


        [IgnoreDataMember]
        public ArmorSlots Slots { get; set; }

        public class ArmorSlots
        {
            public ArmorSlots(int min = 1, int max = 1)
            {
                this.min = min;
                this.max = max;
            }

            public int min { get; set; }
            public int max { get; set; }
        }
    }
}
