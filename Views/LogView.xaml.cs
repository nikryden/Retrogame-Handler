using RetroGameHandler.Entities;
using RetroGameHandler.Handlers;
using RetroGameHandler.Helpers;
using RetroGameHandler.ViewModels;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RetroGameHandler.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, IPage
    {
        public PageControllHandler PageControllHandler { get; set; } = new PageControllHandler();
        public string Title { get; set; } = "StartPage";
        public BaseViewModel ViewModel { get; set; } = new LogViewModel();
        private LogViewModel logViewModel { get; set; }

        public LogView()
        {
            InitializeComponent();
            var LogTypes = Enum.GetValues(typeof(ErrorLevel)).Cast<object>().ToList().Select(s => s.ToString()).ToList();
            LogTypes.Insert(0, "ALL");
            LogType.ItemsSource = LogTypes;
            //ErrorHandler.Error(new Exception("Test if ok"));
            var logList = ErrorHandler.GetAllErrors()?.ToList().OrderByDescending(i => i.DateTime).Take(100);
            LogInfo.ItemsSource = logList;
            logViewModel = (LogViewModel)ViewModel;
            this.DataContext = ViewModel;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            ErrorLevel? enumType = null;
            var selectedType = logViewModel.SelectedLogType;
            if (cbxLogType.IsChecked.Value && selectedType != "ALL")
            {
                ErrorLevel TempenumType;
                if (Enum.TryParse(selectedType, out TempenumType)) enumType = TempenumType;
            }
            DateTime? dtFrom = null;
            if (cbxFrom.IsChecked.Value)
            {
                dtFrom = logViewModel.DateTimeFrom;
            }
            DateTime? dtTo = null;
            if (cbxTo.IsChecked.Value)
            {
                dtTo = logViewModel.DateTimeTo;
            }
            string searchText = null;
            if (cbxSerchText.IsChecked.Value)
            {
                searchText = logViewModel.SearchText;
            }
            LogInfo.ItemsSource = ErrorHandler.GetErrors(
                enumType,
                dtFrom,
                dtTo,
                searchText,
                searchText,
                searchText
                ).ToList().Take(100);
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Do you like to clear the log?\nCan't be undone!",
                "Clear", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No) return;
            ErrorHandler.ClearLog();
            LogInfo.ItemsSource = ErrorHandler.GetAllErrors().ToList();
        }
    }
}