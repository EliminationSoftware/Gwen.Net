﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     ListBox control.
    /// </summary>
    [XmlControl(CustomHandler = "XmlElementHandler")]
    public class ListBox : ScrollControl
    {
        private readonly List<ListBoxRow> m_SelectedRows;
        private readonly Table m_Table;

        private bool m_MultiSelect;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ListBox(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.One;

            m_SelectedRows = new List<ListBoxRow>();

            MouseInputEnabled = true;
            EnableScroll(horizontal: false, vertical: true);
            AutoHideBars = true;

            m_Table = new Table(this);
            m_Table.AutoSizeToContent = true;
            m_Table.ColumnCount = 1;

            m_MultiSelect = false;
            IsToggle = false;
        }

        /// <summary>
        ///     Determines whether multiple rows can be selected at once.
        /// </summary>
        [XmlProperty] public bool AllowMultiSelect
        {
            get => m_MultiSelect;
            set
            {
                m_MultiSelect = value;

                if (value)
                {
                    IsToggle = true;
                }
            }
        }

        [XmlProperty] public bool AlternateColor
        {
            get => m_Table.AlternateColor;
            set => m_Table.AlternateColor = value;
        }

        /// <summary>
        ///     Determines whether rows can be unselected by clicking on them again.
        /// </summary>
        [XmlProperty] public bool IsToggle { get; set; }

        /// <summary>
        ///     Number of rows in the list box.
        /// </summary>
        public int RowCount => m_Table.RowCount;

        /// <summary>
        ///     Returns specific row of the ListBox.
        /// </summary>
        /// <param name="index">Row index.</param>
        /// <returns>Row at the specified index.</returns>
        public ListBoxRow this[int index] => m_Table[index] as ListBoxRow;

        /// <summary>
        ///     List of selected rows.
        /// </summary>
        public IEnumerable<TableRow> SelectedRows => m_SelectedRows;

        /// <summary>
        ///     First selected row (and only if list is not multiselectable).
        /// </summary>
        public ListBoxRow SelectedRow
        {
            get
            {
                if (m_SelectedRows.Count == 0)
                {
                    return null;
                }

                return m_SelectedRows[index: 0];
            }
            set
            {
                if (m_Table.Children.Contains(value))
                {
                    if (AllowMultiSelect)
                    {
                        SelectRow(value);
                    }
                    else
                    {
                        SelectRow(value, clearOthers: true);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the selected row number.
        /// </summary>
        [XmlProperty] public int SelectedRowIndex
        {
            get
            {
                ListBoxRow selected = SelectedRow;

                if (selected == null)
                {
                    return -1;
                }

                return m_Table.GetRowIndex(selected);
            }
            set => SelectRow(value);
        }

        /// <summary>
        ///     Column count of table rows.
        /// </summary>
        [XmlProperty] public int ColumnCount
        {
            get => m_Table.ColumnCount;
            set
            {
                m_Table.ColumnCount = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Invoked when a row has been selected.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<ItemSelectedEventArgs> RowSelected;

        /// <summary>
        ///     Invoked whan a row has beed unselected.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<ItemSelectedEventArgs> RowUnselected;

        /// <summary>
        ///     Invoked whan a row has beed double clicked.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<ItemSelectedEventArgs> RowDoubleClicked;

        /// <summary>
        ///     Selects the specified row by index.
        /// </summary>
        /// <param name="index">Row to select.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRow(int index, bool clearOthers = false)
        {
            if (index < 0 || index >= m_Table.RowCount)
            {
                return;
            }

            SelectRow(m_Table.Children[index], clearOthers);
        }

        /// <summary>
        ///     Selects the specified row(s) by text.
        /// </summary>
        /// <param name="rowText">Text to search for (exact match).</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRows(string rowText, bool clearOthers = false)
        {
            IEnumerable<ListBoxRow> rows = m_Table.Children.OfType<ListBoxRow>().Where(x => x.Text == rowText);

            foreach (ListBoxRow row in rows)
            {
                SelectRow(row, clearOthers);
            }
        }

        /// <summary>
        ///     Selects the specified row(s) by regex text search.
        /// </summary>
        /// <param name="pattern">Regex pattern to search for.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRowsByRegex(string pattern, RegexOptions regexOptions = RegexOptions.None,
            bool clearOthers = false)
        {
            IEnumerable<ListBoxRow> rows = m_Table.Children.OfType<ListBoxRow>()
                .Where(x => Regex.IsMatch(x.Text, pattern));

            foreach (ListBoxRow row in rows)
            {
                SelectRow(row, clearOthers);
            }
        }

        /// <summary>
        ///     Slelects the specified row.
        /// </summary>
        /// <param name="control">Row to select.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRow(ControlBase control, bool clearOthers = false)
        {
            if (!AllowMultiSelect || clearOthers)
            {
                UnselectAll();
            }

            var row = control as ListBoxRow;

            if (row == null)
            {
                return;
            }

            // TODO: make sure this is one of our rows!
            row.IsSelected = true;
            m_SelectedRows.Add(row);

            if (RowSelected != null)
            {
                RowSelected.Invoke(this, new ItemSelectedEventArgs(row));
            }
        }

        /// <summary>
        ///     Removes the all rows from the ListBox
        /// </summary>
        public void RemoveAllRows()
        {
            m_Table.DeleteAllChildren();
        }

        /// <summary>
        ///     Removes the specified row by index.
        /// </summary>
        /// <param name="idx">Row index.</param>
        public void RemoveRow(int idx)
        {
            m_Table.RemoveRow(idx); // this calls Dispose()
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="label">Row text.</param>
        /// <returns>Newly created control.</returns>
        public ListBoxRow AddRow(string label)
        {
            return AddRow(label, string.Empty);
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="label">Row text.</param>
        /// <param name="name">Internal control name.</param>
        /// <returns>Newly created control.</returns>
        public ListBoxRow AddRow(string label, string name)
        {
            return AddRow(label, name, UserData: null);
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="label">Row text.</param>
        /// <param name="name">Internal control name.</param>
        /// <param name="UserData">User data for newly created row</param>
        /// <returns>Newly created control.</returns>
        public ListBoxRow AddRow(string label, string name, object UserData)
        {
            ListBoxRow row = new(this);
            m_Table.AddRow(row);

            row.SetCellText(columnIndex: 0, label);
            row.Name = name;
            row.UserData = UserData;

            row.Selected += OnRowSelected;
            row.DoubleClicked += OnRowDoubleClicked;

            Invalidate();

            return row;
        }

        /// <summary>
        ///     Add row.
        /// </summary>
        /// <param name="row">Row.</param>
        public void AddRow(ListBoxRow row)
        {
            row.Parent = this;

            m_Table.AddRow(row);

            row.Selected += OnRowSelected;

            Invalidate();
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            m_Table.SetColumnWidth(column, width);
            Invalidate();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawListBox(this);
        }

        /// <summary>
        ///     Deselects all rows.
        /// </summary>
        public virtual void UnselectAll()
        {
            foreach (ListBoxRow row in m_SelectedRows)
            {
                row.IsSelected = false;

                if (RowUnselected != null)
                {
                    RowUnselected.Invoke(this, new ItemSelectedEventArgs(row));
                }
            }

            m_SelectedRows.Clear();
        }

        /// <summary>
        ///     Unselects the specified row.
        /// </summary>
        /// <param name="row">Row to unselect.</param>
        public void UnselectRow(ListBoxRow row)
        {
            row.IsSelected = false;
            m_SelectedRows.Remove(row);

            if (RowUnselected != null)
            {
                RowUnselected.Invoke(this, new ItemSelectedEventArgs(row));
            }
        }

        /// <summary>
        ///     Handler for the row selection event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnRowSelected(ControlBase control, ItemSelectedEventArgs args)
        {
            // [omeg] changed default behavior
            var clear = false; // !InputHandler.InputHandler.IsShiftDown;
            var row = args.SelectedItem as ListBoxRow;

            if (row == null)
            {
                return;
            }

            if (row.IsSelected)
            {
                if (IsToggle)
                {
                    UnselectRow(row);
                }
            }
            else
            {
                SelectRow(row, clear);
            }
        }

        /// <summary>
        ///     Handler for the row double click event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnRowDoubleClicked(ControlBase control, ClickedEventArgs args)
        {
            var row = control as ListBoxRow;

            if (row == null)
            {
                return;
            }

            if (RowDoubleClicked != null)
            {
                RowDoubleClicked.Invoke(this, new ItemSelectedEventArgs(row));
            }
        }

        /// <summary>
        ///     Removes all rows.
        /// </summary>
        public virtual void Clear()
        {
            UnselectAll();
            m_Table.RemoveAll();
            Invalidate();
        }

        /// <summary>
        ///     Selects the first menu item with the given text it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="text">The label to look for, this is what is shown to the user.</param>
        public void SelectByText(string text)
        {
            foreach (ListBoxRow item in m_Table.Children)
            {
                if (item.Text == text)
                {
                    SelectedRow = item;

                    return;
                }
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given internal name it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="name">The internal name to look for. To select by what is displayed to the user, use "SelectByText".</param>
        public void SelectByName(string name)
        {
            foreach (ListBoxRow item in m_Table.Children)
            {
                if (item.Name == name)
                {
                    SelectedRow = item;

                    return;
                }
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given user data it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="userdata">
        ///     The UserData to look for. The equivalency check uses "param.Equals(item.UserData)".
        ///     If null is passed in, it will look for null/unset UserData.
        /// </param>
        public void SelectByUserData(object userdata)
        {
            foreach (ListBoxRow item in m_Table.Children)
            {
                if (userdata == null)
                {
                    if (item.UserData == null)
                    {
                        SelectedRow = item;

                        return;
                    }
                }
                else if (userdata.Equals(item.UserData))
                {
                    SelectedRow = item;

                    return;
                }
            }
        }

        internal static ControlBase XmlElementHandler(Parser parser, Type type, ControlBase parent)
        {
            ListBox element = new(parent);
            parser.ParseAttributes(element);

            if (parser.MoveToContent())
            {
                foreach (string elementName in parser.NextElement())
                {
                    if (elementName == "Row")
                    {
                        element.AddRow(parser.ParseElement<ListBoxRow>(element));
                    }
                }
            }

            return element;
        }
    }
}
