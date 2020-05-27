using System;

namespace RetroGameHandler.ViewModels
{
    public class BaseViewModel
    {
        public static implicit operator BaseViewModel(Views.RetroResorcesView v)
        {
            throw new NotImplementedException();
        }
    }
}