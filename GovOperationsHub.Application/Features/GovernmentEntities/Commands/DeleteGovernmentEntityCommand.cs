using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Application.Features.GovernmentEntities.Commands
{
    public record DeleteGovernmentEntityCommand(int Id) : IRequest<Result<bool>>;

    public class DeleteGovernmentEntityCommandHandler : IRequestHandler<DeleteGovernmentEntityCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteGovernmentEntityCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<bool>> Handle(DeleteGovernmentEntityCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.GovernmentEntities.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
                return Result<bool>.Failure("المؤسسة المحددة غير موجودة.");

            _context.GovernmentEntities.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "تم حذف المؤسسة بنجاح.");
        }
    }
}
