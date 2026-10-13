using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Application.Features.Ministries.Queries.GetMinistries
{
    public class MinistryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string MinisterOrRepresentative { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int ContactsCount { get; set; }
    }

    public record GetMinistriesQuery : IRequest<Result<List<MinistryDto>>>;

    public class GetMinistriesQueryHandler : IRequestHandler<GetMinistriesQuery, Result<List<MinistryDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetMinistriesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<MinistryDto>>> Handle(GetMinistriesQuery request, CancellationToken cancellationToken)
        {
            var ministries = await _context.Ministries
                .AsNoTracking()
                .Select(m => new MinistryDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    MinisterOrRepresentative = m.MinisterOrRepresentative,
                    Address = m.Address,
                    ContactsCount = m.Contacts.Count
                })
                .ToListAsync(cancellationToken);

            return Result<List<MinistryDto>>.Success(ministries);
        }
    }
}
