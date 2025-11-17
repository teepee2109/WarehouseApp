using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using WarehouseApp.Models; // Đảm bảo using namespace Models

namespace WarehouseApp
{
    public partial class HistoryDetailWindow : Window
    {
        private int _recordId;
        private bool _isImport;

        /// <summary>
        /// Constructor nhận ID và loại phiếu từ trang Lịch sử
        /// </summary>
        public HistoryDetailWindow(int recordId, bool isImport)
        {
            InitializeComponent();
            _recordId = recordId;
            _isImport = isImport;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var context = new WarehouseDbContext())
            {
                if (_isImport)
                {
                    // Tải chi tiết phiếu NHẬP
                    Title = $"Chi tiết Phiếu Nhập #{_recordId}";
                    var details = context.ImportOrderDetails
                        .Include(d => d.Product) // JOIN để lấy tên sản phẩm
                        .Where(d => d.ImportId == _recordId)
                        .Select(d => new
                        {
                            d.Product.ProductName,
                            d.Quantity,
                            d.Price,
                            Total = d.Quantity * d.Price
                        })
                        .ToList();
                    dgDetails.ItemsSource = details;
                }
                else
                {
                    // Tải chi tiết phiếu XUẤT
                    Title = $"Chi tiết Phiếu Xuất #{_recordId}";
                    var details = context.ExportOrderDetails
                        .Include(d => d.Product)
                        .Where(d => d.ExportId == _recordId)
                        .Select(d => new
                        {
                            d.Product.ProductName,
                            d.Quantity,
                            // Phiếu xuất không có giá, gán null để khớp cột
                            Price = (decimal?)null,
                            Total = (decimal?)null
                        })
                        .ToList();

                    dgDetails.ItemsSource = details;

                    // Ẩn cột Giá và Thành tiền vì phiếu xuất không có
                    dgDetails.Columns[2].Visibility = Visibility.Collapsed; // Cột Giá
                    dgDetails.Columns[3].Visibility = Visibility.Collapsed; // Cột Thành tiền
                }
            }
        }
    }
}