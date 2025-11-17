using System.Linq;
using System.Windows;
using WarehouseApp.Models;

namespace WarehouseApp
{
    public partial class Login : Window
    {
        WarehouseDbContext _db = new WarehouseDbContext();

        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Password.Trim();

            if (user == "" || pass == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            // kiểm tra trong bảng Users
            var account = _db.Users
                .Where(x => x.Username == user && x.PasswordHash == pass)
                .FirstOrDefault();

            if (account == null)
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu.");
                return;
            }

            // Nếu đăng nhập thành công
            MainWindow main = new MainWindow();
            main.Show();

            this.Close();
        }

       
    }
}
