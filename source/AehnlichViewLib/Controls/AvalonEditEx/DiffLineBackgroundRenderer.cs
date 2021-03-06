﻿namespace AehnlichViewLib.Controls.AvalonEditEx
{
	using AehnlichViewLib.Enums;
	using AehnlichViewModelsLib.Models;
	using ICSharpCode.AvalonEdit.Rendering;
	using Interfaces;
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Media;

	internal class DiffLineBackgroundRenderer : IBackgroundRenderer
	{
		#region fields
		static readonly Pen BorderlessPen;
		private readonly DiffView _DiffView;
		#endregion fields

		#region ctors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static DiffLineBackgroundRenderer()
		{
			var transparentBrush = new SolidColorBrush(Colors.Transparent);
			transparentBrush.Freeze();

			BorderlessPen = new Pen(transparentBrush, 0.0);
			BorderlessPen.Freeze();
		}

		/// <summary>
		/// Parameterized class constructor
		/// </summary>
		/// <param name="diffView"></param>
		public DiffLineBackgroundRenderer(DiffView diffView)
			: this()
		{
			this._DiffView = diffView;
		}

		/// <summary>
		/// Hidden standard constructor
		/// </summary>
		protected DiffLineBackgroundRenderer()
		{
		}
		#endregion ctors

		#region properties
		public KnownLayer Layer { get { return KnownLayer.Background; } }
		#endregion properties

		#region methods
		/// <summary>
		/// Draws the background of line based on its <see cref="DiffContext"/>
		/// and whether it is imaginary or not.
		/// </summary>
		/// <param name="textView"></param>
		/// <param name="drawingContext"></param>
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (_DiffView == null)
				return;

			var srcLineDiffs = _DiffView.ItemsSource as IReadOnlyList<IDiffLineInfo>;

			if (srcLineDiffs == null)
				return;

			var toBeRequested = new List<int>();
			foreach (var v in textView.VisualLines)
			{
				var linenum = v.FirstDocumentLine.LineNumber - 1;

				// Find a diff context for a given line
				if (linenum >= srcLineDiffs.Count)
					continue;

				// Determine change context (delete, insert, change) per line
				//  and color background according to bound brush
				var srcLineDiff = srcLineDiffs[linenum];
				var brush = GetLineBackgroundDiffBrush(srcLineDiff.Context, srcLineDiff.ImaginaryLineNumber.HasValue == false);

				if (brush != default(SolidColorBrush))
				{
					foreach (var rc in BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, v.VisualLength))
					{
						drawingContext.DrawRectangle(brush, BorderlessPen,
							new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
					}
				}

				if (srcLineDiff.LineEditScriptSegments != null)
				{
					foreach (var item in srcLineDiff.LineEditScriptSegments)
					{
						// The main line background has already been drawn, so we just
						// need to draw the deleted or inserted background segments.
						if (srcLineDiff.FromA)
							brush = _DiffView.ColorBackgroundDeleted;
						else
							brush = _DiffView.ColorBackgroundAdded;

						BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
						geoBuilder.AlignToWholePixels = true;

						var segment = new Segment(v.StartOffset + item.Offset, item.Length,
												  v.StartOffset + item.EndOffset);
						geoBuilder.AddSegment(textView, segment);

						Geometry geometry = geoBuilder.CreateGeometry();
						if (geometry != null)
							drawingContext.DrawGeometry(brush, null, geometry);
					}
				}
				else
				{
					// Has this line ever been computed before ?
					if (srcLineDiff.LineEditScriptSegmentsIsDirty == true &&
						_DiffView.LineDiffDataProvider != null)
					{
						toBeRequested.Add(linenum);
					}
				}
			}

			// Request matching of additional lines if these appear to be dirty
			if (toBeRequested.Count > 0)
				_DiffView.LineDiffDataProvider.RequestLineDiff(toBeRequested);
		}

		private Brush GetLineBackgroundDiffBrush(DiffContext context, bool lineIsImaginary)
		{
			var brush = default(SolidColorBrush);

			if (lineIsImaginary == false)
			{
				switch (context)
				{
					case DiffContext.Added:
						brush = _DiffView.ColorBackgroundAdded;
						break;

					case DiffContext.Deleted:
						brush = _DiffView.ColorBackgroundDeleted;
						break;

					case DiffContext.Context:
						brush = _DiffView.ColorBackgroundContext;
						break;

					case DiffContext.Blank:
						brush = _DiffView.ColorBackgroundBlank;
						break;
					default:
						throw new System.ArgumentException(context.ToString());
				}
			}
			else
			{
				// These lines have no line number and where inserted or deleted to
				// re-sync view sequences when compared to each other
				switch (context)
				{
					case DiffContext.Added:
						brush = _DiffView.ColorBackgroundImaginaryLineAdded;
						break;

					case DiffContext.Deleted:
						brush = _DiffView.ColorBackgroundImaginaryLineDeleted;
						break;

					default:
					case DiffContext.Blank:
					case DiffContext.Context:
						throw new System.NotSupportedException(string.Format("Context '{0}' not supported. An imaginary line can only be inserted or deleted.", context));
				}
			}

			return brush;
		}
		#endregion methods
	}
}
