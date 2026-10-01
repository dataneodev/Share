using FluentAssertions;
using ProfiBiznes.Shared.Infrastructure.Security;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Security
{
    public sealed class EncryptionTests
    {
        [Theory]
        [InlineData("test")]
        [InlineData("{\"Text\":\"halo\",\"Value\":123}")]
        public void Decrypt_EncryptedText_ReturnsDecryptedText(string text)
        {
            // Arrange
            var encrypted = Encryption.Encrypt(text);

            // Act
            var decrypted = Encryption.Decrypt(encrypted);

            // Assert
            decrypted.Should()
                .Be(text);
        }

        [Fact]
        public void Encryption_Text_ResultDoesntEqualOriginalText()
        {
            // Arrange
            const string text = "test";

            // Act
            var encrypted = Encryption.Encrypt(text);

            // Assert
            encrypted.Should()
                .NotBe(text);
        }

        [Fact]
        public void ReEncrypt_EncryptedTextWithOriginalKeyAndNewKey_ReEncryptsTextUsingNewKey()
        {
            // Arrange
            const string text = "test";
            const string key1 = "key1";
            const string key2 = "key2";

            var encrypted = Encryption.Encrypt(text, key1);

            // Act
            var reEncrypt = Encryption.ReEncrypt(encrypted, key1, key2);

            // Assert
            var decrypted = Encryption.Decrypt(reEncrypt, key2);

            encrypted.Should()
                .NotBe(text);

            reEncrypt.Should()
                .NotBe(text);

            decrypted.Should()
                .Be(text);
        }
    }
}