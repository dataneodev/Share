using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace ProfiBiznes.Shared.UnitTests.Application.Extensions
{
    public sealed class ContextInitiatorTests
    {
        private readonly IHttpContextAccessor _httpContextInitiatorMock;

        public ContextInitiatorTests() => _httpContextInitiatorMock = Substitute.For<IHttpContextAccessor>();

        [Fact]
        public void AccountId_NoAccountIdClaim_ThrowsUnauthorizedException()
        {
            // Arrange
            // _httpContextInitiatorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            //
            // // Act
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock.Object);
            //
            // // Assert
            // Assert.Throws<UnauthorizedException>(() => contextInitiator.Common.AccountId);
        }

        [Fact]
        public void AccountType_NoAccountTypeClaim_ThrowsUnauthorizedException()
        {
            // // Arrange
            // _httpContextInitiatorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            //
            // // Act
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock.Object);
            //
            // // Assert
            // Assert.Throws<UnauthorizedException>(() => contextInitiator.AccountType);
        }

        [Fact]
        public void CompanyId_NoCompanyIdClaim_ThrowsUnauthorizedException()
        {
            // Arrange
            // _httpContextInitiatorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            //
            // // Act
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock.Object);
            //
            // // Assert
            // Assert.Throws<UnauthorizedException>(() => contextInitiator.CompanyId);
        }

        [Fact]
        public void Constructor_NoClaims_ReturnsEmptyContext()
        {
            // // Arrange
            // _httpContextInitiatorMock.HttpContext.Returns(new DefaultHttpContext());
            //
            // // Act
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock);
            //
            // // Assert
            // Assert.Null(contextInitiator.Common.AccountId);
            // Assert.Null(contextInitiator.AccountType);
            // Assert.Null(contextInitiator.CompanyId);
            // Assert.False(contextInitiator.IsAvailable);
            // Assert.False(contextInitiator.Is(AccountType.Administrator));
            // Assert.False(contextInitiator.HasPermission(Permission.View));
        }

        [Fact]
        public void Constructor_WithClaims_ReturnsContextWithClaims()
        {
            // Arrange
            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.NameIdentifier, "1"),
            //     new Claim(ClaimTypes.Role, "Admin"),
            //     new Claim("CompanyId", "2")
            // };
            // _httpContextInitiatorMock.HttpContext.Returns(new DefaultHttpContext
            // {
            //     User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            // });
            //
            // // Act
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock);
            //
            // // Assert
            // Assert.Equal(new AccountId(1), contextInitiator.Common.AccountId);
            // Assert.Equal(AccountType.Administrator, contextInitiator.AccountType);
            // Assert.Equal(new CompanyId(2), contextInitiator.CompanyId);
            // Assert.True(contextInitiator.IsAvailable);
            // Assert.True(contextInitiator.Is(AccountType.Administrator));
            // Assert.False(contextInitiator.Is(AccountType.User));
            // Assert.False(contextInitiator.HasPermission(Permission.View));
        }

        [Fact]
        public void Constructor_WithInvalidPermissionsHeader_ReturnsContextWithoutPermissions()
        {
            // Arrange
            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.NameIdentifier, "1"),
            //     new Claim(ClaimTypes.Role, "Admin"),
            //     new Claim("CompanyId", "2")
            // };
            // var headers = new HeaderDictionary { { "Permissions", "invalid" } };
            // _httpContextInitiatorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            // {
            //     User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
            //     Request = new HttpRequestWrapper(new HttpRequest(null, "http://localhost", null)
            //     {
            //         Headers = headers
            //     })
            // });
            //
            // // Act
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock.Object);
            //
            // // Assert
            // Assert.Empty(contextInitiator.GetPermissions());
            // Assert.False(contextInitiator.HasPermission(Permission.View));
        }

        [Fact]
        public void Constructor_WithPermissionsHeader_ReturnsContextWithPermissions()
        {
            // Arrange
            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.NameIdentifier, "1"),
            //     new Claim(ClaimTypes.Role, "Admin"),
            //     new Claim("CompanyId", "2")
            // };
            // var permissions = new List<int> { 1, 2, 3 };
            // var json = JsonConvert.SerializeObject(permissions);
            // var headers = new HeaderDictionary { { "Permissions", json } };
            // _httpContextInitiatorMock.HttpContext.Returns(new DefaultHttpContext
            // {
            //     User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
            //     Request = new HttpRequestWrapper(new HttpRequest(null, "http://localhost", null)
            //     {
            //         Headers = headers
            //     })
            // });
            //
            // // Act
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock);
            //
            // // Assert
            // Assert.Equal(permissions.SelectList(Permission.FromId), contextInitiator.GetPermissions());
            // Assert.True(contextInitiator.HasPermission(Permission.View));
            // Assert.True(contextInitiator.HasPermission(Permission.Edit));
            // Assert.True(contextInitiator.HasPermission(Permission.Delete));
            // Assert.False(contextInitiator.HasPermission(Permission.Create));
        }

        [Fact]
        public void MustBe_NotAvailable_ThrowsUnauthorizedException()
        {
            // Arrange
            // _httpContextInitiatorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock.Object);
            //
            // // Act & Assert
            // Assert.Throws<UnauthorizedException>(() => contextInitiator.MustBe(AccountType.Administrator));
        }

        [Fact]
        public void MustBe_NotMatchingType_ThrowsNoPermissionException()
        {
            // // Arrange
            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.NameIdentifier, "1"),
            //     new Claim(ClaimTypes.Role, "User"),
            //     new Claim("CompanyId", "2")
            // };
            // _httpContextInitiatorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            // {
            //     User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            // });
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock.Object);
            //
            // // Act & Assert
            // Assert.Throws<NoPermissionException>(() => contextInitiator.MustBe(AccountType.Administrator));
        }

        [Fact]
        public void ValidatePermission_MatchingPermission_DoesNotThrowException()
        {
            // Arrange
            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.NameIdentifier, "1"),
            //     new Claim(ClaimTypes.Role, "Admin"),
            //     new Claim("CompanyId", "2")
            // };
            // var permissions = new List<int> { 1, 2, 3 };
            // var json = JsonConvert.SerializeObject(permissions);
            // var headers = new HeaderDictionary { { "Permissions", json } };
            // _httpContextInitiatorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            // {
            //     User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
            //     Request = new HttpRequestWrapper(new HttpRequest(null, "http://localhost", null)
            //     {
            //         Headers = headers
            //     })
            // });
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock.Object);
            //
            // // Act & Assert
            // contextInitiator.ValidatePermission(Permission.View);
        }

        [Fact]
        public void ValidatePermission_NoMatchingPermission_ThrowsNoPermissionException()
        {
            // Arrange
            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.NameIdentifier, "1"),
            //     new Claim(ClaimTypes.Role, "Admin"),
            //     new Claim("CompanyId", "2")
            // };
            // _httpContextInitiatorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            // {
            //     User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            // });
            // var contextInitiator = ContextInitiator.User(_httpContextInitiatorMock.Object);
            //
            // // Act & Assert
            // Assert.Throws<NoPermissionException>(() => contextInitiator.ValidatePermission(Permission.Create));
        }
    }
}