using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyNUnitWeb.Models
{
    public class IndexViewModel
    {
        private readonly string pathToAssemblies;

        public IndexViewModel(string pathToAssemblies)
        {
            this.pathToAssemblies = pathToAssemblies;
        }

        public IEnumerable<string> LoadedAssemblies 
            => Directory.EnumerateFiles(pathToAssemblies).Select(f => Path.GetFileName(f));

    }
}
