using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RustRBLootEditor.UI
{
    /// <summary>
    /// Interaction logic for RustLoadingUC.xaml
    /// </summary>
    public partial class RustLoadingUC : UserControl
    {
        object SendBackParam;

        public event EventHandler Animation_Completed;

        public RustLoadingUC()
        {
            InitializeComponent();
        }

        public void BeginStoryboard(object sendBackParam)
        {
            SendBackParam = sendBackParam;
            (this.FindResource("loadingSB") as Storyboard).Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (this.Animation_Completed != null)
                this.Animation_Completed(SendBackParam, e);
        }
    }
}
