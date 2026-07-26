using System.Collections.Generic;
using System.Collections.ObjectModel;
using RustRBLootEditor.Models;
using RustRBLootEditor.ViewModels;
using Xunit;

namespace RustRBLootEditor.Tests
{
    public class ValidateLootTableTests
    {
        [Fact]
        public void ValidateLootTable_NullLootTableFile_ReturnsEmptyWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = null!;

            var warnings = vm.ValidateLootTable();

            Assert.Single(warnings);
            Assert.Contains("The loot table is empty.", warnings[0]);
        }

        [Fact]
        public void ValidateLootTable_EmptyLootItems_ReturnsEmptyWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>()
            };

            var warnings = vm.ValidateLootTable();

            Assert.Single(warnings);
            Assert.Contains("The loot table is empty.", warnings[0]);
        }

        [Fact]
        public void ValidateLootTable_AllNullItems_ReturnsNoValidEntriesWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>() { null!, null! }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("The loot table contains no valid entries."));
        }

        [Fact]
        public void ValidateLootTable_AllProbabilitiesZero_ReturnsZeroProbabilityWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "rifle.ak", probability = 0f, amountMin = 1, amount = 1, stacksize = 1 },
                    new LootItem { shortname = "ammo.rifle", probability = 0f, amountMin = 1, amount = 1, stacksize = 1 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("Every probability is 0"));
        }

        [Fact]
        public void ValidateLootTable_AllStackSizesZero_ReturnsZeroStackSizeWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "rifle.ak", probability = 0.5f, amountMin = 1, amount = 1, stacksize = 0 },
                    new LootItem { shortname = "ammo.rifle", probability = 0.5f, amountMin = 1, amount = 1, stacksize = 0 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("Every stacksize is 0"));
        }

        [Fact]
        public void ValidateLootTable_MissingShortname_ReturnsNoShortnameWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "", probability = 0.5f, amountMin = 1, amount = 1, stacksize = 1 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("has no shortname."));
        }

        [Fact]
        public void ValidateLootTable_ProbabilityOutOfRange_ReturnsProbabilityWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "scrap", probability = 1.5f, amountMin = 1, amount = 1, stacksize = 10 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("Probability must be between 0 and 1"));
        }

        [Fact]
        public void ValidateLootTable_ZeroAmount_ReturnsZeroAmountWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "scrap", probability = 0.5f, amountMin = 0, amount = 0, stacksize = 100 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("has amount 0 and will not be loaded."));
        }

        [Fact]
        public void ValidateLootTable_NegativeAmountMin_ReturnsNegativeAmountMinWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "wood", probability = 0.5f, amountMin = -5, amount = 10, stacksize = 1000 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("has amountMin -5 and can roll a non-positive amount."));
        }

        [Fact]
        public void ValidateLootTable_AmountBelowAmountMin_ReturnsAmountBelowWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "stone", probability = 0.5f, amountMin = 100, amount = 50, stacksize = 1000 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("has amount 50 below amountMin 100"));
        }

        [Fact]
        public void ValidateLootTable_ExcessiveStackCount_ReturnsStackSizeWarning()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "wood", probability = 0.5f, amountMin = 100, amount = 1000, stacksize = 50 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Contains(warnings, w => w.Contains("has stacksize 50 set too low"));
        }

        [Fact]
        public void ValidateLootTable_ValidLootTable_ReturnsNoWarnings()
        {
            var vm = new MainViewModel();
            vm.LootTableFile = new LootTableFile
            {
                LootItems = new ObservableCollection<LootItem>
                {
                    new LootItem { shortname = "scrap", probability = 0.5f, amountMin = 10, amount = 100, stacksize = 1000 },
                    new LootItem { shortname = "metal.fragments", probability = 0.8f, amountMin = 100, amount = 500, stacksize = 5000 }
                }
            };

            var warnings = vm.ValidateLootTable();

            Assert.Empty(warnings);
        }
    }
}
