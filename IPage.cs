using RetroGameHandler.Handlers;
using RetroGameHandler.ViewModels;

namespace RetroGameHandler
{
    public interface IPage
    {
        BaseViewModel ViewModel { get; set; }
        PageControllHandler PageControllHandler { get; set; }
        string Title { get; set; }
    }
}