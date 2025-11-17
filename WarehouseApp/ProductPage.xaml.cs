using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseApp.Models;

namespace WarehouseApp
{
    /// <summary>
    /// Interaction logic for ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();
            LoadCategories();
            LoadProducts();
            
        }
        private void LoadProducts()
        {
            // Tạo context mới chỉ dùng cho hàm này
            using (var context = new WarehouseDbContext())
            {
                // Dùng AsQueryable() để xây dựng truy vấn
                var query = context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                // 1. Áp dụng Lọc Tìm kiếm (nếu có)
                string searchTerm = txtSearch.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => p.ProductName.ToLower().Contains(searchTerm));
                }

                // 2. Áp dụng Lọc Danh mục (nếu có)
                if (cbCategoryFilter.SelectedValue != null && (int)cbCategoryFilter.SelectedValue != 0)
                {
                    int categoryId = (int)cbCategoryFilter.SelectedValue;
                    query = query.Where(p => p.CategoryId == categoryId);
                }

                // 3. Thực thi truy vấn
                dgProducts.ItemsSource = query.ToList();
            }
        }
        private void LoadCategories()
        {
            using (var context = new WarehouseDbContext())
            {
                var categories = context.Categories.ToList();
                categories.Insert(0, new Category { CategoryId = 0, CategoryName = "Tất cả danh mục" });

                cbCategoryFilter.ItemsSource = categories;
                cbCategoryFilter.DisplayMemberPath = "CategoryName";
                cbCategoryFilter.SelectedValuePath = "CategoryId";
                cbCategoryFilter.SelectedIndex = 0;
            }
        }
        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();

        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ AddEditProductWindow (bạn cần tự tạo)
            // Constructor rỗng nghĩa là "Chế độ Thêm mới"
            AddEditProductWindow addWindow = new AddEditProductWindow();

            // Dùng ShowDialog() để nó khóa trang chính lại
            if (addWindow.ShowDialog() == true)
            {
                // Nếu cửa sổ đóng với kết quả OK (đã lưu), tải lại danh sách
                LoadProducts();
            }
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // 1. Lấy sản phẩm từ dòng được chọn
            var product = (sender as FrameworkElement)?.DataContext as Product;
            if (product == null) return;

            // 2. Mở cửa sổ AddEditProductWindow
            // Truyền ProductID vào để báo hiệu "Chế độ Sửa"
            AddEditProductWindow editWindow = new AddEditProductWindow(product.ProductId);

            if (editWindow.ShowDialog() == true)
            {
                // Nếu lưu thành công, tải lại danh sách
                LoadProducts();
            }
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as FrameworkElement)?.DataContext as Product;
            if (product == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm: {product.ProductName}?",
                                         "Xác nhận xóa",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                using (var context = new WarehouseDbContext())
                {
                    var productToDelete = context.Products.Find(product.ProductId);
                    if (productToDelete != null)
                    {
                        context.Products.Remove(productToDelete);
                        context.SaveChanges();
                    }
                }

                LoadProducts();
            }
            catch (DbUpdateException) 
            {
                MessageBox.Show("Không thể xóa sản phẩm này vì đã có lịch sử nhập/xuất.",
                                "Lỗi",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
