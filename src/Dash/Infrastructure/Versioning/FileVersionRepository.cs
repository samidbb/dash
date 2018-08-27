using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure.Versioning
{
    public class FileVersionRepository
    {
        private readonly Lazy<List<FileVersion>> _fileVersionList;

        public FileVersionRepository(FileVersionParser fileVersionParser)
        {
            _fileVersionList = new Lazy<List<FileVersion>>(() => fileVersionParser.ParseFileVersions().ToList());
        }

        public IEnumerable<FileVersion> GetFileVersionList()
        {
            return _fileVersionList.Value;
        }

        public void Save(FileVersion fileVersion)
        {
            var index = _fileVersionList.Value.FindIndex(x => x.Entry == fileVersion.Entry);
            if (index == -1)
            {
                _fileVersionList.Value.Add(fileVersion);
            }
            else
            {
                _fileVersionList.Value[index] = fileVersion;
            }
        }

        public void Remove(string entry)
        {
            var index = _fileVersionList.Value.FindIndex(x => x.Entry == entry);
            if (index != -1)
            {
                _fileVersionList.Value.RemoveAt(index);
            }
        }
    }
}