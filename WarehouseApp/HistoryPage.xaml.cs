using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseApp.Models; // Đảm bảo using namespace Models

namespace WarehouseApp
{
    /// <summary>
    /// ViewModel chung để hợp nhất Phiếu Nhập và Phiếu Xuất
    /// </summary>
    public class HistoryItemViewModel
    {
        public int RecordId { get; set; }     // ImportID hoặc ExportID
        public bool IsImport { get; set; }   // true = Nhập, false = Xuất
        public string Type => IsImport ? "Phiếu Nhập" : "Phiếu Xuất";
        public DateTime Date { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string UserName { get; set; }
        public string Reference { get; set; } // Nhà cung cấp (nếu nhập) hoặc Ghi chú (nếu xuất)
        public string Note { get; set; }
    }

    public partial class HistoryPage : Page
    {
        public HistoryPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFilters();
            LoadHistory();
        }

        /// <summary>
        /// Tải dữ liệu cho các ComboBox lọc
        /// </summary>
        private void LoadFilters()
        {
            // Lọc Loại phiếu
            cbTypeFilter.ItemsSource = new List<string> { "Tất cả", "Phiếu Nhập", "Phiếu Xuất" };
            cbTypeFilter.SelectedIndex = 0;

            // Lọc Kho
            using (var context = new WarehouseDbContext())
            {
                var warehouses = context.Warehouses.ToList();
                warehouses.Insert(0, new Warehouse { WarehouseId = 0, WarehouseName = "Tất cả kho" });
                cbWarehouseFilter.ItemsSource = warehouses;
                cbWarehouseFilter.SelectedIndex = 0;
            }

            // Đặt ngày mặc định (30 ngày qua)
            dpFromDate.SelectedDate = DateTime.Today.AddDays(-30);
            dpToDate.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// Tải danh sách lịch sử (hợp nhất Nhập và Xuất)
        /// </summary>
        // Trong file HistoryPage.xaml.cs
        // Hãy thay thế hàm LoadHistory() của bạn bằng hàm này:

        // Trong file HistoryPage.xaml.cs

private void LoadHistory()
{
    using (var context = new WarehouseDbContext())
    {
        // 1. Lấy tất cả Phiếu Nhập
        var imports = context.ImportOrders
            .Include(i => i.Warehouse)
            .Include(i => i.User)
            .Include(i => i.Supplier)
            .Select(i => new HistoryItemViewModel
            {
                // SỬA LỖI Ở ĐÂY: Thêm '?? 0'
                // Điều này có nghĩa là "lấy ImportId, nhưng nếu nó null, dùng 0"
                RecordId = i.ImportId,
                IsImport = true,
                Date = i.ImportDate,
                
                // SỬA LỖI Ở ĐÂY: Thêm '?? 0'
                WarehouseId = i.WarehouseId ?? 0,

                WarehouseName = i.Warehouse.WarehouseName,
                UserName = i.User.FullName, 
                Reference = i.Supplier.SupplierName,
                Note = i.Note
            });

        // 2. Lấy tất cả Phiếu Xuất
        var exports = context.ExportOrders
            .Include(e => e.Warehouse)
            .Include(e => e.User)
            .Select(e => new HistoryItemViewModel
            {
                // SỬA LỖI Ở ĐÂY: Thêm '?? 0'
                RecordId = e.ExportId,
                IsImport = false,
                Date = e.ExportDate,
                
                // SỬA LỖI Ở ĐÂY: Thêm '?? 0'
                WarehouseId = e.WarehouseId ?? 0,

                WarehouseName = e.Warehouse.WarehouseName,
                UserName = e.User.FullName, 
                Reference = "N/A", 
                Note = e.Note
            });

        // 3. Hợp nhất hai danh sách
        var combinedList = imports.Concat(exports);

        // 4. Áp dụng bộ lọc (Code này của bạn đã ĐÚNG)
        var fromDate = dpFromDate.SelectedDate ?? DateTime.MinValue;
        var toDate = dpToDate.SelectedDate?.AddDays(1) ?? DateTime.MaxValue;
        combinedList = combinedList.Where(h => h.Date >= fromDate && h.Date < toDate);

        string type = cbTypeFilter.SelectedItem as string;
        if (type == "Phiếu Nhập")
        {
            combinedList = combinedList.Where(h => h.IsImport);
        }
        else if (type == "Phiếu Xuất")
        {
            combinedList = combinedList.Where(h => !h.IsImport);
        }

        if (cbWarehouseFilter.SelectedValue != null && (int)cbWarehouseFilter.SelectedValue != 0)
        {
            int warehouseId = (int)cbWarehouseFilter.SelectedValue;
            combinedList = combinedList.Where(h => h.WarehouseId == warehouseId);
        }

        dgHistory.ItemsSource = combinedList.OrderByDescending(h => h.Date).ToList();
    }
}

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadHistory();
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (sender as FrameworkElement)?.DataContext as HistoryItemViewModel;
            if (selectedItem == null) return;

            HistoryDetailWindow detailWindow = new HistoryDetailWindow(selectedItem.RecordId, selectedItem.IsImport);

            detailWindow.Owner = Window.GetWindow(this);
            detailWindow.ShowDialog();
        }

        
    }
}