using System.Windows;

namespace UD2_1_Bouzas_Prado_Bran
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnPPT_Click(object sender, RoutedEventArgs e)
        {
            new windows.PiedraPapelTijera(this).Show();
            this.Hide(); 
        }

        private void BtnNavegador_Click(object sender, RoutedEventArgs e)
        {
            new windows.NavegadorGestos(this).Show();
            this.Hide();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}