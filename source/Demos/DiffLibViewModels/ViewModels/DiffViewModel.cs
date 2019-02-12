﻿namespace DiffLibViewModels.ViewModels
{
    using DiffLib.Text;
    using DiffLibViewModels.Enums;
    using DiffViewLib.Enums;
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class DiffViewModel : Base.ViewModelBase
    {
        #region fields
        private ChangeDiffOptions _ChangeDiffOptions;
        private DiffViewLines lines;

        private TextDocument _document = null;
        private bool _isDirty = false;

        private Dictionary<int, DiffContext> _DocumentLineDiffs;
        private readonly ObservableRangeCollection<DiffContext> _DocLineDiffs;
        #endregion fields

        #region ctors
        public DiffViewModel()
        {
            _DocLineDiffs = new ObservableRangeCollection<DiffContext>();
        }
        #endregion ctors

        #region properties
        public TextDocument Document
        {
            get { return this._document; }
            set
            {
                if (this._document != value)
                {
                    this._document = value;
                    NotifyPropertyChanged(() => Document);
                }
            }
        }

        public IEnumerable<DiffContext> DocLineDiffs
        {
            get
            {
                return _DocLineDiffs;
            }
        }

        public Dictionary<int, DiffContext> DocumentLineDiffs
        {
            get { return this._DocumentLineDiffs; }
            protected set
            {
                if (this._DocumentLineDiffs != value)
                {
                    this._DocumentLineDiffs = value;
                    NotifyPropertyChanged(() => DocumentLineDiffs);
                }
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    NotifyPropertyChanged(() => IsDirty);
                }
            }
        }

        public ChangeDiffOptions ChangeDiffOptions
        {
            get
            {
                return _ChangeDiffOptions;
            }

            internal set
            {
                if (_ChangeDiffOptions != value)
                {
                    _ChangeDiffOptions = value;
                    NotifyPropertyChanged(() => ChangeDiffOptions);
                }
            }
        }

        [Browsable(false)]
        public int LineCount => this.lines != null ? this.lines.Count : 0;
        #endregion properties

        #region methods

        /// <summary>
        /// Used to setup the ViewA/ViewB view that shows the left and right text views
        /// with the textual content and imaginary lines.
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        public void SetData(IList<string> stringList, EditScript script, bool useA)
        {
            this.lines = new DiffViewLines(stringList, script, useA);

            Dictionary<int, DiffContext> documentLineDiffs;
            string text = GetDocumentFromRawLines(out documentLineDiffs);

            DocumentLineDiffs = documentLineDiffs;
            Document = new TextDocument(text);

            NotifyPropertyChanged(() => DocumentLineDiffs);
            NotifyPropertyChanged(() => Document);

            this.UpdateAfterSetData();
        }

        private string GetDocumentFromRawLines(out Dictionary<int, DiffContext> documentLineDiffs)
        {
            string ret = string.Empty;

            documentLineDiffs = new Dictionary<int, DiffContext>();
            _DocLineDiffs.Clear();
            int line = 0;

            foreach (var item in lines)
            {
                DiffContext lineContext = DiffContext.Blank;
                switch (item.EditType)
                {
                    case DiffLib.Enums.EditType.Delete:
                        lineContext = DiffContext.Deleted;
                        break;
                    case DiffLib.Enums.EditType.Insert:
                        lineContext = DiffContext.Added;
                        break;
                    case DiffLib.Enums.EditType.Change:
                        lineContext = DiffContext.Context;
                        break;

                    case DiffLib.Enums.EditType.None:
                    default:
                        break;
                }

                _DocLineDiffs.Add(lineContext);
                documentLineDiffs.Add(line++, lineContext);
                ret += item.Text + '\n';
            }

            return ret;
        }

        /// <summary>
        /// Used to setup the ViewLineDiff view that shows only 2 lines over each other
        /// representing the currently active line from the left/right side views under
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
		internal void SetData(DiffViewLine lineOne, DiffViewLine lineTwo)
        {
            this.lines = new DiffViewLines(lineOne, lineTwo);
            this.UpdateAfterSetData();
        }

        /// <summary>
        /// Sets the Counterpart property in each line property of each
        /// <see cref="DiffViewModel"/> to refer to each other. This information
        /// can be used for finding equivelant from left to right lines[] collection
        /// and vice versa.
        /// </summary>
        /// <param name="counterpartView"></param>
        public void SetCounterpartLines(DiffViewModel counterpartView)
        {
            int numLines = this.LineCount;
            if (numLines != counterpartView.LineCount)
            {
                throw new ArgumentException("The counterpart view has a different number of view lines.", nameof(counterpartView));
            }

            for (int i = 0; i < numLines; i++)
            {
                DiffViewLine line = this.lines[i];
                DiffViewLine counterpart = counterpartView.lines[i];

                // Make the counterpart lines refer to each other.
                line.Counterpart = counterpart;
                counterpart.Counterpart = line;
            }
        }

        private void UpdateAfterSetData()
        {
            // Reset the position before we start calculating things
////            this.position = new DiffViewPosition(0, 0);
////            this.selectionStart = DiffViewPosition.Empty;
////
////            // We have to call this to recalc the gutter width
////            this.UpdateTextMetrics(false);
////
////            // We have to call this to setup the scroll bars
////            this.SetupScrollBars();
////
////            // Reset the scroll position
////            this.VScrollPos = 0;
////            this.HScrollPos = 0;
////
////            // Update the caret
////            this.UpdateCaret();
////
////            // Force a repaint
////            this.Invalidate();
////
////            // Fire the LinesChanged event
////            if (this.LinesChanged != null)
////            {
////                this.LinesChanged(this, EventArgs.Empty);
////            }
////
////            // Fire the position changed event
////            if (this.PositionChanged != null)
////            {
////                this.PositionChanged(this, EventArgs.Empty);
////            }
////
////            this.FireSelectionChanged();
        }
        #endregion methods
    }
}