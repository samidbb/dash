using System.Linq;
using Dash.Infrastructure;
using Xunit;

namespace Dash.Tests
{
    public class TestIdGenerator
    {
        [Theory]
        [InlineData("aws-account-billing.json")]
        [InlineData("aws-billing.json")]
        [InlineData("aws-elasticsearch.json")]
        [InlineData("aws-s3.json")]
        [InlineData("blaster-red.json")]
        [InlineData("grafana.json")]
        [InlineData("kubernetes-capacity-planning.json")]
        [InlineData("kubernetes-cluster-health-overview.json")]
        [InlineData("kubernetes-deployment.json")]
        [InlineData("kubernetes-fluentd-pods.json")]
        [InlineData("kubernetes-general-overview.json")]
        [InlineData("kubernetes-kube2iam.json")]
        [InlineData("kubernetes-monitoring-setup.json")]
        [InlineData("kubernetes-nodes.json")]
        [InlineData("kubernetes-persistent-volumes.json")]
        [InlineData("kubernetes-pods.json")]
        [InlineData("kubernetes-resource-requests.json")]
        [InlineData("kubernetes-statefulset.json")]
        [InlineData("kubernetes-tiller.json")]
        [InlineData("kubernetes-weave-net.json")]
        [InlineData("tbd20180803-traefik-red-metrics.json")]
        [InlineData("traefik-detailed.json")]
        [InlineData("wikijs.json")]
        public void Id_only_has_valid_characters(string name)
        {
            var id = IdGenerator.Generate(name);

            Assert.True(id.All(char.IsLetterOrDigit));
        }
    }
}