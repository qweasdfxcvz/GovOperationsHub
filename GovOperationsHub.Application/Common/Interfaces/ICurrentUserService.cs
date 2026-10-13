using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
    }
}
