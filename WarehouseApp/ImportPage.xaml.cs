using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseApp.Models; 

namespace WarehouseApp
{
    public class ImportDetailViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }

    public partial class ImportPage : Page
    {
        private ObservableCollection<ImportDetailViewModel> importList;

        public ImportPage()
        {
            InitializeComponent();
            LoadInitialData();

            importList = new ObservableCollection<ImportDetailViewModel>();
            dgImportDetails.ItemsSource = importList;
        }

        /// <summary>
        /// Tải dữ liệu ban đầu (dùng 'using' DbContext)
        /// </summary>
        private void LoadInitialData()
        {
            // Dùng 'using' để đảm bảo context được giải phóng
            using (var context = new WarehouseDbContext())
            {
                cbSupplier.ItemsSource = context.Suppliers.ToList();
                cbWarehouse.ItemsSource = context.Warehouses.ToList();
                cbProductSelect.ItemsSource = context.Products.ToList();
            }
            dpImportDate.SelectedDate = DateTime.Today;
            txtUser.Text = "Admin (ID: 1)"; // Giả sử UserID 1
        }

        /// <summary>
        /// Lưu phiếu nhập (LOGIC ĐÃ THAY ĐỔI HOÀN TOÀN)
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validate
            if (cbSupplier.SelectedValue == null || cbWarehouse.SelectedValue == null || importList.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn Nhà cung cấp, Kho và thêm ít nhất 1 sản phẩm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Dùng DbContext MỚI và Transaction (Rất quan trọng)
            using (var context = new WarehouseDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // 3. Tạo phiếu nhập chính
                    var newImportOrder = new ImportOrder
                    {
                        ImportDate = dpImportDate.SelectedDate ?? DateTime.Today,
                        SupplierId = (int)cbSupplier.SelectedValue,
                        WarehouseId = (int)cbWarehouse.SelectedValue,
                        UserId = 1, // Giả sử UserID 1 (Admin)
                        Note = txtNote.Text,
                        ImportOrderDetails = new List<ImportOrderDetail>()
                    };

                    // 4. Thêm chi tiết VÀ CẬP NHẬT TỒN KHO TĨNH
                    foreach (var item in importList)
                    {
                        // 4a. Thêm chi tiết vào phiếu
                        newImportOrder.ImportOrderDetails.Add(new ImportOrderDetail
                        {
                            ProductId = item.ProductID,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });

                        // 4b. CỘNG dồn tồn kho vào bảng Products
                        // <-- SỬA ĐỔI QUAN TRỌNG
                        var productToUpdate = context.Products.Find(item.ProductID);
                        if (productToUpdate != null)
                        {
                            // GIẢ SỬ BẠN CÓ CỘT 'Quantity' TRONG MODEL 'Product'
                            productToUpdate.Quantity += item.Quantity; // CỘNG TỒN KHO
                        }
                    }

                    // 5. Lưu tất cả vào CSDL
                    context.ImportOrders.Add(newImportOrder);
                    context.SaveChanges(); // Lưu cả phiếu nhập VÀ cập nhật tồn kho

                    // 6. Hoàn tất Transaction
                    transaction.Commit();

                    MessageBox.Show("Tạo phiếu nhập thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetPage();
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi, hủy bỏ mọi thay đổi
                    transaction.Rollback();
                    MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // --- CÁC HÀM CÒN LẠI (GIỮ NGUYÊN) ---

        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            if (cbProductSelect.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Số lượng phải là một số dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!decimal.TryParse(txtImportPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Giá nhập phải là một số hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedProduct = (Product)cbProductSelect.SelectedItem;

            importList.Add(new ImportDetailViewModel
            {
                ProductID = selectedProduct.ProductId,
                ProductName = selectedProduct.ProductName,
                Quantity = quantity,
                Price = price
            });

            UpdateTotalAmount();
            ResetAddProductForm();
        }

        private void BtnRemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            var itemToRemove = (sender as FrameworkElement)?.DataContext as ImportDetailViewModel;
            if (itemToRemove != null)
            {
                importList.Remove(itemToRemove);
                UpdateTotalAmount();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn hủy phiếu nhập này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ResetPage();
            }
        }

        private void UpdateTotalAmount()
        {
            decimal total = importList.Sum(item => item.Total);
            tbTotalAmount.Text = $"Tổng cộng: {total:N0} ₫";
        }

        private void ResetAddProductForm()
        {
            cbProductSelect.SelectedIndex = -1;
            txtQuantity.Clear();
            txtImportPrice.Clear();
        }

        private void ResetPage()
        {
            cbSupplier.SelectedIndex = -1;
            cbWarehouse.SelectedIndex = -1;
            dpImportDate.SelectedDate = DateTime.Today;
            txtNote.Clear();
            importList.Clear();
            UpdateTotalAmount();
            ResetAddProductForm();
        }
    }
}