using RetroGameHandler.Handlers;
using RetroGameHandler.models;
using RetroGameHandler.TimeOnline;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RetroGameHandler.Views.Modals
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private DataModel data;

        public LoginView()
        {
            InitializeComponent();
            //this.DialogResult = false;
            this.Loaded += ((s, e) =>
            {
                data = LiteDBHelper.Load<DataModel>().FirstOrDefault() ?? new DataModel() { Id = 1, ScrapGuid = "", ScrapEmail = "" };
                email.Text = data.ScrapEmail;
                email2.Text = data.ScrapEmail;
            });
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (passwordRetype.Password != password2.Password)
            {
                txInfo.Text = "Password is not the same!";
                return;
            }
            var url = RGHSettings.ScrapPath + "register";
            var headers = new Dictionary<string, string>();
            headers.Add("email", email2.Text);
            headers.Add("password", password2.Password);
            headers.Add("secret", RGHSettings.ProgGuid);
            var info = JsonHandler.DownloadSerializedJsonData<Info>(url, headers);
            if (info.Error.Code > 299)
            {
                txInfo.Text = info.Error.Message;
                ErrorHandler.Warning(info.Error.Message);
            }
            else
            {
                txInfo.Text = info.Error.Message; ;
            }
            brdInfo.Visibility = Visibility.Visible;
            data.ScrapEmail = email.Text;
            data.ScrapGuid = info.Guid;
            LiteDBHelper.Save(data);
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (passwordRetype.Password != password2.Password)
            {
                txInfo.Text = "Password is not the same!";
                return;
            }
            var url = RGHSettings.ScrapPath + "changepassword";
            var headers = new Dictionary<string, string>();
            headers.Add("email", email2.Text);
            headers.Add("password", password2.Password);
            headers.Add("secret", RGHSettings.ProgGuid);
            var info = JsonHandler.DownloadSerializedJsonData<Info>(url, headers);
            if (info.Error.Code > 299)
            {
                txInfo.Text = info.Error.Message;
                ErrorHandler.Warning(info.Error.Message);
            }
            else
            {
                txInfo.Text = info.Error.Message; ;
            }
            brdInfo.Visibility = Visibility.Visible;
            data.ScrapEmail = email.Text;
            data.ScrapGuid = info.Guid;
            LiteDBHelper.Save(data);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var url = RGHSettings.ScrapPath + "login";
            var headers = new Dictionary<string, string>();
            headers.Add("email", email.Text);
            headers.Add("password", password.Password);
            headers.Add("secret", RGHSettings.ProgGuid);
            var info = JsonHandler.DownloadSerializedJsonData<Info>(url, headers);
            if (info.Error.Code == 200)
            {
                data.ScrapGuid = info.Guid;
                data.ScrapEmail = email.Text;
                LiteDBHelper.Save(data);
                this.DialogResult = true;
                ErrorHandler.Info(info.Error.Message);
                MessageBox.Show(info.Error.Message, "info", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            txInfo.Text = info.Error.Message;
            brdInfo.Visibility = Visibility.Visible;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnActivation_Click(object sender, RoutedEventArgs e)
        {
            var url = RGHSettings.ScrapPath + "activate";
            var headers = new Dictionary<string, string>();
            headers.Add("email", email2.Text);
            headers.Add("activationcode", acctivationCode.Text);
            headers.Add("secret", RGHSettings.ProgGuid);
            var info = JsonHandler.DownloadSerializedJsonData<Info>(url, headers);
            if (info.Error.Code == 200)
            {
                data.ScrapGuid = info.Guid;
                data.ScrapEmail = email.Text;
                ErrorHandler.Info(info.Error.Message);
                LiteDBHelper.Save(data);
                MessageBox.Show(info.Error.Message, "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            txInfo.Text = info.Error.Message;
        }

        public enum PasswordScore
        {
            Blank = 0,
            VeryWeak = 1,
            Weak = 2,
            Medium = 3,
            Strong = 4,
            VeryStrong = 5
        }

        public static PasswordScore CheckStrength(string password)
        {
            int score = 1;
            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 5)
                return PasswordScore.VeryWeak;
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?"))   //number only //"^\d+$" if you need to match more than one digit.
                score++;
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$")) //both, lower and upper case
                score++;
            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]")) //^[A-Z]+$
                score++;
            return (PasswordScore)score;
        }

        private void password2_KeyUp(object sender, KeyEventArgs e)
        {
            b1.Background = Brushes.Transparent;
            b2.Background = Brushes.Transparent;
            b3.Background = Brushes.Transparent;
            b4.Background = Brushes.Transparent;
            b5.Background = Brushes.Transparent;
            var ch = CheckStrength(password2.Password);
            tstatus.Text = ch.ToString();
            Register.IsEnabled = ch >= PasswordScore.Medium;
            ChangePassword.IsEnabled = Register.IsEnabled;
            switch (ch)
            {
                case PasswordScore.Blank:
                    //b1.Background = Brushes.Red;
                    break;

                case PasswordScore.VeryWeak:
                    b1.Background = Brushes.Red;
                    b2.Background = Brushes.Red;
                    break;

                case PasswordScore.Weak:
                    b1.Background = Brushes.Red;
                    b2.Background = Brushes.OrangeRed;
                    b3.Background = Brushes.Yellow;

                    break;

                case PasswordScore.Medium:
                    b1.Background = Brushes.Red;
                    b2.Background = Brushes.OrangeRed;
                    b3.Background = Brushes.Yellow;
                    b4.Background = Brushes.YellowGreen;
                    break;

                case PasswordScore.Strong:
                    b1.Background = Brushes.Red;
                    b2.Background = Brushes.OrangeRed;
                    b3.Background = Brushes.Yellow;
                    b4.Background = Brushes.YellowGreen;
                    b5.Background = Brushes.Green;
                    break;

                case PasswordScore.VeryStrong:
                    b1.Background = Brushes.Green;
                    b2.Background = Brushes.Green;
                    b3.Background = Brushes.Green;
                    b4.Background = Brushes.Green;
                    b5.Background = Brushes.Green;
                    break;
            }
        }
    }
}