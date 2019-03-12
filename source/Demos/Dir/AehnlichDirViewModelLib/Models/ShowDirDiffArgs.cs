﻿namespace AehnlichDirViewModelLib.Models
{
    using AehnlichLib.Dir;

    public class ShowDirDiffArgs
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="leftDir"></param>
        /// <param name="rightDir"></param>
        public ShowDirDiffArgs(string leftDir, string rightDir)
            : this()
        {
            this.LeftDir = leftDir;
            this.RightDir = rightDir;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected ShowDirDiffArgs()
        {
            ShowOnlyInA = true;
            ShowOnlyInB = true;
            ShowDifferent = true;
            ShowSame = true;
            Recursive = true;
            IgnoreDirectoryComparison = false;
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the path of the left directory A being compared to the right directory B.
        /// </summary>
        public string LeftDir { get; }

        /// <summary>
        /// Gets the path of the right directory B being compared to the left directory A.
        /// </summary>
        public string RightDir { get; }

        /// <summary>
        /// Determines whether comparison results should only be relevant for the left view A.
        /// 
        /// Setting this to false will ignore differences that are relevant for B only. That is,
        /// directories that are missing in view B will not be flagged as difference.
        /// </summary>
        public bool ShowOnlyInA { get; set; }

        /// <summary>
        /// Determines whether comparison results should only be relevant for the right view B.
        /// 
        /// Setting this to false will ignore differences that are relevant for A only. That is,
        /// directories that are missing in view A will not be flagged as difference.
        /// </summary>
        public bool ShowOnlyInB { get; set; }

        /// <summary>
        /// Determines whether the result set also contains similar items, such as folders with
        /// similar or the same content, or whether the result contains only items that are present
        /// in one view but not the other view.
        /// 
        /// Setting this to false to generates only insert/delete item differences in the result.
        /// Set it to true to generate insert/delete and update (similar) item differences in
        /// the result set.
        /// </summary>
        public bool ShowDifferent { get; set; }

        /// <summary>
        /// Determines whether the result set also contains equal items or not.
        /// 
        /// Set this to false to generate a result set that contains only
        /// insert/delete/update item differences but no equal items
        /// (equal items are not part of the result set even if they exists).
        /// Setting this to true also includes equal items in the generated result set.
        /// </summary>
        public bool ShowSame { get; set; }

        /// <summary>
        /// <param <see cref="Recursive">Determines whether or not only files and folders
        /// in a given directory are compared without comparing contents of sub-directories.
        /// 
        /// Set this option to true to also compare contents in sub-directories and not just
        /// content on the given root level.
        /// </param>
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Determines whether sub-directories with different content
        /// (both sub-directories exist in both directories A and B) are flagged as different if they contain
        /// different files/directories or not.
        /// 
        /// Setting this parameter to true ignores directories completely (they are never flagged as different).
        /// Setting it to false leads to generating entries with a hint towards difference even for directories.</param>
        /// </summary>
        public bool IgnoreDirectoryComparison { get; set; }

        /// <summary>
        /// This can be used to setup a file filter that determines the type of file(s)
        /// that can be included or excluded in the comaparison of directories.
        /// 
        /// Setting this for instance to DirectoryDiffFileFilter("*.cs", true) leads
        /// to comparing only C-Sharp files (all other files are ignored).
        /// 
        /// And directories are equal if they contain the same sub-directory structure
        /// and either no C-Sharp files or equal C-Sharp files.
        /// </summary>
        public DirectoryDiffFileFilter FileFilter { get; set; }
        #endregion properties
    }
}
