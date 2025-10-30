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
            new windows.PiedraPapelTijera().Show();
            this.Hide(); // Oculta la pantalla principal
        }

        private void BtnNavegador_Click(object sender, RoutedEventArgs e)
        {
            new windows.NavegadorGestos().Show();
            this.Hide();
        }
    }
}