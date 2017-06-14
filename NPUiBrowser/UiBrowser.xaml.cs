using System.Windows;
using CefSharp;
using CefSharp.WinForms;

namespace NPUiBrowser
{
    /// <summary>
    ///     用 CefSharp 57.0.0 的 WinForm 控件 封装成 WPF 用户控件
    ///     可支持页面地址绑定
    /// </summary>
    public partial class UiBrowser
    {
        private static ChromiumWebBrowser _browser;

        /// <summary>
        ///     页面地址属性
        /// </summary>
        public static readonly DependencyProperty AdressProperty = DependencyProperty.Register(
            "Adress", typeof(string), typeof(UiBrowser),
            new PropertyMetadata("https://www.baidu.com/", PropertyChangedCallback));

        /// <summary>
        ///     Chrome 内核 WPF 浏览器
        /// </summary>
        public UiBrowser()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     页面地址
        /// </summary>
        public string Adress
        {
            get { return (string) GetValue(AdressProperty); }
            set { SetValue(AdressProperty, value); }
        }

        /// <summary>
        ///     CefSharp 控件
        /// </summary>
        public ChromiumWebBrowser Browser => _browser;

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            _browser?.Load(dependencyPropertyChangedEventArgs.NewValue.ToString());
        }

        private void UiBrowser_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_browser == null)
                _browser = new ChromiumWebBrowser(Adress);
            FormsHost.Child = _browser;
        }

        /// <summary>
        ///     销毁控件  在关闭窗口前调用
        /// </summary>
        public void Closing()
        {
            Cef.Shutdown();
            _browser = null;
            FormsHost.Child = null;
        }
    }
}