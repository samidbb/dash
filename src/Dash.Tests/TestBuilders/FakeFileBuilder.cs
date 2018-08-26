using Dash.Tests.TestDoubles;

namespace Dash.Tests.TestBuilders
{
    internal class FakeFileBuilder
    {
        private string _content = string.Empty;

        public FakeFileBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public FakeFile Build()
        {
            return new FakeFile(string.Empty, _content);
        }

        public static implicit operator FakeFile(FakeFileBuilder builder)
        {
            return builder.Build();
        }
    }
}