using System;
using System.Linq;
using System.Text;
using Dash.Infrastructure;
using Dash.Tests.TestDoubles;
using Xunit;
using Xunit.Sdk;

namespace Dash.Tests
{
    public class TestDashboardVersionRepository
    {
        [Fact]
        public void Has_correct_filename()
        {
            Assert.Equal("dashboard_version.csv", DashboardVersionRepository.DashboardVersionCsvFileName);
        }

        [Fact]
        public void Has_correct_list_of_headers()
        {
            Assert.Equal("entry;hash;author_name;author_email;author_date;committer_name;committer_email;committer_date;message", DashboardVersionRepository.Headers);
        }

        [Theory]
        [InlineData("")]
        [InlineData(DashboardVersionRepository.Headers)]
        public void Can_parse_empty_version_entries(string input)
        {
            var stub = new StubFileSystem(input);
            var sut = new DashboardVersionRepository(stub);

            var dashboardMetaEntries = sut.GetDashboardVersionList();

            Assert.Empty(dashboardMetaEntries);
        }

        [Fact]
        public void Can_parse_version_entries()
        {
            var stub = new StubFileSystem(
                $"{DashboardVersionRepository.Headers}\n" +
                "aws-account-billing.json;24083c9fae5cdcd5f707584ce126b98cd1472281;rifisdfds;40063756+rifisdfds@users.noreply.github.com;2018-08-08 10:10:36 +0100;GitHub;noreply@github.com;2018-08-08 10:10:36 +0100;Sorted graph by cost. Changed period to 90 days"
            );
            var sut = new DashboardVersionRepository(stub);

            var dashboardMetaEntries = sut.GetDashboardVersionList();

            Assert.Single(dashboardMetaEntries);
        }

        [Fact]
        public void Can_parse_dashboard_version_entry()
        {
            var stub = new StubFileSystem(
                $"{DashboardVersionRepository.Headers}\n" +
                "aws-account-billing.json;24083c9fae5cdcd5f707584ce126b98cd1472281;rifisdfds;40063756+rifisdfds@users.noreply.github.com;2018-08-08 10:10:36 +0100;GitHub;noreply@github.com;2018-08-08 10:10:36 +0100;Sorted graph by cost. Changed period to 90 days"
            );
            var sut = new DashboardVersionRepository(stub);

            var result = sut.GetDashboardVersionList().FirstOrDefault();

            DashboardVersionAssert.Equal(
                expected: new DashboardVersionBuilder()
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
    }

    internal static class DashboardVersionAssert
    {
        public static void Equal(DashboardVersion expected, DashboardVersion actual)
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
                sb.AppendLine("DashboardVersion with");
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

            AppendLine($"\t.{nameof(DashboardVersion.Entry)}           = '{{0}}'", expected.Entry, actual.Entry);
            AppendLine($"\t.{nameof(DashboardVersion.Hash)}            = '{{0}}'", expected.Hash, actual.Hash);
            AppendLine($"\t.{nameof(DashboardVersion.AuthorName)}      = '{{0}}'", expected.AuthorName, actual.AuthorName);
            AppendLine($"\t.{nameof(DashboardVersion.AuthorEmail)}     = '{{0}}'", expected.AuthorEmail, actual.AuthorEmail);
            AppendLine($"\t.{nameof(DashboardVersion.AuthorDate)}      = '{{0}}'", expected.AuthorDate, actual.AuthorDate);
            AppendLine($"\t.{nameof(DashboardVersion.CommitterName)}   = '{{0}}'", expected.CommitterName, actual.CommitterName);
            AppendLine($"\t.{nameof(DashboardVersion.CommitterEmail)}  = '{{0}}'", expected.CommitterEmail, actual.CommitterEmail);
            AppendLine($"\t.{nameof(DashboardVersion.CommitterDate)}   = '{{0}}'", expected.CommitterDate, actual.CommitterDate);
            AppendLine($"\t.{nameof(DashboardVersion.Message)}         = '{{0}}'", expected.Message, actual.Message);

            if (notEqual)
            {
                throw new EqualException(expectedText, actualText);
            }
        }
    }
}