using System.Collections.Generic;
using System.Linq;
using Dash.Infrastructure;
using Dash.Infrastructure.Configuration;
using Dash.Infrastructure.Versioning;

namespace Dash.Domain
{
    public class DashboardService
    {
        private readonly FileVersionRepository _fileVersionRepository;
        private readonly DashboardConfigurationRepository _dashboardConfigurationRepository;
        private readonly IFileSystem _fileSystem;

        public DashboardService(FileVersionRepository fileVersionRepository, DashboardConfigurationRepository dashboardConfigurationRepository, IFileSystem fileSystem)
        {
            _fileVersionRepository = fileVersionRepository;
            _dashboardConfigurationRepository = dashboardConfigurationRepository;
            _fileSystem = fileSystem;
        }

        public IEnumerable<Dashboard> GetDashboards()
        {
            var versionList = _fileVersionRepository.GetFileVersionList().ToList();
            var settingsList = _dashboardConfigurationRepository.GetAll().ToList();

            var map = settingsList.ToDictionary(x => x.Name);

            foreach (var version in versionList)
            {
                if (!map.TryGetValue(version.Entry, out var settings))
                {
                    continue;
                }

                Signature author = new SignatureBuilder().WithName(version.AuthorName).WithEmail(version.AuthorEmail).WithDate(version.AuthorDate);
                Signature committer = new SignatureBuilder().WithName(version.CommitterName).WithEmail(version.CommitterEmail).WithDate(version.CommitterDate);
                DashboardMeta meta = new DashboardMetaBuilder().WithCommit(version.Hash).WithCommitMessage(version.Message).WithCommitter(committer).WithAuthor(author);
                var content = _fileSystem.ReadAllText(version.Entry);

                yield return new DashboardBuilder()
                    .WithId(settings.Id)
                    .WithName(version.Entry)
                    .WithTeam(settings.Team)
                    .WithMeta(meta)
                    .WithContent(content)
                    .Build();
            }
        }
    }
}