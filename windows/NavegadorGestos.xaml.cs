using System;
using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace UD2_1_Bouzas_Prado_Bran.windows
{
    public partial class NavegadorGestos : Window
    {
        private readonly MainWindow _mainWindow;

        public NavegadorGestos(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Loaded += NavegadorGestos_Loaded;
        }

        private async void NavegadorGestos_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicializar navegador principal
            await webPrincipal.EnsureCoreWebView2Async();
            webPrincipal.Source = new Uri("https://www.google.com");

            // Inicializar overlay cámara
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = Path.Combine(baseDir, "web", "cam_overlay.html");

            if (!File.Exists(htmlPath))
            {
                MessageBox.Show($"❌ Falta el archivo: {htmlPath}");
                return;
            }

            await webCamOverlay.EnsureCoreWebView2Async();
            webCamOverlay.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webCamOverlay.Source = new Uri(htmlPath);

            // Permitir cámara en overlay
            webCamOverlay.CoreWebView2.PermissionRequested += (s, args) =>
            {
                if (args.PermissionKind == CoreWebView2PermissionKind.Camera)
                    args.State = CoreWebView2PermissionState.Allow;
            };
        }

        private void BtnAtras_Click(object sender, RoutedEventArgs e)
        {
            if (webPrincipal.CanGoBack) webPrincipal.GoBack();
        }

        private void BtnAdelante_Click(object sender, RoutedEventArgs e)
        {
            if (webPrincipal.CanGoForward) webPrincipal.GoForward();
        }

        private void BtnRecargar_Click(object sender, RoutedEventArgs e)
        {
            webPrincipal.Reload();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Show();
            this.Close();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string texto = TxtBusqueda.Text;
            if (string.IsNullOrWhiteSpace(texto)) return;

            string url = texto.StartsWith("http") ? texto : $"https://www.google.com/search?q={Uri.EscapeDataString(texto)}";
            webPrincipal.Source = new Uri(url);
        }
    }
}