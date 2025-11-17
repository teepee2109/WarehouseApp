using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseApp.Models;

namespace WarehouseApp
{
    public partial class CategoryPage : Page
    {
        private Category _selectedCategory = null;

        public CategoryPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

       
        private void LoadCategories()
        {
            using (var context = new WarehouseDbContext())
            {
                var query = context.Categories.AsQueryable();
                string searchTerm = txtSearch.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(c => c.CategoryName.ToLower().Contains(searchTerm));
                }

                dgCategories.ItemsSource = query.ToList();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        private void DgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = dgCategories.SelectedItem as Category;

            if (_selectedCategory != null)
            {
                txtCategoryId.Text = _selectedCategory.CategoryId.ToString();
                txtCategoryName.Text = _selectedCategory.CategoryName;

                btnSave.Content = "Lưu Sửa";
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            dgCategories.SelectedItem = null;

            _selectedCategory = null;

            txtCategoryId.Text = "(Tự động)";
            txtCategoryName.Text = "";

            btnSave.Content = "Lưu Thêm Mới"; 
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Tên danh mục là bắt buộc.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new WarehouseDbContext())
            {
                if (_selectedCategory == null)
                {
                    var newCategory = new Category
                    {
                        CategoryName = txtCategoryName.Text.Trim()
                    };

                    context.Categories.Add(newCategory);
                    MessageBox.Show("Thêm danh mục thành công!");
                }
                else
                {
                    var categoryToUpdate = context.Categories.Find(_selectedCategory.CategoryId);
                    if (categoryToUpdate != null)
                    {
                        categoryToUpdate.CategoryName = txtCategoryName.Text.Trim();
                        MessageBox.Show("Cập nhật thành công!");
                    }
                }

                context.SaveChanges();
            }

            LoadCategories();
            BtnClear_Click(null, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var category = (sender as FrameworkElement)?.DataContext as Category;
            if (category == null) return;

            var confirm = MessageBox.Show(
                $"Xóa danh mục '{category.CategoryName}'?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                using (var context = new WarehouseDbContext())
                {
                    var categoryToDelete = context.Categories.Find(category.CategoryId);
                    if (categoryToDelete != null)
                    {
                        context.Categories.Remove(categoryToDelete);
                        context.SaveChanges();
                    }
                }

                LoadCategories();
                BtnClear_Click(null, null);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Không thể xóa. Có sản phẩm đang dùng danh mục này!",
                    "Lỗi khóa ngoại",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

    }
}