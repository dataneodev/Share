using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Shared.UnitTests.MartenExtensions
{
    public sealed class DocumentStoreBatchPatchExtensionsTest
    {
        [Fact]
        public void GetCorrectPages()
        {
            var total = 1200;
            var pageSize = 11;

            var pages = GetPages(pageSize, total);
            pages.Should().HaveCount(110);
        }

        [Fact]
        public void GetCorrectPagesOnEmpty()
        {
            var total = 0;
            var pageSize = 11;

            GetPages(pageSize, total).Should().HaveCount(0);
        }

        private static IEnumerable<int> GetPages(int pageSize, int totalCount) =>
            totalCount == 0
                ? Enumerable.Empty<int>()
                : Enumerable.Range(0, (int)Math.Ceiling(totalCount/ (double)pageSize));

        private static IEnumerable<T> GetPage<T>(IEnumerable<T> input, int page, int pagesize) => input.Skip(page * pagesize).Take(pagesize);

    }
}
