using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace GovOperationsHub.Application.Features.Contracts.Queries
{
    public class ContractListDto
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string MinistryName { get; set; } = string.Empty;
        public string ContractTypeArabic { get; set; } = string.Empty;
        public decimal TotalBudget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public record GetContractsQuery : IRequest<Result<List<ContractListDto>>>;

    public class GetContractsQueryHandler : IRequestHandler<GetContractsQuery, Result<List<ContractListDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetContractsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<ContractListDto>>> Handle(GetContractsQuery request, CancellationToken cancellationToken)
        {
            var contracts = await _context.Contracts
                .Include(c => c.Ministry)
                .AsNoTracking()
                .OrderByDescending(c => c.Id)
                .Select(c => new ContractListDto
                {
                    Id = c.Id,
                    ContractNumber = c.ContractNumber,
                    Title = c.Title,
                    MinistryName = c.Ministry.Name,
                    ContractTypeArabic = GetDisplayName(c.ContractType),
                    TotalBudget = c.TotalBudget,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                })
                .ToListAsync(cancellationToken);

            return Result<List<ContractListDto>>.Success(contracts);
        }

        private static string GetDisplayName(Enum value) =>
            value.GetType().GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}
