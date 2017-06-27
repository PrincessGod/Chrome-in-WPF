using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BrowserTest.Annotations;
using CefSharp;

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
            uiBrowser.OnBrowserInited += UiBrowser_OnBrowserInited;
            uiBrowser.OnDOMLoaded += UiBrowser_OnDOMLoaded;
            uiBrowser.OnFrameEndLoad += UiBrowser_OnFrameEndLoad;
            uiBrowser.OnFrameStartLoad += UiBrowser_OnFrameStartLoad;
            uiBrowser.OnLoadingStateChanged += UiBrowser_OnLoadingStateChanged;
        }

        private void UiBrowser_OnLoadingStateChanged(object arg1, LoadingStateChangedEventArgs arg2)
        {
            //Wait for the Page to finish loading
            if (arg2.IsLoading == false)
            {
                uiBrowser.Browser.GetMainFrame().ExecuteJavaScriptAsync("alert('All Resources Have Loaded');");
            }
        }

        private void UiBrowser_OnFrameStartLoad(object arg1, FrameLoadStartEventArgs arg2)
        {
            //Wait for the MainFrame to finish loading
            if (arg2.Frame.IsMain)
            {
                arg2.Frame.ExecuteJavaScriptAsync("alert('MainFrame start loading');");
            }
        }

        private void UiBrowser_OnFrameEndLoad(object arg1, FrameLoadEndEventArgs arg2)
        {
            //Wait for the MainFrame to finish loading
            if (arg2.Frame.IsMain)
            {
                arg2.Frame.ExecuteJavaScriptAsync("alert('MainFrame finished loading');");
            }
        }

        private void UiBrowser_OnDOMLoaded(IWebBrowser arg1, IBrowser arg2, IFrame arg3)
        {
            const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
            arg3.ExecuteJavaScriptAsync(script);
        }

        private void UiBrowser_OnBrowserInited()
        {
            MessageBox.Show("浏览器初始化！");
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