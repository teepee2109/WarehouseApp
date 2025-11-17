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
using System.Windows.Shapes;
using WarehouseApp.Models;

namespace WarehouseApp
{
    /// <summary>
    /// Interaction logic for AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        // Biến này dùng để xác định là "Thêm" (null) hay "Sửa" (có giá trị)
        private int? _productId;
        private Product _productToEdit;

        public AddEditProductWindow()
        {
            InitializeComponent();
            _productId = null;
            Title = "Thêm Sản phẩm mới";
        }

        public AddEditProductWindow(int productId)
        {
            InitializeComponent();
            _productId = productId;
            Title = "Sửa Thông tin Sản phẩm";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();

            if (_productId.HasValue)
            {
                LoadProductData();
            }
        }

        private void LoadCategories()
        {
            using (var context = new WarehouseDbContext())
            {
                cbCategory.ItemsSource = context.Categories.ToList();

                if (cbCategory.Items.Count > 0)
                {
                    cbCategory.SelectedIndex = 0;
                }
            }
        }

        private void LoadProductData()
        {
            using (var context = new WarehouseDbContext())
            {
                _productToEdit = context.Products.Find(_productId.Value);
                if (_productToEdit == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm. Cửa sổ sẽ đóng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                txtProductName.Text = _productToEdit.ProductName;
                cbCategory.SelectedValue = _productToEdit.CategoryId;
                txtUnit.Text = _productToEdit.Unit;
                txtPrice.Text = _productToEdit.Price?.ToString("N0");
                txtDescription.Text = _productToEdit.Description;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text) || cbCategory.SelectedValue == null)
            {
                MessageBox.Show("Tên sản phẩm và Danh mục là bắt buộc.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                price = 0; 
            }

            try
            {
                using (var context = new WarehouseDbContext())
                {
                    if (_productId == null)
                    {
                        var newProduct = new Product
                        {
                            ProductName = txtProductName.Text,
                            CategoryId = (int)cbCategory.SelectedValue,
                            Unit = txtUnit.Text,
                            Price = price,
                            Description = txtDescription.Text
                        };
                        context.Products.Add(newProduct);
                    }
                    else 
                    {
                        var product = context.Products.Find(_productId.Value);
                        if (product != null)
                        {
                            product.ProductName = txtProductName.Text;
                            product.CategoryId = (int)cbCategory.SelectedValue;
                            product.Unit = txtUnit.Text;
                            product.Price = price;
                            product.Description = txtDescription.Text;
                        }
                    }

                    context.SaveChanges();
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
