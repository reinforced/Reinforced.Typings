using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Tests.Core
{
    class MockFileOperations : IFilesOperations
    {
        public bool DeployCalled { get; private set; }

        public void DeployTempFiles()
        {
            DeployCalled = true;
        }

        public Stream GetTmpFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public string GetPathForType(Type t)
        {
            throw new NotImplementedException();
        }

        public string GetRelativePathForType(Type typeToReference, Type currentlyExportingType)
        {
            throw new NotImplementedException();
        }

        public void ClearTempRegistry()
        {
            throw new NotImplementedException();
        }
    }
}
