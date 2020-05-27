using RetroGameHandler.Handlers;
using RetroGameHandler.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for RetroResorces.xaml
    /// </summary>
    public partial class RetroResorcesView : UserControl,IPage
    {
        public RetroResorcesView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "Resorces";
        public BaseViewModel ViewModel { get; set; } = new RetroResorcesViewModel();

        private void nvgUri_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}
