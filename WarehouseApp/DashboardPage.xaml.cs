using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseApp.Models; // Đảm bảo using namespace Models

namespace WarehouseApp
{
    public partial class DashboardPage : Page
    {
        // Đặt mức tồn kho tối thiểu
        private const int LOW_STOCK_THRESHOLD = 5;

        public DashboardPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        /// <summary>
        /// Tải dữ liệu cho các thẻ KPI và DataGrid
        /// </summary>
        private void LoadDashboardData()
        {
            using (var context = new WarehouseDbContext())
            {
                try
                {
                    var totalStock = context.Products.Sum(p => (int?)p.Quantity) ?? 0;
                    tbTotalStock.Text = totalStock.ToString("N0");

                    var lowStockProducts = context.Products
                        .Include(p => p.Category)
                        .Where(p => p.Quantity <= LOW_STOCK_THRESHOLD)
                        .OrderBy(p => p.Quantity) 
                        .ToList();

                    tbWarningCount.Text = lowStockProducts.Count.ToString();
                    dgLowStockItems.ItemsSource = lowStockProducts;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải dữ liệu Dashboard: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}