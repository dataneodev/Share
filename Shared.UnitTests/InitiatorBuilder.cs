using ProfiBiznes.Shared.Application.Security;
using ProfiBiznes.Shared.Application.Security.Initiator;
using ProfiBiznes.Shared.Domain.Enumerations;
using ProfiBiznes.Shared.Domain.ValueObjects;

namespace ProfiBiznes.Shared.UnitTests
{
    public static class InitiatorBuilder
    {
        public static Initiator User =>
            ForUser(
                new AccountId(1),
                new CompanyId(1),
                new List<LicenseType> { LicenseType.ERP },
                new List<Permission> { Permission.Common_Application_Read },
                language: Language.Polish
            );

        public static Initiator ForAdmin(AccountId accountId, string? name = null) =>
            Initiator.ForAdmin(AccountSessionToken.New, accountId, name ?? "Administrator", Language.Polish, TimeZoneInfo.Utc);

        public static Initiator ForSystem() => Initiator.ForSystem();

        public static Initiator ForUser(
            AccountId accountId,
            CompanyId companyId,
            IEnumerable<LicenseType>? licenses = null,
            IEnumerable<Permission>? permissions = null,
            IEnumerable<BranchId>? branches = null,
            Language? language = null,
            string? name = null,
            bool isCompanyManager = false,
            TimeZoneInfo? timeZoneInfo = null
        ) =>
            Initiator.ForUser(
                AccountSessionToken.New,
                accountId,
                companyId,
                licenses ?? [],
                permissions ?? [],
                branches ?? [],
                language ?? Language.GetFallback(),
                name ?? "Tester 1",
                isCompanyManager,
                timeZoneInfo ?? TimeZoneInfo.Utc
            );
    }
}