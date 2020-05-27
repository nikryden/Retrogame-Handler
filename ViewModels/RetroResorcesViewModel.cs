using RetroGameHandler.Handlers;
using RetroGameHandler.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;


namespace RetroGameHandler.ViewModels
{
    public class RetroResorcesViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public RetroResorcesViewModel()
        {
            var lnk = JsonHandler.DownloadSerializedJsonData<LinksModel>("https://timeonline.se/RGHandler/resources.json");
          if(lnk.Links!=null) Links = new ObservableCollection<Link>(lnk.Links);
          if (lnk.Categories != null) Categories = new ObservableCollection<Category>(lnk.Categories);
        }
        public ObservableCollection<Link> Links { get; set; }
        public static ObservableCollection<Category> Categories { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
   
}
