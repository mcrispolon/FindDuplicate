using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindDuplicateFile
{
   public class DuplicateFile
    {
        public string FileHash { get; set; }
        public string FileName { get; set; }
        public string FileDirectory { get; set; }
    }


    public class FileGrouping
    {
        public string File { get; set; }

        public List<DuplicateFile> Files { get; set; }
    }


}
