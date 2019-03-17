namespace AehnlichLib.Dir
{
    using System;

    public sealed class DirectoryDiffEntry
	{
		#region Private Data Members

		private bool _different;
		private readonly bool _inA;
		private readonly bool _inB;
		private readonly bool _isFile;
		private string _error;
        private DirectoryDiffEntryCollection subentries;

        #endregion

        #region Constructors
        /// <summary>
        /// Class constructor (with additional length parameters that are mostly useful for files).
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="name"></param>
        /// <param name="isFile"></param>
        /// <param name="inA"></param>
        /// <param name="inB"></param>
        /// <param name="lastUpdateA"></param>
        /// <param name="lastUpdateB"></param>
        /// <param name="lengthA"></param>
        /// <param name="lengthB"></param>
        internal DirectoryDiffEntry(string basePath,
                            string name,
                            bool isFile,
                            bool inA, bool inB,
                            DateTime lastUpdateA, DateTime lastUpdateB,
                            long lengthA, long lengthB)
            : this(basePath, name, isFile, inA, inB,lastUpdateA, lastUpdateB)
        {
            this.LengthA = lengthA;
            this.LengthB = lengthB;
        }


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="name"></param>
        /// <param name="isFile"></param>
        /// <param name="inA"></param>
        /// <param name="inB"></param>
        /// <param name="lastUpdateA"></param>
        /// <param name="lastUpdateB"></param>
        internal DirectoryDiffEntry(string basePath,
                                    string name,
                                    bool isFile,
                                    bool inA, bool inB,
                                    DateTime lastUpdateA, DateTime lastUpdateB)
            : this()
        {
            this.BasePath = basePath;
            this.Name = name;
            _isFile = isFile;
            _inA = inA;
            _inB = inB;

            // Mark node as different if this entry either refers to A only or B only
            _different = (inA == true && inB == false || inA == false && inB == true);
            this.LastUpdateA = lastUpdateA;
            this.LastUpdateB = lastUpdateB;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public DirectoryDiffEntry()
        {
            this.Name = string.Empty;
            this.BasePath = string.Empty;
            _isFile = true;
            _inA = true;
            _inB = true;
            _different = false;
        }
        #endregion

        #region Public Properties

        public bool Different
		{
			get { return this._different; }

			internal set { this._different = value; }
		}

		public string Error
		{
			get { return this._error; }

			internal set { this._error = value; }
		}

		public bool InA => this._inA;

		public bool InB => this._inB;

		public bool IsFile => this._isFile;

        public string BasePath { get; }

        public string Name { get; }

        /// <summary>
        /// Gets the last time this item has been changed through a write access.
        /// </summary>
		public DateTime LastUpdateA { get; }

        /// <summary>
        /// Gets the last time this item has been changed through a write access.
        /// </summary>
        public DateTime LastUpdateB { get; }

        /// <summary>
        /// Gets the size, in bytes, of the current file system item.
        /// </summary>
        public long LengthA { get; internal set; }

        /// <summary>
        /// Gets the size, in bytes, of the current file system item.
        /// </summary>
        public long LengthB { get; internal set; }

        public DirectoryDiffEntryCollection Subentries
		{
			get
			{
				if (this.subentries == null && !this._isFile)
				{
					this.subentries = new DirectoryDiffEntryCollection();
				}

				return this.subentries;
			}
		}

        #endregion properties

        #region methods
        public void AddSubEntry(DirectoryDiffEntry entry)
        {
            if (this.subentries == null)
                this.subentries = new DirectoryDiffEntryCollection();

            this.subentries.Add(entry);
        }

        public int CountSubDirectories()
        {
            return (Subentries == null ? 0 : Subentries.Count);
        }

        public bool SetDiffBasedOnChildren(bool ignoreDirectoryComparison)
        {
            // Is this a directory and do we want to ignore directory diffs?
            if (IsFile == false && ignoreDirectoryComparison == true)
            {
                Different = false;
                return Different;
            }

            if (Different == false && ignoreDirectoryComparison == false && Subentries != null)
            {
                for (int i = 0; i < Subentries.Count; i++)
                {
                    if (Subentries[i].Different == true)
                    {
                        Different = true;
                        break;
                    }
                }
            }

            return Different;
        }
        #endregion methods
    }
}
