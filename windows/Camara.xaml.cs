using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace UD2_1_Bouzas_Prado_Bran.windows
{
    public partial class Camara : Window
    {
        public event Action<string>? PrediccionRecibida;
        public Camara(string nombreHtml)
        {
            InitializeComponent();
            Loaded += async (s, e) => await AbrirCamara(nombreHtml);
        }

        private async Task AbrirCamara(string nombreHtml)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = Path.Combine(baseDir, "web", nombreHtml);

            if (!File.Exists(htmlPath))
            {
                MessageBox.Show($"Error: El archivo 'ppt.html' no se encuentra.\nRuta de búsqueda: {htmlPath}", "Archivo No Encontrado", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            webView.Source = new Uri(htmlPath);
            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.Settings.IsScriptEnabled = true;
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            webView.CoreWebView2.PermissionRequested += (s, args) =>
            {
                if (args.PermissionKind == Microsoft.Web.WebView2.Core.CoreWebView2PermissionKind.Camera)
                {
                    args.State = Microsoft.Web.WebView2.Core.CoreWebView2PermissionState.Allow;
                }
            };

            webView.CoreWebView2.WebMessageReceived += (s, args) =>
            {
                string msg = args.TryGetWebMessageAsString();
                Dispatcher.Invoke(() => PrediccionRecibida?.Invoke(msg));
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            webView?.Dispose();
        }
    }
}
