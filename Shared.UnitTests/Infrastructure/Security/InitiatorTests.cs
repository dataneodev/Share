using FluentAssertions;
using ProfiBiznes.Shared.Application.Exceptions;
using ProfiBiznes.Shared.Application.Security;
using ProfiBiznes.Shared.Application.Security.Initiator;
using ProfiBiznes.Shared.Domain.Enumerations;
using ProfiBiznes.Shared.Domain.ValueObjects;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Security
{
    public sealed class InitiatorTests
    {
        [Fact]
        public void AccountId_CorrectContextInitiator_ReturnsAccountId()
        {
            // Arrange
            var accountId = new AccountId(1);
            var initiator = InitiatorBuilder.ForAdmin(accountId);

            // Act
            var initiatorAccountId = initiator.AccountId;

            // Assert
            initiatorAccountId.Should()
                .Be(accountId);
        }

        [Fact]
        public void AccountId_IncorrectContextInitiator_ThrowsUnauthorizedException()
        {
            var initiator = InitiatorBuilder.ForSystem();

            // Act
            var act = () => initiator.AccountId;

            // Assert
            act.Should()
                .Throw<UnauthorizedException>();
        }

        [Fact]
        public void AccountType_CorrectContextInitiator_ReturnsAccountType()
        {
            // Arrange
            var initiator = Initiator.ForSystem();

            // Act
            var initiatorAccountType = initiator.Type;

            // Assert
            initiatorAccountType.Should()
                .Be(InitiatorType.System);
        }

        [Fact]
        public void AccountType_IncorrectContextInitiator_ThrowsUnauthorizedException()
        {
            var initiator = InitiatorBuilder.ForSystem();

            // Act
            var act = () => initiator.AccountId;

            // Assert
            act.Should()
                .Throw<UnauthorizedException>();
        }

        [Fact]
        public void CompanyId_CorrectContextInitiator_ReturnsCompanyId()
        {
            // Arrange
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);

            var initiator = InitiatorBuilder.ForUser(accountId, companyId);

            // Act
            var initiatorCompanyId = initiator.CompanyId;

            // Assert
            initiatorCompanyId.Should()
                .Be(companyId);
        }

        [Fact]
        public void CompanyId_IncorrectContextInitiator_ThrowsUnauthorizedException()
        {
            var initiator = InitiatorBuilder.ForSystem();

            // Act
            var act = () => initiator.CompanyId;

            // Assert
            act.Should()
                .Throw<UnauthorizedException>();
        }

        [Fact]
        public void Is_CorrectContextInitiator_ReturnsFalseWhenAccountTypeDoesNotMatch()
        {
            // Arrange
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var initiator = InitiatorBuilder.ForUser(accountId, companyId);


            // Act
            var result = initiator.Is.Admin;

            // Assert
            result.Should()
                .BeFalse();
        }

        [Fact]
        public void Is_CorrectContextInitiator_ReturnsTrueWhenAccountTypeMatches()
        {
            // Arrange
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var initiator = InitiatorBuilder.ForUser(accountId, companyId);

            // Act
            var result = initiator.Is.User;

            // Assert
            result.Should()
                .BeTrue();
        }

        [Fact]
        public void IsAvailable_CorrectContextInitiator_ReturnsTrue()
        {
            // Arrange
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var initiator = InitiatorBuilder.ForUser(accountId, companyId);


            // Act
            var isAvailable = initiator.HasAccount;

            // Assert
            isAvailable.Should()
                .BeTrue();
        }

        [Fact]
        public void IsAvailable_IncorrectContextInitiator_ReturnsFalse()
        {
            var initiator = InitiatorBuilder.ForSystem();

            // Act
            var isAvailable = initiator.HasAccount;

            // Assert
            isAvailable.Should()
                .BeFalse();
        }

        [Fact]
        public void Language_CorrectContextInitiator_ReturnsLanguage()
        {
            // Arrange
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var language = Language.English;

            var initiator = InitiatorBuilder.ForUser(accountId, companyId, language: language);

            // Act
            var initiatorLanguage = initiator.Language;

            // Assert
            initiatorLanguage.Should()
                .Be(language);
        }

        [Fact]
        public void MustBeOneOf_CorrectContextInitiator_DoesNotThrowWhenTypeMatch()
        {
            // Arrange
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var initiator = InitiatorBuilder.ForUser(accountId, companyId);

            // Act
            var act = () => initiator.MustHave.AccountId();

            // Assert
            act.Should()
                .NotThrow();
        }

        [Fact]
        public void MustBeOneOf_CorrectContextInitiator_ThrowsWhenTypeDoNotMatch()
        {
            // Arrange
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var initiator = InitiatorBuilder.ForUser(accountId, companyId);

            // Act
            var act = () => initiator.MustBe.Admin();

            // Assert
            act.Should()
                .Throw<NoPermissionException>();
        }

        [Fact]
        public void MustHave_CorrectInitiator_DoesNotThrow()
        {
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var license = LicenseType.API;
            var permissions = new[] { Permission.Management_Users_Update };
            var branchId = new BranchId(1);
            var initiator = InitiatorBuilder.ForUser(accountId, companyId, [license], permissions, [branchId]);

            // Act
            var act = () => initiator.User.MustHave(license, permissions: permissions, branchId: branchId);

            // Assert
            act.Should()
                .NotThrow();
        }

        [Fact]
        public void MustHave_DifferentBranchId_ThrowsNoPermissionException()
        {
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var license = LicenseType.API;
            var permissions = new[] { Permission.Management_Users_Update };
            var branchId = new BranchId(1);
            var initiator = InitiatorBuilder.ForUser(accountId, companyId, [license], permissions, [branchId]);

            // Act
            var act = () => initiator.User.MustHave(license, permissions: permissions, branchId: new BranchId(2));

            // Assert
            act.Should()
                .Throw<NoPermissionException>();
        }

        [Fact]
        public void MustHave_DifferentLicense_ThrowsNoPermissionException()
        {
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var license = LicenseType.API;
            var permissions = new[] { Permission.Management_Users_Update };
            var initiator = InitiatorBuilder.ForUser(accountId, companyId, [license], permissions);

            // Act
            var act = () => initiator.User.MustHave(LicenseType.BI, permissions: permissions);

            // Assert
            act.Should()
                .Throw<NoPermissionException>();
        }

        [Fact]
        public void MustHave_DifferentPermissionAndIsCompanyManager_DoesNotThrow()
        {
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var license = LicenseType.API;
            var permissions = Array.Empty<Permission>();

            var initiator = InitiatorBuilder.ForUser(accountId, companyId, [license], permissions, isCompanyManager: true);

            // Act
            var act = () => initiator.User.MustHave(license, permissions: [Permission.Management_Users_Update]);

            // Assert
            act.Should()
                .NotThrow();
        }

        [Fact]
        public void MustHave_DifferentPermissions_ThrowsNoPermissionException()
        {
            var accountId = new AccountId(1);
            var companyId = new CompanyId(1);
            var license = LicenseType.API;
            var permissions = new[] { Permission.Management_Users_Update };
            var initiator = InitiatorBuilder.ForUser(accountId, companyId, [license], permissions);


            // Act
            var act = () => initiator.User.MustHave(license, permissions: [Permission.Accounting_Issuement_Create]);

            // Assert
            act.Should()
                .Throw<NoPermissionException>();
        }
    }
}