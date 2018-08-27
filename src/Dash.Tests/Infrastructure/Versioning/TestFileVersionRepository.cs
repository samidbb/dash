using System;
using System.Linq;
using Dash.Infrastructure;
using Dash.Infrastructure.Versioning;
using Dash.Tests.TestHelpers;
using Xunit;

namespace Dash.Tests.Infrastructure.Versioning
{
    public class TestFileVersionRepository
    {
        [Fact]
        public void Can_add_new_file_version()
        {
            var sut = A.FileVersionRepository.Build();

            var dummy = A.FileVersion
                .WithEntry("dummy_entry")
                .Build();

            sut.Save(dummy);

            var result = sut.GetFileVersionList().Single();

            Assert.Equal(dummy, result);
        }

        [Fact]
        public void Can_overwrite_existing_file_version()
        {
            var sut = A.FileVersionRepository.Build();

            var dummy = A.FileVersion
                .WithEntry("e")
                .Build();

            sut.Save(dummy);

            var result = sut.GetFileVersionList().Single();

            Assert.Equal(dummy, result);
        }

        [Fact]
        public void Can_remove_file_version()
        {
            var sut = A.FileVersionRepository.With(
                FileVersionParser.CreateNull(A.FileVersion.WithEntry("e"))
            ).Build();

            sut.Remove("e");

            var result = sut.GetFileVersionList();

            Assert.Empty(result);
        }
    }
}