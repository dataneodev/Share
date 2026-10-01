using ProfiBiznes.Shared.Application.Security.Initiator;
using ProfiBiznes.Shared.Domain.ValueObjects;

namespace ProfiBiznes.Shared.UnitTests.Application
{
    public abstract class ApplicationTestsToolkit
    {
        protected static readonly CompanyId CompanyId = new(1);
        protected static readonly BranchId BranchId = new(1);
        protected static readonly Guid CorrelationId = Guid.NewGuid();
        protected static readonly DateTimeOffset OccurredAt = DateTimeOffset.UtcNow;
        protected static readonly Initiator UserInitiator = InitiatorBuilder.ForUser(new AccountId(1), CompanyId, branches: [BranchId]);
        protected static readonly Initiator AdminInitiator = InitiatorBuilder.ForAdmin(new AccountId(1));
        protected static readonly Initiator.InitiatorJson UserInitiatorJson = UserInitiator.ToJson();
    }
}