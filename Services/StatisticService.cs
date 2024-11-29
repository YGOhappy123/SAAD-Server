using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Response;
using milktea_server.Extensions;
using milktea_server.Extensions.Mappers;
using milktea_server.Interfaces.Repositories;
using milktea_server.Interfaces.Services;
using milktea_server.Models;
using milktea_server.Utilities;

namespace milktea_server.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IMilkteaRepository _milkteaRepo;

        public StatisticService(ICustomerRepository customerRepo, IOrderRepository orderRepo, IMilkteaRepository milkteaRepo)
        {
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
            _milkteaRepo = milkteaRepo;
        }

        private class ChartParams
        {
            public int Columns { get; set; } = 1;
            public string TimeUnit { get; set; } = string.Empty;
            public string Format { get; set; } = string.Empty;

            public ChartParams(int columns, string timeUnit, string format)
            {
                Columns = columns;
                TimeUnit = timeUnit;
                Format = format;
            }
        }

        private class ChartItem
        {
            public DateTime Date { get; set; } = DateTime.Now;
            public string Name { get; set; } = string.Empty;
            public int TotalUnits { get; set; } = 0;
            public decimal TotalSales { get; set; } = 0;

            public ChartItem(DateTime date, string name, int totalUnits, decimal totalSales)
            {
                Date = date;
                Name = name;
                TotalUnits = totalUnits;
                TotalSales = totalSales;
            }
        }

        public async Task<ServiceResponse<object>> GetSummaryStatistic(string type)
        {
            var currentTime = TimestampHandler.GetNow();
            var previousTime = TimestampHandler.GetPreviousTimeByType(currentTime, type);
            var startOfCurrentTime = TimestampHandler.GetStartOfTimeByType(currentTime, type);
            var startOfPreviousTime = TimestampHandler.GetStartOfTimeByType(previousTime, type);

            var currUsersCount = await _customerRepo.CountCustomersCreatedInTimeRange(startOfCurrentTime, currentTime);
            var prevUsersCount = await _customerRepo.CountCustomersCreatedInTimeRange(startOfPreviousTime, startOfCurrentTime);

            var currOrdersCount = await _orderRepo.CountNewOrdersInTimeRange(startOfCurrentTime, currentTime);
            var prevOrdersCount = await _orderRepo.CountNewOrdersInTimeRange(startOfPreviousTime, startOfCurrentTime);

            var currRevenues = await _orderRepo.CountTotalRevenuesInTimeRange(startOfCurrentTime, currentTime);
            var prevRevenues = await _orderRepo.CountTotalRevenuesInTimeRange(startOfPreviousTime, startOfCurrentTime);

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = new
                {
                    Users = new { CurrentCount = currUsersCount, PreviousCount = prevUsersCount },
                    Orders = new { CurrentCount = currOrdersCount, PreviousCount = prevOrdersCount },
                    Revenues = new { CurrentCount = currRevenues, PreviousCount = prevRevenues },
                },
            };
        }

        public async Task<ServiceResponse<object>> GetPopularStatistic(string type)
        {
            var currentTime = TimestampHandler.GetNow();
            var startOfCurrentTime = TimestampHandler.GetStartOfTimeByType(currentTime, type);

            var bestSellers = await _milkteaRepo.GetBestSellers(startOfCurrentTime, currentTime, 5);
            var newestCustomers = await _customerRepo.GetNewestCustomers(startOfCurrentTime, currentTime, 5);
            var customersWithHighestTotal = await _customerRepo.GetCustomersWithHighestTotalOrderValue(startOfCurrentTime, currentTime, 5);

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = new
                {
                    ProductsWithHighestSales = bestSellers.Select(mt => new
                    {
                        mt.Id,
                        mt.NameVi,
                        mt.NameEn,
                        mt.DescriptionVi,
                        mt.DescriptionEn,
                        mt.Image,
                        mt.IsAvailable,
                        mt.IsActive,
                        mt.CreatedAt,
                        mt.CategoryId,
                        Price = new
                        {
                            S = mt.PriceS,
                            M = mt.PriceM,
                            L = mt.PriceL,
                        },
                        Quantity = mt.OrderItems.Sum(oi => oi.Quantity),
                        Amount = mt.OrderItems.Sum(oi => oi.Quantity * oi.Price),
                    }),
                    NewCustomers = newestCustomers.Select(cus => new
                    {
                        cus.Id,
                        cus.FirstName,
                        cus.LastName,
                        cus.Avatar,
                        cus.PhoneNumber,
                        cus.CreatedAt,
                    }),
                    UsersWithHighestTotalOrderValue = customersWithHighestTotal.Select(cus => new
                    {
                        cus.Id,
                        cus.FirstName,
                        cus.LastName,
                        cus.Avatar,
                        Amount = cus.Orders.Sum(od => od.TotalPrice),
                    }),
                },
            };
        }

        public async Task<ServiceResponse<object>> GetRevenuesChart(string type, string locale)
        {
            var currentTime = TimestampHandler.GetNow();
            var startOfCurrentTime = TimestampHandler.GetStartOfTimeByType(currentTime, type);

            var orders = await _orderRepo.GetAllOrdersInTimeRange(startOfCurrentTime, currentTime);
            var revenuesChart = CreateRevenuesChart(orders, startOfCurrentTime, PrepareCreateChartParams(type, startOfCurrentTime), locale);

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = revenuesChart.Select(ci => new
                {
                    Name = ci.Name.CapitalizeEachWords(),
                    ci.TotalUnits,
                    ci.TotalSales,
                }),
            };
        }

        private ChartParams PrepareCreateChartParams(string type, DateTime startDate)
        {
            return type.ToLower() switch
            {
                "daily" => new ChartParams(24, "hour", "HH:mm"),
                "weekly" => new ChartParams(7, "day", "dddd dd-MM"),
                "monthly" => new ChartParams(startDate.GetDaysInMonth(), "day", "dd"),
                "yearly" => new ChartParams(12, "month", "MMMM"),
                _ => throw new ArgumentException("Invalid type"),
            };
        }

        private List<ChartItem> CreateRevenuesChart(List<Order> orders, DateTime startDate, ChartParams chartParams, string locale)
        {
            CultureInfo cultureInfo = new CultureInfo(locale == "vi" ? "vi-VN" : "en-US");
            List<ChartItem> chartItems = [];

            for (int i = 0; i < chartParams.Columns; i++)
            {
                chartItems.Add(
                    new ChartItem(
                        startDate.AddByUnitAndAmount(i, chartParams.TimeUnit),
                        startDate.AddByUnitAndAmount(i, chartParams.TimeUnit).ToString(chartParams.Format, cultureInfo),
                        0,
                        0
                    )
                );
            }

            foreach (var order in orders)
            {
                var index = chartItems.FindIndex(item => TimestampHandler.IsSame(item.Date, order.CreatedAt, chartParams.TimeUnit));
                chartItems[index].TotalUnits += order.Items.Sum(oi => oi.Quantity);
                chartItems[index].TotalSales += order.TotalPrice;
            }

            return chartItems;
        }

        public async Task<ServiceResponse<object>> GetProductStatistic(int milkteaId)
        {
            var currentTime = TimestampHandler.GetNow();
            var startOfCurrentDay = TimestampHandler.GetStartOfTimeByType(currentTime, "daily");
            var startOfCurrentWeek = TimestampHandler.GetStartOfTimeByType(currentTime, "weekly");
            var startOfCurrentMonth = TimestampHandler.GetStartOfTimeByType(currentTime, "monthly");
            var startOfCurrentYear = TimestampHandler.GetStartOfTimeByType(currentTime, "yearly");

            var thisDayStatistic = await _milkteaRepo.GetMilkteaStatisticInTimeRange(startOfCurrentDay, currentTime, milkteaId);
            var thisWeekStatistic = await _milkteaRepo.GetMilkteaStatisticInTimeRange(startOfCurrentWeek, currentTime, milkteaId);
            var thisMonthStatistic = await _milkteaRepo.GetMilkteaStatisticInTimeRange(startOfCurrentMonth, currentTime, milkteaId);
            var thisYearStatistic = await _milkteaRepo.GetMilkteaStatisticInTimeRange(startOfCurrentYear, currentTime, milkteaId);

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = new
                {
                    Daily = new { TotalUnits = thisDayStatistic.Item1, TotalSales = thisDayStatistic.Item2 },
                    Weekly = new { TotalUnits = thisWeekStatistic.Item1, TotalSales = thisWeekStatistic.Item2 },
                    Monthly = new { TotalUnits = thisMonthStatistic.Item1, TotalSales = thisMonthStatistic.Item2 },
                    Yearly = new { TotalUnits = thisYearStatistic.Item1, TotalSales = thisYearStatistic.Item2 },
                },
            };
        }
    }
}
