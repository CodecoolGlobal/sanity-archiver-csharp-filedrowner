using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SanityArchiver.Application.Models
{
    /// <summary>
    /// Dummy equivalent of FileSystemObjectInfo
    /// </summary>
    internal class DummyFileSystemObjectInfo : FileSystemObjectInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyFileSystemObjectInfo"/> class.
        /// Basic Constructor
        /// </summary>
        public DummyFileSystemObjectInfo()
        : base(new DirectoryInfo("DummyFileSystemObjectInfo"))
        {
        }
    }
}
