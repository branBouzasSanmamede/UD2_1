using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Speech.Recognition;
using System.Threading.Tasks;
using System.Windows;

namespace UD2_1_Bouzas_Prado_Bran.windows
{
    public partial class NavegadorGestos : Window
    {
        private SpeechRecognitionEngine recognizer;

        public NavegadorGestos()
        {
            InitializeComponent();
            Loaded += NavegadorGestos_Loaded;
        }

        private async void NavegadorGestos_Loaded(object sender, RoutedEventArgs e)
        {
            string path = Path.GetFullPath(@"..\web\nav_gestos.html");
            webView.Source = new Uri(path);
            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.Settings.IsScriptEnabled = true;
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;

            webView.CoreWebView2.WebMessageReceived += (s, args) =>
            {
                string msg = args.TryGetWebMessageAsString();
                Dispatcher.Invoke(() => ProcesarGesto(msg));
            };

            InicializarReconocimientoVoz();
        }
        private void InicializarReconocimientoVoz()
        {
            try
            {
                recognizer = new SpeechRecognitionEngine();
                recognizer.SetInputToDefaultAudioDevice();

                recognizer.LoadGrammar(new DictationGrammar());
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error iniciando reconocimiento de voz: " + ex.Message);
            }
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string texto = e.Result.Text;
            TxtBusqueda.Text = texto;
            Buscar(texto);
        }

        private void ProcesarGesto(string gesto)
        {
            switch (gesto)
            {
                case "0": 
                    BtnAtras_Click(null, null);
                    break;

                case "1": 
                    BtnAdelante_Click(null, null);
                    break;

                case "2": 
                    BtnRecargar_Click(null, null);
                    break;

                case "3": 
                    lblEstado.Text = "🎤 Di lo que quieres buscar...";
                    recognizer.RecognizeAsync(RecognizeMode.Single);
                    break;

                case "ERROR_CAMARA_O_MODELO":
                    MessageBox.Show("Error al cargar cámara o modelo de gestos.");
                    break;
            }
        }

        private void BtnAtras_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoBack) webView.GoBack();
        }

        private void BtnAdelante_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoForward) webView.GoForward();
        }

        private void BtnRecargar_Click(object sender, RoutedEventArgs e)
        {
            webView.Reload();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Buscar(TxtBusqueda.Text);
        }

        private void TxtBusqueda_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TxtPlaceholder.Visibility = string.IsNullOrWhiteSpace(TxtBusqueda.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void Buscar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return;

            string url = texto.StartsWith("http") ? texto : $"https://www.google.com/search?q={Uri.EscapeDataString(texto)}";
            webView.Source = new Uri(url);
            lblEstado.Text = "🔍 Buscando: " + texto;
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}