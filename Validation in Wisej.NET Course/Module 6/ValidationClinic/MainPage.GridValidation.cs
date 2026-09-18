using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wisej.Web;
using ValidationClinic.Models;
using ValidationClinic.Services;

namespace ValidationClinic
{
    public partial class MainPage
    {
        private static string CellMessage(string column, string text) => column switch
        {
            "NameColumn" when string.IsNullOrWhiteSpace(text) => "Enter a name.",
            "EmailColumn" when string.IsNullOrWhiteSpace(text) || !text.Contains('@') => "Enter a valid email address.",
            "AgeColumn" when !int.TryParse(text, out int age) || age < 0 || age > 120 => "Enter a whole-number age from 0 to 120.",
            _ => ""
        };
        private void gridContacts_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (reloading || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var row = gridContacts.Rows[e.RowIndex];
            // An untouched placeholder is not a contact. A typed value must still be checked.
            if (row.IsNewRow && string.IsNullOrWhiteSpace(Convert.ToString(e.FormattedValue))) return;
            var cell = row.Cells[e.ColumnIndex];
            var text = Convert.ToString(e.FormattedValue)?.Trim() ?? "";
            cell.ErrorText = CellMessage(gridContacts.Columns[e.ColumnIndex].Name, text);
            e.Cancel = cell.ErrorText.Length > 0;
            Trace("cell", e.Cancel ? cell.ErrorText : "Typed value accepted.");
            RefreshGridSummary();
        }
        private void gridContacts_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (reloading || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            gridContacts.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
            ValidateRow(e.RowIndex);
            RefreshGridSummary();
        }
        private void ValidateRow(int rowIndex)
        {
            var row = gridContacts.Rows[rowIndex];
            if (row.IsNewRow) return;
            var closed = row.Cells["ClosedDateColumn"];
            const string message = "Closed contacts require a closed date.";
            bool missing = Convert.ToString(row.Cells["StatusColumn"].Value) == "Closed" &&
                (closed.Value == null || closed.Value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(closed.Value)));
            row.ErrorText = missing ? message : "";
            // Never erase an unrelated conversion error while updating this rule.
            if (missing) closed.ErrorText = message;
            else if (closed.ErrorText == message) closed.ErrorText = "";
        }
        private void gridContacts_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            if (reloading) return;
            e.Cancel = true;
            Debug.WriteLine(e.Exception);
            Console.Error.WriteLine(e.Exception);
            if (e.RowIndex >= 0 && e.RowIndex < gridContacts.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < gridContacts.Columns.Count)
                gridContacts.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "The value cannot be converted or saved. Check the format.";
            Trace("data error", "Conversion/commit refused; technical details are in the server log.");
            // Reading row.ErrorText inside DataError can raise DataError again.
            gridSummaryLabel.Text = "A grid value cannot be converted or saved. Correct its format or cancel the edit.";
            gridSummaryLabel.Visible = true;
        }
        private List<string> GridMessages()
        {
            var messages = new List<string>();
            foreach (DataGridViewRow row in gridContacts.Rows)
            {
                if (row.IsNewRow) continue;
                if (!string.IsNullOrEmpty(row.ErrorText)) messages.Add($"Row {row.Index + 1}: {row.ErrorText}");
                foreach (DataGridViewCell cell in row.Cells)
                    if (!string.IsNullOrEmpty(cell.ErrorText)) messages.Add($"Row {row.Index + 1}: {cell.ErrorText}");
            }
            return messages.Distinct().ToList();
        }
        private void RefreshGridSummary()
        {
            var messages = GridMessages();
            gridSummaryLabel.Text = string.Join(Environment.NewLine, messages);
            gridSummaryLabel.Visible = messages.Count > 0;
        }
        private List<string> ValidateGrid()
        {
            // Refuse a failed pending edit before examining the old committed values.
            if (!gridContacts.EndEdit())
            {
                var pending = GridMessages();
                if (pending.Count == 0) pending.Add("Finish or cancel the current grid edit.");
                return pending;
            }
            gridSource.EndEdit();
            foreach (DataGridViewRow row in gridContacts.Rows)
            {
                if (row.IsNewRow) continue;
                foreach (string column in new[] { "NameColumn", "EmailColumn", "AgeColumn" })
                    row.Cells[column].ErrorText = CellMessage(column, Convert.ToString(row.Cells[column].Value)?.Trim() ?? "");
                ValidateRow(row.Index);
                if (row.DataBoundItem is ContactEditModel model)
                {
                    var modelErrors = ModelValidation.ValidateModel(model).Select(x => x.ErrorMessage)
                        .Concat(new ContactValidator().Validate(model).Select(x => x.Message)).Distinct();
                    row.ErrorText = string.Join(" ", modelErrors);
                }
            }
            RefreshGridSummary();
            var result = GridMessages();
            Trace("grid", result.Count == 0 ? "All edited rows accepted." : $"Blocked: {result.Count} row/cell problems.");
            return result;
        }
        private void btnSaveGrid_Click(object sender, EventArgs e)
        {
            if (ValidateGrid().Count != 0) { lblStatus.Text = "Nothing saved. Correct the grid errors."; return; }
            try { repository.SaveAll(workingRows); Reload(); Trace("write", "All grid rows saved together."); }
            catch (DuplicateNameException) { lblStatus.Text = "Nothing saved. Contact names must be unique."; }
            catch (Exception ex) { Debug.WriteLine(ex); Console.Error.WriteLine(ex); lblStatus.Text = "Could not save. Your edits are still here; please try again."; }
        }
    }
}
