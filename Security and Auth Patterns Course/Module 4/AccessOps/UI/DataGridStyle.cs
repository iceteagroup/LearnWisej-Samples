using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>The mock's grid look: light header row, 31 px rows, faint alternate rows.</summary>
    internal static class DataGridStyle
    {
        public static void Apply(DataGridView grid, int rowHeight = 31)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.ReadOnly = true;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoGenerateColumns = false;
            grid.ColumnHeadersHeight = 30;
            grid.RowTemplate.Height = rowHeight;
            grid.Font = Theme.Px(12.5f);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Theme.HeaderBack;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Theme.Muted;
            grid.ColumnHeadersDefaultCellStyle.Font = Theme.Px(11.5f, FontStyle.Bold);
            grid.DefaultCellStyle.ForeColor = Theme.Text;
            grid.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
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
    }
}
