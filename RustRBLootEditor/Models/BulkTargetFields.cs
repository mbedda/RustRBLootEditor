using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RustRBLootEditor.Models
{
    public class BulkTargetFields : BindableBase
    {
        private bool _amountMin;
        public bool amountMin
        {
            get { return _amountMin; }
            set { SetProperty(ref _amountMin, value); }
        }

        private bool _amount;
        public bool amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private bool _probability;
        public bool probability
        {
            get { return _probability; }
            set { SetProperty(ref _probability, value); }
        }

        private bool _stacksize;
        public bool stacksize
        {
            get { return _stacksize; }
            set { SetProperty(ref _stacksize, value); }
        }
    }
}
