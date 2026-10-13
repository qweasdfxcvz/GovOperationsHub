using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;


namespace GovOperationsHub.Application.Features.Ministries.Commands.DeleteMinistry
{
    public record DeleteMinistryCommand(int Id) : IRequest<Result<bool>>;

    public class DeleteMinistryCommandHandler : IRequestHandler<DeleteMinistryCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteMinistryCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<bool>> Handle(DeleteMinistryCommand request, CancellationToken cancellationToken)
        {
            var ministry = await _context.Ministries.FindAsync(new object[] { request.Id }, cancellationToken);

            if (ministry == null)
                return Result<bool>.Failure("الوزارة المحددة غير موجودة.");

            // الدالة Remove ستقوم بتفعيل الحذف المرن تلقائياً بفضل الـ DbContext الذي برمجناه
            _context.Ministries.Remove(ministry);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "تم حذف الوزارة بنجاح.");
        }
    }
}
