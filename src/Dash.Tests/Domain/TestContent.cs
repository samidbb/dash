using Dash.Domain;
using Xunit;

namespace Dash.Tests.Domain
{
    public class TestContent
    {
        [Theory]
        [InlineData("{}", "{}")]
        [InlineData("{\"id\": 1}", "{}")]
        [InlineData("{\"id\": 1, \"uid\": \"some-uid\"}", "{\"uid\":\"some-uid\"}")]
        public void Can_strip_id_from_dashboard_json_content(string input, string expected)
        {
            Content content = input;

            Assert.Equal(expected, content);
        }
    }
}