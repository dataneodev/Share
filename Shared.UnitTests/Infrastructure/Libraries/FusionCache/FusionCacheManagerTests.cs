using ProfiBiznes.Shared.Infrastructure.Caching;
using ZiggyCreatures.Caching.Fusion;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Libraries.FusionCache
{
    public sealed class FusionCacheManagerTests
    {
        private readonly ICacheManager _cache = new FusionCacheManager(new ZiggyCreatures.Caching.Fusion.FusionCache(new FusionCacheOptions()));

        [Fact]
        public async Task GetOrCreateAsync_GivenIntKey_ShouldReturnCachedValue_WhenKeyExists()
        {
            // Arrange
            var value = "cachedValue";
            var key = 123;

            // Act
            var item = await _cache.GetOrCreateAsync(
                nameof(GetOrCreateAsync_GivenIntKey_ShouldReturnCachedValue_WhenKeyExists),
                key,
                () => Task.FromResult(value)
            );

            item = await _cache.GetOrCreateAsync(nameof(GetOrCreateAsync_GivenIntKey_ShouldReturnCachedValue_WhenKeyExists), key, () => Task.FromResult(value));

            // Assert
            Assert.Equal(value, item);
        }

        [Fact]
        public async Task GetOrCreateAsync_GivenObjectKey_ShouldNotCreateDuplicateCache_WhenKeyExists()
        {
            // Arrange
            var value1 = "cachedValue1";
            var value2 = "cachedValue2";
            var key1 = new CacheKey
            {
                Key = "key",
                Key2 = 1,
                Key3 = 1.1m,
                Key4 = [1, 2, 3],
                Key5 = ["a", "b", "c"],
                Key6 = [new CacheKey.CacheKey_Child("child1", "child2"), new CacheKey.CacheKey_Child("child3", "child4")],
                Key7 = new CacheKey.CacheKey_Child("child5", "child6")
            };

            var key2 = key1 with { Key = "key2" };

            // Act
            var item1 = await _cache.GetOrCreateAsync(
                nameof(GetOrCreateAsync_GivenObjectKey_ShouldNotCreateDuplicateCache_WhenKeyExists),
                key1,
                () => Task.FromResult(value1)
            );

            var item2 = await _cache.GetOrCreateAsync(
                nameof(GetOrCreateAsync_GivenObjectKey_ShouldNotCreateDuplicateCache_WhenKeyExists),
                key2,
                () => Task.FromResult(value2)
            );

            // Assert
            Assert.Equal(value1, item1);
            Assert.Equal(value2, item2);
            Assert.NotEqual(key1, key2);
            Assert.NotEqual(item1, item2);
        }

        [Fact]
        public async Task GetOrCreateAsync_GivenObjectKey_ShouldReturnCachedValue_WhenKeyExists()
        {
            // Arrange
            var value = "cachedValue";
            var key = new CacheKey
            {
                Key = "key",
                Key2 = 1,
                Key3 = 1.1m,
                Key4 = [1, 2, 3],
                Key5 = ["a", "b", "c"],
                Key6 = [new CacheKey.CacheKey_Child("child1", "child2"), new CacheKey.CacheKey_Child("child3", "child4")],
                Key7 = new CacheKey.CacheKey_Child("child5", "child6")
            };

            // Act
            var item = await _cache.GetOrCreateAsync(
                nameof(GetOrCreateAsync_GivenObjectKey_ShouldReturnCachedValue_WhenKeyExists),
                key,
                () => Task.FromResult(value)
            );

            item = await _cache.GetOrCreateAsync(
                nameof(GetOrCreateAsync_GivenObjectKey_ShouldReturnCachedValue_WhenKeyExists),
                key,
                () => Task.FromResult(value)
            );

            // Assert
            Assert.Equal(value, item);
        }

        [Fact]
        public async Task GetOrCreateAsync_GivenStringKey_ShouldReturnCachedValue_WhenKeyExists()
        {
            // Arrange
            var value = "cachedValue";
            var key = "stringKey";

            // Act
            var item = await _cache.GetOrCreateAsync(
                nameof(GetOrCreateAsync_GivenStringKey_ShouldReturnCachedValue_WhenKeyExists),
                key,
                () => Task.FromResult(value)
            );

            item = await _cache.GetOrCreateAsync(
                nameof(GetOrCreateAsync_GivenStringKey_ShouldReturnCachedValue_WhenKeyExists),
                key,
                () => Task.FromResult(value)
            );

            // Assert
            Assert.Equal(value, item);
        }

        private sealed record CacheKey
        {
            public CacheKey_Child Key7 { get; set; }

            public decimal Key3 { get; set; }

            public int Key2 { get; set; }

            public List<CacheKey_Child> Key6 { get; set; }

            public List<int> Key4 { get; set; }

            public List<string> Key5 { get; set; }

            public string Key { get; set; }

            public sealed record CacheKey_Child(string ChildKey1, string ChildKey2);
        }
    }
}