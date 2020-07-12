using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroGameHandler.ViewModels
{
    internal class LogViewModel : BaseViewModel
    {
        public LogViewModel()
        {
            DateTimeFrom = DateTime.Now.AddDays(-1);
            DateTimeTo = DateTime.Now;
            SelectedLogType = "ALL";
        }

        private DateTime _dateTimeFrom;
        private DateTime _dateTimeTo;
        private string _selectedLogType;
        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                NotifyPropertyChanged();
            }
        }

        public string SelectedLogType
        {
            get => _selectedLogType;
            set
            {
                _selectedLogType = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime DateTimeFrom
        {
            get => _dateTimeFrom;
            set
            {
                _dateTimeFrom = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime DateTimeTo
        {
            get => _dateTimeTo;
            set
            {
                _dateTimeTo = value;
                NotifyPropertyChanged();
            }
        }
    }
}