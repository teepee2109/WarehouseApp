using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseApp.Models;

namespace WarehouseApp
{
    public partial class WareHousePage : Page
    {
        private Warehouse _selectedWarehouse = null;

        public WareHousePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWarehouses();
            ClearForm();
        }

        private void LoadWarehouses()
        {
            using (var context = new WarehouseDbContext())
            {
                var query = context.Warehouses.AsQueryable();
                string searchTerm = txtSearch.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(w => w.WarehouseName.ToLower().Contains(searchTerm));
                }

                dgWarehouses.ItemsSource = query.ToList();
            }
        }

        private void ClearForm()
        {
            dgWarehouses.SelectedItem = null;
            _selectedWarehouse = null;

            txtWarehouseId.Text = "(Tự động)";
            txtWarehouseName.Text = "";
            txtAddress.Text = "";

            btnSave.Content = "Lưu Thêm Mới";
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWarehouses();
        }

        private void DgWarehouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedWarehouse = dgWarehouses.SelectedItem as Warehouse;

            if (_selectedWarehouse != null)
            {
                txtWarehouseId.Text = _selectedWarehouse.WarehouseId.ToString();
                txtWarehouseName.Text = _selectedWarehouse.WarehouseName;
                txtAddress.Text = _selectedWarehouse.Address;

                btnSave.Content = "Lưu Sửa";
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWarehouseName.Text))
            {
                MessageBox.Show("Tên kho hàng là bắt buộc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new WarehouseDbContext())
            {
                if (_selectedWarehouse == null) 
                {
                    var newWarehouse = new Warehouse
                    {
                        WarehouseName = txtWarehouseName.Text,
                        Address = txtAddress.Text
                    };
                    context.Warehouses.Add(newWarehouse);
                    MessageBox.Show("Đã thêm kho hàng mới!", "Thành công");
                }
                else 
                {
                    var warehouseToUpdate = context.Warehouses.Find(_selectedWarehouse.WarehouseId);
                    if (warehouseToUpdate != null)
                    {
                        warehouseToUpdate.WarehouseName = txtWarehouseName.Text;
                        warehouseToUpdate.Address = txtAddress.Text;
                    }
                    MessageBox.Show("Đã cập nhật kho hàng!", "Thành công");
                }
                context.SaveChanges();
            }

            LoadWarehouses();
            ClearForm();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var warehouse = (sender as FrameworkElement)?.DataContext as Warehouse;
            if (warehouse == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa '{warehouse.WarehouseName}'?",
                                         "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new WarehouseDbContext())
                    {
                        var warehouseToDelete = context.Warehouses.Find(warehouse.WarehouseId);
                        if (warehouseToDelete != null)
                        {
                            context.Warehouses.Remove(warehouseToDelete);
                            context.SaveChanges();
                        }
                    }

                    LoadWarehouses();
                    ClearForm();
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Không thể xóa kho này vì đã có phiếu nhập/xuất liên quan.", "Lỗi khóa ngoại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}