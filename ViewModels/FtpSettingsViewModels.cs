using RetroGameHandler.Handlers;

namespace RetroGameHandler.ViewModels
{
    internal class FtpSettingsViewModels : BaseViewModel
    {
        private FtpHelper _ftp = new FtpHelper();
        private string _host = "10.1.1.2";
        private int _port = 21;
        public FtpHelper FTP { get => _ftp; }

        //public async void ChangePath(string path)
        //{
        //    await FTP.ChangeDirectoryAsync(path);
        //}

        //public async Task<IEnumerable<FtpListItem>> Connect()
        //{
        //    //await FTP.ConnectAsync();
        //    //return await FTP.ListAsync();
        //}
    }
}