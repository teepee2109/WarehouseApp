using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseApp.Models; // Đảm bảo using namespace Models

namespace WarehouseApp
{
    /// <summary>
    /// ViewModel tạm thời cho DataGrid
    /// </summary>
    public class ExportDetailViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public partial class ExportPage : Page
    {
        private ObservableCollection<ExportDetailViewModel> exportList;
        // Cache để lưu Tồn kho Tĩnh
        // Key = ProductID, Value = Product (để lấy tồn kho)
        private Dictionary<int, Product> productCache = new Dictionary<int, Product>();

        public ExportPage()
        {
            InitializeComponent();
            LoadInitialData();

            exportList = new ObservableCollection<ExportDetailViewModel>();
            dgExportDetails.ItemsSource = exportList;
        }

        /// <Tải dữ liệu ban đầu cho ComboBoxes>
        private void LoadInitialData()
        {
            using (var context = new WarehouseDbContext())
            {
                cbWarehouse.ItemsSource = context.Warehouses.ToList();

                // Tải sản phẩm vào Cache
                var products = context.Products.ToList();
                cbProductSelect.ItemsSource = products;

                // Lưu cache (Giả sử Product model có cột Quantity)
                // NẾU LỖI Ở ĐÂY, có nghĩa là Model 'Product' của bạn thiếu 'Quantity'
                try
                {
                    productCache = products.ToDictionary(p => p.ProductId, p => p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: Model Product của bạn có thể thiếu cột 'Quantity'. " + ex.Message);
                }
            }
            dpExportDate.SelectedDate = DateTime.Today;
            txtUser.Text = "Admin (ID: 1)";
        }

        /// <summary>
        /// Khi chọn sản phẩm, hiển thị tồn kho từ Cache
        /// </summary>
        private void CbProductSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbProductSelect.SelectedItem is Product selectedProduct)
            {
                // Tìm tồn kho từ cache
                if (productCache.TryGetValue(selectedProduct.ProductId, out var product))
                {
                    // GIẢ SỬ BẠN CÓ CỘT 'Quantity' TRONG MODEL 'Product'
                    txtStock.Text = product.Quantity.ToString(); // <-- SỬA ĐỔI QUAN TRỌNG
                }
                else
                {
                    txtStock.Text = "0";
                }
            }
            else
            {
                txtStock.Text = "0";
            }
        }

        /// <summary>
        /// Thêm sản phẩm vào DataGrid
        /// </summary>
        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            if (cbProductSelect.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Số lượng xuất phải là một số dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra Tồn kho
            if (!int.TryParse(txtStock.Text, out int availableStock))
            {
                availableStock = 0;
            }

            if (quantity > availableStock)
            {
                MessageBox.Show($"Số lượng xuất ({quantity}) không thể lớn hơn tồn kho ({availableStock}).", "Lỗi Tồn kho", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedProduct = (Product)cbProductSelect.SelectedItem;
            exportList.Add(new ExportDetailViewModel
            {
                ProductID = selectedProduct.ProductId,
                ProductName = selectedProduct.ProductName,
                Quantity = quantity
            });

            ResetAddProductForm();
        }

        /// <summary>
        /// Lưu phiếu xuất vào CSDL (LOGIC ĐÃ THAY ĐỔI HOÀN TOÀN)
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbWarehouse.SelectedValue == null || exportList.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn Kho xuất và thêm ít nhất 1 sản phẩm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Dùng DbContext mới VÀ Transaction (Rất quan trọng)
            using (var context = new WarehouseDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Tạo phiếu xuất chính
                    var newExportOrder = new ExportOrder
                    {
                        ExportDate = dpExportDate.SelectedDate ?? DateTime.Today,
                        WarehouseId = (int)cbWarehouse.SelectedValue,
                        UserId = 1, // Giả sử UserID 1 (Admin)
                        Note = txtNote.Text,
                        ExportOrderDetails = new List<ExportOrderDetail>()
                    };

                    // 2. Thêm chi tiết và CẬP NHẬT TỒN KHO TĨNH
                    foreach (var item in exportList)
                    {
                        // 2a. Thêm chi tiết vào phiếu
                        newExportOrder.ExportOrderDetails.Add(new ExportOrderDetail
                        {
                            ProductId = item.ProductID,
                            Quantity = item.Quantity
                        });

                        // 2b. Trừ tồn kho khỏi bảng Products
                        var productToUpdate = context.Products.Find(item.ProductID);
                        if (productToUpdate != null)
                        {
                            if (productToUpdate.Quantity < item.Quantity)
                            {
                                // Kiểm tra lại tồn kho đề phòng 2 người cùng xuất
                                throw new Exception($"Tồn kho sản phẩm '{productToUpdate.ProductName}' không đủ.");
                            }
                            productToUpdate.Quantity -= item.Quantity; // TRỪ TỒN KHO
                        }
                    }

                    // 3. Lưu tất cả vào CSDL
                    context.ExportOrders.Add(newExportOrder);
                    context.SaveChanges(); // Lưu cả phiếu xuất VÀ cập nhật tồn kho

                    // 4. Hoàn tất Transaction
                    transaction.Commit();

                    MessageBox.Show("Tạo phiếu xuất thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetPage();
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi (ví dụ: hết hàng), hủy bỏ mọi thay đổi
                    transaction.Rollback();
                    MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // --- CÁC HÀM CÒN LẠI (GIỮ NGUYÊN) ---

        private void BtnRemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            var itemToRemove = (sender as FrameworkElement)?.DataContext as ExportDetailViewModel;
            if (itemToRemove != null)
            {
                exportList.Remove(itemToRemove);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn hủy phiếu xuất này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ResetPage();
            }
        }

        private void ResetAddProductForm()
        {
            cbProductSelect.SelectedIndex = -1;
            txtQuantity.Clear();
            txtStock.Text = "0";
        }

        private void ResetPage()
        {
            cbWarehouse.SelectedIndex = -1;
            dpExportDate.SelectedDate = DateTime.Today;
            txtNote.Clear();
            exportList.Clear();
            ResetAddProductForm();
            LoadInitialData();
        }
    }
}