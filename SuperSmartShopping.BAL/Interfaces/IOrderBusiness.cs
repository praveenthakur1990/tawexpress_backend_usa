using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IOrderBusiness
    {
        string GenerateOrderNumber(string connectionStr);
        int SaveOrder(OrderModel model, string connectionStr);
        List<OrderInfoModel> GetOrder(int orderId, string orderNo, string orderBy, string status, bool IsCurrentDate, PaginationModel pageModel, string connectionStr);
        List<OrderDetailInfoModel> GetOrderDetail(int orderId, string connectionStr);
        int UpdateOrderStatus(int orderId, string status, string userId, string connectionStr);
        List<OrderStatusLogs> GetOrderStatusLogs(int orderId, string connectionStr);

        List<OrderInfoModel> GetOrderByIds(string orderIds, string orderNo, string orderBy, string status, bool IsCurrentDate, PaginationModel pageModel, string connectionStr);


    }
}
