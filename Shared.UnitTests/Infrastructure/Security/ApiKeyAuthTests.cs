using ProfiBiznes.Shared.Application.Helpers;
using ProfiBiznes.Shared.Application.Security.Auth.Api;
using ProfiBiznes.Shared.Domain.Extensions;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Security
{
    public sealed class ApiKeyAuthTests
    {
        [Fact]
        public void CreateKey_GivenCorrectParameters_CreatesKey()
        {
            //arrange
            var id = 1;
            var expiration = DateTimeOffset.Now.AddYears(5);

            //act
            var key = ApiKeyAuth.CreateKey(id, "Testowe API", expiration);

            //assert
            Assert.NotNull(key);
        }

        [Fact]
        public void SymmetricKeyGenerator()
        {
            var keys = Enumerable.Range(0, 8)
                .SelectList(i => RandomStringGenerator.Generate(128));

            Assert.NotNull(keys);
        }
    }
}