﻿using System;
using System.Linq;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Base class for multi-column tables.
    /// </summary>
    public class Table : ControlBase
    {
        private readonly int[] m_ColumnWidth;
        private int m_ColumnCount;
        private int m_MaxWidth; // for autosizing, if nonzero - fills last cell up to this size
        private bool m_RowMeasurementDirty;
        private bool m_SizeToContents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Table" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Table(ControlBase parent) : base(parent)
        {
            m_ColumnCount = 1;

            m_ColumnWidth = new int[TableRow.MaxColumns];

            for (var i = 0; i < TableRow.MaxColumns; i++)
            {
                m_ColumnWidth[i] = 20;
            }

            AutoSizeToContent = false;
            m_SizeToContents = false;
            m_RowMeasurementDirty = false;
        }

        /// <summary>
        ///     Column count (default 1).
        /// </summary>
        public int ColumnCount
        {
            get => m_ColumnCount;
            set
            {
                SetColumnCount(value);
                Invalidate();
            }
        }

        /// <summary>
        ///     Row count.
        /// </summary>
        public int RowCount => Children.Count;

        public bool AutoSizeToContent { get; set; }

        public bool AlternateColor { get; set; }

        /// <summary>
        ///     Returns specific row of the table.
        /// </summary>
        /// <param name="index">Row index.</param>
        /// <returns>Row at the specified index.</returns>
        public TableRow this[int index] => Children[index] as TableRow;

        /// <summary>
        ///     Sets the number of columns.
        /// </summary>
        /// <param name="count">Number of columns.</param>
        public void SetColumnCount(int count)
        {
            if (m_ColumnCount == count)
            {
                return;
            }

            foreach (TableRow row in Children.OfType<TableRow>())
            {
                row.ColumnCount = count;
            }

            m_ColumnCount = count;
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            if (m_ColumnWidth[column] == width)
            {
                return;
            }

            m_ColumnWidth[column] = width;
            Invalidate();
        }

        /// <summary>
        ///     Gets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Column width.</returns>
        public int GetColumnWidth(int column)
        {
            return m_ColumnWidth[column];
        }

        /// <summary>
        ///     Adds a new empty row.
        /// </summary>
        /// <returns>Newly created row.</returns>
        public TableRow AddRow()
        {
            TableRow row = new(this);
            row.ColumnCount = m_ColumnCount;
            m_RowMeasurementDirty = true;

            return row;
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="row">Row to add.</param>
        public void AddRow(TableRow row)
        {
            row.Parent = this;
            row.ColumnCount = m_ColumnCount;
            m_RowMeasurementDirty = true;
        }

        /// <summary>
        ///     Adds a new row with specified text in first column.
        /// </summary>
        /// <param name="text">Text to add.</param>
        /// <returns>New row.</returns>
        public TableRow AddRow(string text)
        {
            TableRow row = AddRow();
            row.SetCellText(columnIndex: 0, text);

            return row;
        }

        /// <summary>
        ///     Removes a row by reference.
        /// </summary>
        /// <param name="row">Row to remove.</param>
        public void RemoveRow(TableRow row)
        {
            RemoveChild(row, dispose: true);
            m_RowMeasurementDirty = true;
        }

        /// <summary>
        ///     Removes a row by index.
        /// </summary>
        /// <param name="idx">Row index.</param>
        public void RemoveRow(int idx)
        {
            ControlBase row = Children[idx];
            RemoveRow(row as TableRow);
        }

        /// <summary>
        ///     Removes all rows.
        /// </summary>
        public void RemoveAll()
        {
            while (RowCount > 0)
            {
                RemoveRow(idx: 0);
            }
        }

        /// <summary>
        ///     Gets the index of a specified row.
        /// </summary>
        /// <param name="row">Row to search for.</param>
        /// <returns>Row index if found, -1 otherwise.</returns>
        public int GetRowIndex(TableRow row)
        {
            return Children.IndexOf(row);
        }

        protected override Size Measure(Size availableSize)
        {
            if (m_RowMeasurementDirty && (AutoSizeToContent || m_SizeToContents))
            {
                m_SizeToContents = false;

                return DoSizeToContents(availableSize);
            }

            var height = 0;
            var width = 0;

            foreach (TableRow row in Children)
            {
                row.DoMeasure(availableSize);

                width = Math.Max(width, row.MeasuredSize.Width);
                height += row.MeasuredSize.Height;
            }

            return new Size(width, height);
        }

        protected override Size Arrange(Size finalSize)
        {
            var y = 0;
            var width = 0;
            var even = false;

            foreach (TableRow row in Children)
            {
                if (AlternateColor)
                {
                    row.EvenRow = even;
                    even = !even;
                }

                row.DoArrange(new Rectangle(x: 0, y, finalSize.Width, row.MeasuredSize.Height));
                width = Math.Max(width, row.MeasuredSize.Width);
                y += row.MeasuredSize.Height;
            }

            return new Size(finalSize.Width, y);
        }

        /// <summary>
        ///     Sizes to fit contents.
        /// </summary>
        public void SizeToContent(int maxWidth = 0)
        {
            m_MaxWidth = maxWidth;
            m_SizeToContents = true;
            Invalidate();
        }

        protected Size DoSizeToContents(Size availableSize)
        {
            var height = 0;
            var width = 0;

            for (var i = 0; i < ColumnCount; i++)
            {
                m_ColumnWidth[i] = 0;
            }

            foreach (TableRow row in Children)
            {
                row.DoMeasure(availableSize);

                for (var i = 0; i < ColumnCount; i++)
                {
                    ControlBase cell = row.GetColumn(i);

                    if (null != cell)
                    {
                        m_ColumnWidth[i] = Math.Max(m_ColumnWidth[i], cell.MeasuredSize.Width);
                    }
                }
            }

            // sum all column widths 
            for (var i = 0; i < ColumnCount; i++)
            {
                width += m_ColumnWidth[i];
            }

            width = 0;

            foreach (TableRow row in Children)
            {
                for (var i = 0; i < ColumnCount; i++)
                {
                    if (i < ColumnCount - 1 || m_MaxWidth == 0)
                    {
                        row.SetColumnWidth(i, m_ColumnWidth[i]);
                    }
                    else
                    {
                        row.SetColumnWidth(i, m_ColumnWidth[i] + Math.Max(val1: 0, m_MaxWidth - width));
                    }
                }

                row.DoMeasure(availableSize);

                width = Math.Max(width, row.MeasuredSize.Width);
                height += row.MeasuredSize.Height;
            }

            m_RowMeasurementDirty = false;

            if (m_MaxWidth == 0 || m_MaxWidth < width)
            {
                return new Size(width, height);
            }

            return new Size(m_MaxWidth, height);
        }
    }
}
