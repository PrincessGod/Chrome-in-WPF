using System;
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

        /// <summary>
        ///     浏览器对象创建完成
        /// </summary>
        public event Action OnBrowserInited;

        /// <summary>
        ///     DOM创建完成
        /// </summary>
        public event Action<IWebBrowser, IBrowser, IFrame> OnDOMLoaded;

        /// <summary>
        ///     加载状态改变
        /// </summary>
        public event Action<object, LoadingStateChangedEventArgs> OnLoadingStateChanged;

        /// <summary>
        ///     页面开始加载
        /// </summary>
        public event Action<object, FrameLoadStartEventArgs> OnFrameStartLoad;

        /// <summary>
        ///     页面加载完成
        /// </summary>
        public event Action<object, FrameLoadEndEventArgs> OnFrameEndLoad;

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            _browser?.Load(dependencyPropertyChangedEventArgs.NewValue.ToString());
        }

        private void UiBrowser_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_browser == null)
            {
                _browser = new ChromiumWebBrowser(Adress);

                _browser.RenderProcessMessageHandler = new RenderProcessMessageHandler(OnDOMLoaded);

                //Wait for the page to finish loading (all resources will have been loaded, rendering is likely still happening)
                _browser.LoadingStateChanged += (s, args) => { OnLoadingStateChanged?.Invoke(s, args); };

                //Wait for the MainFrame to finish loading
                _browser.FrameLoadEnd += (s, args) => { OnFrameEndLoad?.Invoke(s, args); };

                _browser.FrameLoadStart += (o, args) => { OnFrameStartLoad?.Invoke(o, args); };

                OnBrowserInited?.Invoke();
            }
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

    /// <summary>
    ///     JavaScript Context 状态变化
    /// </summary>
    public class RenderProcessMessageHandler : IRenderProcessMessageHandler
    {
        private readonly Action<IWebBrowser, IBrowser, IFrame> _loadevent;

        /// <summary>
        ///     Javascript Contetx 变化
        /// </summary>
        /// <param name="loadAction"></param>
        public RenderProcessMessageHandler(Action<IWebBrowser, IBrowser, IFrame> loadAction)
        {
            _loadevent = loadAction;
        }

        // Wait for the underlying `Javascript Context` to be created, this is only called for the main frame.
        // If the page has no javascript, no context will be created.
        void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            _loadevent?.Invoke(browserControl, browser, frame);
        }

        public void OnContextReleased(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
        }

        public void OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node)
        {
        }
    }
}