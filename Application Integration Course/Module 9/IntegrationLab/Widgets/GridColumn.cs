using System.ComponentModel;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// One typed column definition for <see cref="WorkOrderGrid"/>. This is the "promote the
    /// options that survive into typed properties" step from the lesson: the Designer can
    /// show it, a reviewer can check it, and the vendor never sees anything else.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class GridColumn
    {
        public GridColumn() { }

        public GridColumn(string field, string title, int width, bool editable = true, string type = "text", string[] values = null)
        {
            this.Field = field;
            this.Title = title;
            this.Width = width;
            this.Editable = editable;
            this.Type = type;
            this.Values = values;
        }

        /// <summary>Wire (camelCase) field name in the row objects: id, asset, status...</summary>
        public string Field { get; set; }

        public string Title { get; set; }

        public int Width { get; set; } = 100;

        /// <summary>False for keys and computed fields. The store enforces it again server-side.</summary>
        public bool Editable { get; set; } = true;

        /// <summary>"text", "number" or "select".</summary>
        public string Type { get; set; } = "text";

        /// <summary>Allowed values for "select" columns.</summary>
        public string[] Values { get; set; }

        /// <summary>The plain object handed to the vendor (camelCased by Wisej when serialized).</summary>
        internal object ToOption() => new
        {
            field = this.Field,
            title = this.Title ?? this.Field,
            width = this.Width,
            editable = this.Editable,
            type = this.Type ?? "text",
            values = this.Values
        };

        public override string ToString() => $"{this.Field} ({this.Type}{(this.Editable ? "" : ", read-only")})";
    }
}
