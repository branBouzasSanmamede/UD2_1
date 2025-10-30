using System;
using System.IO;
using System.Windows;

namespace UD2_1_Bouzas_Prado_Bran.windows
{
    public partial class PiedraPapelTijera : Window
    {
        private MainWindow _mainWindow;
        public PiedraPapelTijera(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Loaded += Window_Loaded;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = Path.Combine(baseDir, "web", "ppt.html");

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
                Dispatcher.Invoke(() => ProcesarPrediccion(msg));
            };
        }

        private void ProcesarPrediccion(string prediction)
        {
            switch (prediction)
            {
                case "0":
                    lblResultado.Text = "Piedra";
                    btnJugar.IsEnabled = true;
                    break;
                case "1":
                    lblResultado.Text = "Papel";
                    btnJugar.IsEnabled = true;
                    break;
                case "2":
                    lblResultado.Text = "Tijeras";
                    btnJugar.IsEnabled = true;
                    break;
                case "ERROR_CAMARA_O_MODELO":
                    lblResultado.Text = "❌ Error: Cámara o modelo no disponible";
                    btnJugar.IsEnabled = false;
                    break;
                case "ERROR_PREDICCION":
                    lblResultado.Text = "❌ Error en la predicción";
                    btnJugar.IsEnabled = false;
                    break;
                default:
                    lblResultado.Text = "Esperando predicción...";
                    btnJugar.IsEnabled = false;
                    break;
            }
        }

        private void BtnJugar_Click(object sender, RoutedEventArgs e)
        {
            int jugador = PredictionToInt(lblResultado.Text);
            if (jugador == -1)
            {
                MessageBox.Show("No hay jugada válida para jugar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int jugadaMaquina = new Random().Next(0, 3);

            lblJugadaMaquina.Text = jugadaMaquina switch
            {
                0 => "Piedra",
                1 => "Papel",
                2 => "Tijeras",
                _ => "---"
            };

            int resultado = DeterminarGanador(jugador, jugadaMaquina);

            lblResultadoFinal.Text = resultado switch
            {
                0 => "Empate",
                1 => "¡Has ganado!",
                2 => "Ha ganado la máquina",
                _ => "---"
            };
        }

        private static int PredictionToInt(string pred)
        {
            return pred switch
            {
                "Piedra" => 0,
                "Papel" => 1,
                "Tijeras" => 2,
                _ => -1
            };
        }

        private static int DeterminarGanador(int jugador, int maquina)
        {
            if (jugador == maquina) return 0;
            if ((jugador == 0 && maquina == 2) ||
                (jugador == 1 && maquina == 0) ||
                (jugador == 2 && maquina == 1))
            {
                return 1;
            }
            return 2;
        }

        private void BtnReiniciar_Click(object sender, RoutedEventArgs e)
        {
            lblResultado.Text = "Esperando predicción...";
            lblJugadaMaquina.Text = "---";
            lblResultadoFinal.Text = "---";
            btnJugar.IsEnabled = false;
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Show();
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            webView?.Dispose();

            if (_mainWindow != null)
            {
                _mainWindow.Show();
            }
        }
    }
}