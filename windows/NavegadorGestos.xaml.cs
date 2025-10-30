using System;
using System.IO;
using System.Windows;
using System.Speech.Recognition;
using Microsoft.Web.WebView2.Core;

namespace UD2_1_Bouzas_Prado_Bran.windows
{
    public partial class NavegadorGestos : Window
    {
        private SpeechRecognitionEngine? _speechRecognizer;
        private readonly MainWindow _mainWindow;
        private Camara? _camara;

        public NavegadorGestos(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Loaded += Window_Loaded;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await webPrincipal.EnsureCoreWebView2Async();
            webPrincipal.Source = new Uri("https://www.google.com");
            webPrincipal.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            _camara = new Camara("navegador.html");
            _camara.PrediccionRecibida += ProcesarPrediccion;
            _camara.Show();

            InicializarReconocimientoVoz();
        }

        private void InicializarReconocimientoVoz()
        {
            try
            {
                _speechRecognizer = new SpeechRecognitionEngine();
                _speechRecognizer.SetInputToDefaultAudioDevice();

                _speechRecognizer.LoadGrammar(new DictationGrammar());
                _speechRecognizer.SpeechRecognized += ReconocimientoDeVoz;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar reconocimiento de voz: " + ex.Message);
            }
        }

        private void ProcesarPrediccion(string prediction)
        {
            switch (prediction)
            {
                case "0": if (webPrincipal.CanGoBack) webPrincipal.GoBack(); break;
                case "1": if (webPrincipal.CanGoForward) webPrincipal.GoForward(); break;
                case "2": webPrincipal.Reload(); break;
                case "3":
                    EscucharBusquedaPorVoz();
                    break;
                case "4":
                    string texto = TxtBusqueda.Text;
                    if (string.IsNullOrWhiteSpace(texto)) return;

                    string url = texto.StartsWith("http") ? texto : $"https://www.google.com/search?q={Uri.EscapeDataString(texto)}";

                    webPrincipal.Source = new Uri(url);
                    break;
                case "ERRORES_CAMARA":
                    MessageBox.Show("Error en la predicción o la cámara");
                    break;
                default:
                    Console.WriteLine("Gesto desconocido: " + prediction);
                    break;
            }
        }

        private void EscucharBusquedaPorVoz()
        {
            if (_speechRecognizer == null) return;

            MessageBox.Show("🎤 Diga lo que desea buscar...");

            try
            {
                _speechRecognizer.RecognizeAsyncStop();
                _speechRecognizer.RecognizeAsync(RecognizeMode.Single);
            }
            catch (InvalidOperationException)
            {
                // Ignora si ya está escuchando
            }
        }

        private void ReconocimientoDeVoz(object? sender, SpeechRecognizedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                string texto = e.Result.Text.Trim();
                TxtBusqueda.Text = texto;
                MessageBox.Show($"🗣️ Texto reconocido: \"{texto}\"");
            });
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _mainWindow.Show();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            _speechRecognizer?.RecognizeAsyncStop();
            _speechRecognizer?.Dispose();
            _camara?.Close();
            _mainWindow?.Show();
        }
    }
}