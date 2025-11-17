using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WarehouseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Product_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductPage());
        }

        private void BtnDashboard(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void BtnLogout(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ImportPage());
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CategoryPage());
        }

        private void Suplier_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SuplierPage());
        }

        private void WareHouse_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WareHousePage());
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ExportPage());

        }

       

        private void History_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HistoryPage());
        }
    }
}