using System;
using System.Collections.Generic;
using System.IO;

namespace Dash.Infrastructure
{
    public class FileSystem
    {
        public static FileSystem CreateNull()
        {
            return CreateNull(File.Null);
        }
        
        public static FileSystem CreateNull(File file)
        {
            return new NullFileSystem(new Settings {Root = ""}, file);
        }

        private readonly string _rootPath;

        public FileSystem(Settings settings)
        {
            _rootPath = settings.Root;
        }

        public void Delete(string path)
        {
            path = Path.Combine(_rootPath, path);

            System.IO.File.Delete(path);
        }

        public File GetFile(string path)
        {
            path = Path.Combine(_rootPath, path);
            return InternalGetFile(path);
        }

        protected virtual File InternalGetFile(string path)
        {
            return new File(path);
        }

        private class NullFileSystem : FileSystem
        {
            private readonly File _file;

            public NullFileSystem(Settings settings, File file) : base(settings)
            {
                _file = file;
            }

            protected override File InternalGetFile(string path)
            {
                return _file;
            }
        }
    }

    public class File
    {
        public static readonly File Null = new NullFile();
        
        private readonly string _path;

        public File(string path)
        {
            _path = path;
        }

        public virtual string ReadAllText()
        {
            return System.IO.File.ReadAllText(_path);
        }

        public virtual IEnumerable<string> ReadLines()
        {
            return System.IO.File.ReadLines(_path);
        }

        public virtual void WriteAllText(string content)
        {
            System.IO.File.WriteAllText(_path, content);
        }

        public virtual void WriteLines(IEnumerable<string> content)
        {
            System.IO.File.WriteAllLines(_path, content);
        }
        
        private class NullFile : File
        {
            private const string NoName = "";
            private const string NoText = "";
            private static readonly string[] NoLines = new string[0];

            public NullFile() : base(NoName)
            {
            }

            public override string ReadAllText()
            {
                return NoText;
            }

            public override IEnumerable<string> ReadLines()
            {
                return NoLines;
            }

            public override void WriteAllText(string content)
            {
            }

            public override void WriteLines(IEnumerable<string> content)
            {
            }
        }
    }
}