using Dash.Infrastructure.Versioning;

namespace Dash.Tests.TestHelpers
{
    internal static class FileVersionAssert
    {
        public static void Equal(FileVersion expected, FileVersion actual)
        {
            new FileVersionAssertHelper().Equal(expected, actual);
        }

        private class FileVersionAssertHelper : AssertHelper<FileVersion>
        {
            protected override void AssertObject(FileVersion expected, FileVersion actual)
            {
                AssertProperty($"\t.{nameof(FileVersion.Entry)}           = '{{0}}'", x => x.Entry);
                AssertProperty($"\t.{nameof(FileVersion.Hash)}            = '{{0}}'", x => x.Hash);
                AssertProperty($"\t.{nameof(FileVersion.AuthorName)}      = '{{0}}'", x => x.AuthorName);
                AssertProperty($"\t.{nameof(FileVersion.AuthorEmail)}     = '{{0}}'", x => x.AuthorEmail);
                AssertProperty($"\t.{nameof(FileVersion.AuthorDate)}      = '{{0}}'", x => x.AuthorDate);
                AssertProperty($"\t.{nameof(FileVersion.CommitterName)}   = '{{0}}'", x => x.CommitterName);
                AssertProperty($"\t.{nameof(FileVersion.CommitterEmail)}  = '{{0}}'", x => x.CommitterEmail);
                AssertProperty($"\t.{nameof(FileVersion.CommitterDate)}   = '{{0}}'", x => x.CommitterDate);
                AssertProperty($"\t.{nameof(FileVersion.Message)}         = '{{0}}'", x => x.Message);
            }
        }
    }
}