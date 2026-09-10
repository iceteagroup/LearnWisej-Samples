namespace OperationsConsole.Models
{
    /// <summary>
    /// One entry of a <c>ComboBox</c> list: the <see cref="Key"/> is what the application stores,
    /// the <see cref="Text"/> is what the user reads. Module 2 keeps the two apart on purpose —
    /// "Active" on screen must never become the value that is persisted
    /// (<c>cboStatus.DisplayMember = "Text"; cboStatus.ValueMember = "Key";</c>).
    /// </summary>
    public sealed class OptionItem
    {
        public OptionItem(string key, string text)
        {
            Key = key;
            Text = text;
        }

        /// <summary>The stored value ("ACT"). Never shown to the user.</summary>
        public string Key { get; set; }

        /// <summary>The display text ("Active"). Never persisted.</summary>
        public string Text { get; set; }

        /// <summary>Only used when a list is bound without a <c>DisplayMember</c> — the editors always set one.</summary>
        public override string ToString() => Text;
    }
}
