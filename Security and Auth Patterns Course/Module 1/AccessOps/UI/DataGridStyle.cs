using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>The mock's grid look: 28 px light header row, 32 px rows, horizontal lines only.</summary>
    internal static class DataGridStyle
    {
        public static void Apply(DataGridView grid, int rowHeight = 32)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.Horizontal;
            grid.ShowColumnVisibilityMenu = false;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            grid.RowHeadersVisible = false;
            grid.ReadOnly = true;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoGenerateColumns = false;
            grid.ColumnHeadersHeight = 28;
            grid.RowTemplate.Height = rowHeight;
            grid.Font = Theme.Px(12.5f);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Theme.Strip;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Theme.Muted;
            grid.ColumnHeadersDefaultCellStyle.Font = Theme.Px(11.5f, FontStyle.Bold);
            grid.DefaultCellStyle.ForeColor = Theme.Text;
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        }

        public static DataGridViewTextBoxColumn Column(string name, string header, string property, int width, bool fill = false)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                DataPropertyName = property,
                Width = width,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            if (fill)
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            return column;
        }

        /// <summary>A monospace column (record numbers, tenants, IDs).</summary>
        public static DataGridViewTextBoxColumn MonoColumn(string name, string header, string property, int width, Color color, bool bold = false)
        {
            var column = Column(name, header, property, width);
            column.DefaultCellStyle.Font = Theme.Mono(12.5f, bold ? FontStyle.Bold : FontStyle.Regular);
            column.DefaultCellStyle.ForeColor = color;
            return column;
        }
    }
}
