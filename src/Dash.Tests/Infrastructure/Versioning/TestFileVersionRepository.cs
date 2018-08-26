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
        public void Has_correct_filename()
        {
            // TODO -- inject filename from settings
            Assert.Equal("dashboard_version.csv", FileVersionRepository.FileVersionCsvFileName);
        }

        [Fact]
        public void Has_correct_list_of_headers()
        {
            Assert.Equal("entry;hash;author_name;author_email;author_date;committer_name;committer_email;committer_date;message", FileVersionRepository.Headers);
        }

        [Theory]
        [InlineData("")]
        [InlineData(FileVersionRepository.Headers)]
        public void Can_parse_empty_version_entries(string input)
        {
            var sut = A.FileVersionRepository
                .With(FileSystem.CreateNull(A.File.WithContent(input)))
                .Build();

            var dashboardMetaEntries = sut.GetFileVersionList();

            Assert.Empty(dashboardMetaEntries);
        }

        [Fact]
        public void Can_parse_version_entries()
        {
            var sut = A.FileVersionRepository.With(
                FileSystem.CreateNull(
                    A.File.WithContent(
                        $"{FileVersionRepository.Headers}\n" +
                        "e;h;an;ae;2018-08-08 10:10:36 +0100;cn;ce;2018-08-08 10:10:36 +0100;m\n" +
                        "e;h;an;ae;2018-08-08 10:10:36 +0100;cn;ce;2018-08-08 10:10:36 +0100;m\n"
                    )
                )
            ).Build();

            var dashboardMetaEntries = sut.GetFileVersionList();

            Assert.Equal(2, dashboardMetaEntries.Count());
        }

        [Fact]
        public void Can_parse_dashboard_version_entry()
        {
            var sut = A.FileVersionRepository.With(
                FileSystem.CreateNull(
                    A.File.WithContent(
                        $"{FileVersionRepository.Headers}\n" +
                        "e;h;an;ae;2018-08-08 10:10:36 +0100;cn;ce;2018-08-08 10:10:36 +0100;m"
                    )
                )
            ).Build();

            var result = sut.GetFileVersionList().Single();

            FileVersionAssert.Equal(
                expected: A.FileVersion
                    .WithEntry("e")
                    .WithHash("h")
                    .WithAuthorName("an")
                    .WithAuthorEmail("ae")
                    .WithAuthorDate(new DateTime(2018, 8, 8, 9, 10, 36))
                    .WithCommitterName("cn")
                    .WithCommitterEmail("ce")
                    .WithCommitterDate(new DateTime(2018, 8, 8, 9, 10, 36))
                    .WithMessage("m"),
                actual: result);
        }

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
            var sut = A.FileVersionRepository.With(
                FileSystem.CreateNull(
                    A.File.WithContent(
                        $"{FileVersionRepository.Headers}\n" +
                        "e;h;an;ae;2018-08-08 10:10:36 +0100;cn;ce;2018-08-08 10:10:36 +0100;m"
                    )
                )
            ).Build();

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
                FileSystem.CreateNull(
                    A.File.WithContent(
                        $"{FileVersionRepository.Headers}\n" +
                        "e;h;an;ae;2018-08-08 10:10:36 +0100;cn;ce;2018-08-08 10:10:36 +0100;m"
                    )
                )
            ).Build();

            sut.Remove("e");

            var result = sut.GetFileVersionList();

            Assert.Empty(result);
        }
    }
}