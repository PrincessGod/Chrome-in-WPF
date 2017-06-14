using System.ComponentModel;
using System.Runtime.CompilerServices;
using BrowserTest.Annotations;

namespace BrowserTest
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string _adress = "www.google.com";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        public string Adress
        {
            get { return _adress; }

            set
            {
                _adress = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            uiBrowser.Closing();
        }
    }
}