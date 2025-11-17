using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseApp.Models;

namespace WarehouseApp
{
    public partial class SuplierPage : Page
    {
        private Supplier _selectedSupplier = null;

        public SuplierPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSuppliers();
            ClearForm();
        }

        private void LoadSuppliers()
        {
            using (var context = new WarehouseDbContext())
            {
                var query = context.Suppliers.AsQueryable();
                string searchTerm = txtSearch.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(s => s.SupplierName.ToLower().Contains(searchTerm));
                }

                dgSuppliers.ItemsSource = query.ToList();
            }
        }

        private void ClearForm()
        {
            dgSuppliers.SelectedItem = null;
            _selectedSupplier = null;

            txtSupplierId.Text = "(Tự động)";
            txtSupplierName.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";

            btnSave.Content = "Lưu Thêm Mới";
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadSuppliers();
        }

        private void DgSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSupplier = dgSuppliers.SelectedItem as Supplier;

            if (_selectedSupplier != null)
            {
                txtSupplierId.Text = _selectedSupplier.SupplierId.ToString();
                txtSupplierName.Text = _selectedSupplier.SupplierName;
                txtPhone.Text = _selectedSupplier.Phone;
                txtAddress.Text = _selectedSupplier.Address;

                btnSave.Content = "Lưu Sửa";
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSupplierName.Text))
            {
                MessageBox.Show("Tên nhà cung cấp là bắt buộc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new WarehouseDbContext())
            {
                if (_selectedSupplier == null) 
                {
                    var newSupplier = new Supplier
                    {
                        SupplierName = txtSupplierName.Text,
                        Phone = txtPhone.Text,
                        Address = txtAddress.Text
                    };
                    context.Suppliers.Add(newSupplier);
                    MessageBox.Show("Đã thêm nhà cung cấp mới!", "Thành công");
                }
                else 
                {
                    var supplierToUpdate = context.Suppliers.Find(_selectedSupplier.SupplierId);
                    if (supplierToUpdate != null)
                    {
                        supplierToUpdate.SupplierName = txtSupplierName.Text;
                        supplierToUpdate.Phone = txtPhone.Text;
                        supplierToUpdate.Address = txtAddress.Text;
                    }
                    MessageBox.Show("Đã cập nhật nhà cung cấp!", "Thành công");
                }
                context.SaveChanges();
            }

            LoadSuppliers();
            ClearForm();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var supplier = (sender as FrameworkElement)?.DataContext as Supplier;
            if (supplier == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa '{supplier.SupplierName}'?",
                                         "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new WarehouseDbContext())
                    {
                        var supplierToDelete = context.Suppliers.Find(supplier.SupplierId);
                        if (supplierToDelete != null)
                        {
                            context.Suppliers.Remove(supplierToDelete);
                            context.SaveChanges();
                        }
                    }

                    LoadSuppliers();
                    ClearForm();
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Không thể xóa nhà cung cấp này vì đã có phiếu nhập liên quan.", "Lỗi khóa ngoại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}