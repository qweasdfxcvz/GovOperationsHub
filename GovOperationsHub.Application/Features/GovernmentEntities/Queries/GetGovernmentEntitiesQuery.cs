using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace GovOperationsHub.Application.Features.GovernmentEntities.Queries
{
    public class GovernmentEntityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityTypeArabic { get; set; } = string.Empty; // لجلب الاسم العربي للهيئة/المؤسسة
        public string DirectorName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public int MinistryId { get; set; }
        public string MinistryName { get; set; } = string.Empty; // اسم الوزارة التابعة لها

        public int ContactsCount { get; set; }
    }

    public record GetGovernmentEntitiesQuery : IRequest<Result<List<GovernmentEntityDto>>>;

    public class GetGovernmentEntitiesQueryHandler : IRequestHandler<GetGovernmentEntitiesQuery, Result<List<GovernmentEntityDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetGovernmentEntitiesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<GovernmentEntityDto>>> Handle(GetGovernmentEntitiesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _context.GovernmentEntities
                .Include(g => g.Ministry) // لجلب اسم الوزارة
                .AsNoTracking()
                .Select(g => new GovernmentEntityDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    EntityTypeArabic = GetDisplayName(g.EntityType),
                    DirectorName = g.DirectorName,
                    Address = g.Address,
                    MinistryId = g.MinistryId,
                    MinistryName = g.Ministry.Name,
                    ContactsCount = g.Contacts.Count
                })
                .ToListAsync(cancellationToken);

            return Result<List<GovernmentEntityDto>>.Success(entities);
        }

        private static string GetDisplayName(Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName() ?? value.ToString();
        }
    }
}
