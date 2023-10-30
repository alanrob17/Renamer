// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgList.cs" company="Software Inc.">
//   A.Robson
// </copyright>
// <summary>
//   The argument list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Replace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The argument list.
    /// </summary>
    public class ArgList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgList"/> class.
        /// </summary>
        /// <param name="subFolder">The sub folder.</param>
        /// <param name="changeFileName">The change file name.</param>
        /// <param name="changeFolderName">The change folder name.</param>
        /// <param name="phrase">The text to be removed from each file.</param>
        public ArgList(bool subFolder, bool changeFileName, bool changeFolderName, string phrase)
        {
            this.SubFolder = subFolder;
            this.ChangeFileName = changeFileName;
            this.Phrase = phrase;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to search sub folders.
        /// </summary>
        public bool SubFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to change file names.
        /// </summary>
        public bool ChangeFileName { get; set; }

        /// <summary>
        /// Gets or sets the phrase to be removed.
        /// </summary>
        public string Phrase { get; set; }
    }
}
