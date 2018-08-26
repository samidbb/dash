using System;
using System.Linq;
using System.Text;
using Dash.Infrastructure;
using Dash.Infrastructure.Versioning;
using Dash.Tests.Infrastructure.Configuration;
using Xunit;
using Xunit.Sdk;

namespace Dash.Tests.Infrastructure.Versioning
{
    public class TestFileVersionRepository
    {
        [Fact]
        public void Has_correct_filename()
        {
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
            var sut = new FileVersionRepository(FileSystem.CreateNull(A.File.WithContent(input)));

            var dashboardMetaEntries = sut.GetFileVersionList();

            Assert.Empty(dashboardMetaEntries);
        }

        [Fact]
        public void Can_parse_version_entries()
        {
            var sut = new FileVersionRepository(
                FileSystem.CreateNull(
                    A.File.WithContent(
                        $"{FileVersionRepository.Headers}\n" +
                        "aws-account-billing.json;24083c9fae5cdcd5f707584ce126b98cd1472281;rifisdfds;40063756+rifisdfds@users.noreply.github.com;2018-08-08 10:10:36 +0100;GitHub;noreply@github.com;2018-08-08 10:10:36 +0100;Sorted graph by cost. Changed period to 90 days"
                    )
                )
            );

            var dashboardMetaEntries = sut.GetFileVersionList();

            Assert.Single(dashboardMetaEntries);
        }

        [Fact]
        public void Can_parse_dashboard_version_entry()
        {
            var sut = new FileVersionRepository(
                FileSystem.CreateNull(
                    A.File.WithContent(
                        $"{FileVersionRepository.Headers}\n" +
                        "aws-account-billing.json;24083c9fae5cdcd5f707584ce126b98cd1472281;rifisdfds;40063756+rifisdfds@users.noreply.github.com;2018-08-08 10:10:36 +0100;GitHub;noreply@github.com;2018-08-08 10:10:36 +0100;Sorted graph by cost. Changed period to 90 days"
                    )
                )
            );

            var result = sut.GetFileVersionList().FirstOrDefault();

            FileVersionAssert.Equal(
                expected: A.FileVersion
                    .WithEntry("aws-account-billing.json")
                    .WithHash("24083c9fae5cdcd5f707584ce126b98cd1472281")
                    .WithAuthorName("rifisdfds")
                    .WithAuthorEmail("40063756+rifisdfds@users.noreply.github.com")
                    .WithAuthorDate(new DateTime(2018, 8, 8, 9, 10, 36))
                    .WithCommitterName("GitHub")
                    .WithCommitterEmail("noreply@github.com")
                    .WithCommitterDate(new DateTime(2018, 8, 8, 9, 10, 36))
                    .WithMessage("Sorted graph by cost. Changed period to 90 days"),
                actual: result);
        }

        [Fact]
        public void Can_add_new_file_version()
        {
            var sut = new FileVersionRepository(FileSystem.CreateNull(A.File.WithContent("")));

            var dummy = A.FileVersion
                .WithEntry("dummy_entry")
                .Build();

            sut.Save(dummy);

            var result = sut.GetFileVersionList().FirstOrDefault();

            FileVersionAssert.Equal(
                expected: dummy,
                actual: result);
        }

        [Fact]
        public void Can_overwrite_existing_file_version()
        {
            var sut = new FileVersionRepository(
                FileSystem.CreateNull(
                    A.File.WithContent(
                        $"{FileVersionRepository.Headers}\n" +
                        "aws-account-billing.json;24083c9fae5cdcd5f707584ce126b98cd1472281;rifisdfds;40063756+rifisdfds@users.noreply.github.com;2018-08-08 10:10:36 +0100;GitHub;noreply@github.com;2018-08-08 10:10:36 +0100;Sorted graph by cost. Changed period to 90 days"
                    )
                )
            );

            var dummy = A.FileVersion
                .WithEntry("aws-account-billing.json")
                .Build();

            sut.Save(dummy);

            var result = sut.GetFileVersionList().FirstOrDefault();

            FileVersionAssert.Equal(
                expected: dummy,
                actual: result);
        }

        [Fact]
        public void Can_remove_file_version()
        {
            var sut = new FileVersionRepository(
                FileSystem.CreateNull(
                    A.File.WithContent(
                        $"{FileVersionRepository.Headers}\n" +
                        "aws-account-billing.json;24083c9fae5cdcd5f707584ce126b98cd1472281;rifisdfds;40063756+rifisdfds@users.noreply.github.com;2018-08-08 10:10:36 +0100;GitHub;noreply@github.com;2018-08-08 10:10:36 +0100;Sorted graph by cost. Changed period to 90 days"
                    )
                )
            );

            var dummy = A.FileVersion
                .WithEntry("aws-account-billing.json")
                .Build();

            sut.Remove("aws-account-billing.json");

            var result = sut.GetFileVersionList();

            Assert.Empty(result);
        }
    }

    internal static class FileVersionAssert
    {
        public static void Equal(FileVersion expected, FileVersion actual)
        {
            if (Equals(expected, actual))
            {
                return;
            }

            Assert.NotNull(expected);
            Assert.NotNull(actual);

            StringBuilder CreateStringBuilder()
            {
                var sb = new StringBuilder();
                sb.AppendLine("FileVersion with");
                return sb;
            }

            var expectedText = CreateStringBuilder();
            var actualText = CreateStringBuilder();
            var notEqual = false;

            void AppendLine(string format, object expectedValue, object actualValue)
            {
                if (!Equals(expectedValue, actualValue))
                {
                    notEqual = true;
                    expectedText.AppendLine(string.Format(format, expectedValue));
                    actualText.AppendLine(string.Format(format, actualValue));
                }
            }

            AppendLine($"\t.{nameof(FileVersion.Entry)}           = '{{0}}'", expected.Entry, actual.Entry);
            AppendLine($"\t.{nameof(FileVersion.Hash)}            = '{{0}}'", expected.Hash, actual.Hash);
            AppendLine($"\t.{nameof(FileVersion.AuthorName)}      = '{{0}}'", expected.AuthorName, actual.AuthorName);
            AppendLine($"\t.{nameof(FileVersion.AuthorEmail)}     = '{{0}}'", expected.AuthorEmail, actual.AuthorEmail);
            AppendLine($"\t.{nameof(FileVersion.AuthorDate)}      = '{{0}}'", expected.AuthorDate, actual.AuthorDate);
            AppendLine($"\t.{nameof(FileVersion.CommitterName)}   = '{{0}}'", expected.CommitterName, actual.CommitterName);
            AppendLine($"\t.{nameof(FileVersion.CommitterEmail)}  = '{{0}}'", expected.CommitterEmail, actual.CommitterEmail);
            AppendLine($"\t.{nameof(FileVersion.CommitterDate)}   = '{{0}}'", expected.CommitterDate, actual.CommitterDate);
            AppendLine($"\t.{nameof(FileVersion.Message)}         = '{{0}}'", expected.Message, actual.Message);

            if (notEqual)
            {
                throw new EqualException(expectedText, actualText);
            }
        }
    }
}