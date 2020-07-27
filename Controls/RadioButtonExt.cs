using System.Windows;
using System.Windows.Controls;

namespace RetroGameHandler.Controls
{
    internal class RadioButtonExt : RadioButton
    {
        public static int WasChecked { get; set; }

        public bool? IsCheckedChanged
        {
            get { return (bool?)GetValue(IsCheckedChangedProperty); }
            set { SetValue(IsCheckedChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChanged.
        //This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedChangedProperty =
              DependencyProperty.Register("IsChanged", typeof(bool?),
                typeof(RadioButtonExt),
            new FrameworkPropertyMetadata(false,
            FrameworkPropertyMetadataOptions.Journal |
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
              CheckedChanged));

        public static void CheckedChanged(DependencyObject d,
                 DependencyPropertyChangedEventArgs e)
        {
            ((RadioButtonExt)d).IsChecked = (bool)e.NewValue;
        }

        public RadioButtonExt()
        {
            this.Checked += new RoutedEventHandler(RadioButtonExtension_Checked);
            this.Click += RadioButtonExtension_Click;
        }

        private void RadioButtonExtension_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (WasChecked > 0)
            {
                this.IsCheckedChanged = !this.IsCheckedChanged;
            }
            WasChecked = 1;
        }

        private void RadioButtonExtension_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.IsCheckedChanged = true;
            WasChecked = 0;
        }
    }
}