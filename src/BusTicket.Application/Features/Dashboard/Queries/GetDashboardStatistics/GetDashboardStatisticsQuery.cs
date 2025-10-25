using BusTicket.Application.Contracts.Features.Dashboard;
using MediatR;

namespace BusTicket.Application.Features.Dashboard.Queries.GetDashboardStatistics;

public record GetDashboardStatisticsQuery : IRequest<DashboardStatisticsDto>;
