using System;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// The second screen the modernization checklist is applied to: one work order in a modal dialog.
    /// Header = title from resources + the same StatusChip as the dashboard; the values are the SAME raw
    /// DateTime/decimal/double the grid shows, formatted here for the session's culture (6/14/2026 · $1,850.00
    /// vs 14.06.2026 · 1.850,00 €). It re-skins with the theme like every other window — there is no colour,
    /// pattern or literal caption in this file either.
    /// </summary>
    public partial class WorkOrderDetail : Form
    {
        private readonly IWorkOrderService _workOrders;
        private readonly ILocalizationService _localization;
        private readonly ILog _log;
        private WorkOrder _order;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrderDetail() : this(null, null, null, new ActivityLog())
        {
        }

        public WorkOrderDetail(WorkOrder order, IWorkOrderService workOrders, ILocalizationService localization, ILog log)
        {
            InitializeComponent();

            _order = order;
            _workOrders = workOrders;
            _localization = localization;
            _log = log;
        }

        private void WorkOrderDetail_Load(object sender, EventArgs e)
        {
            try
            {
                if (_order == null || _localization == null)
                    return;

                ApplyLocalizedText();
                Bind();
                _log.Info(LogLayer.UI, "WorkOrderDetail.Load",
                    $"#{_order.Id} shown · formatted for {_localization.Culture.Name}: {_localization.FormatDate(_order.DueOn)} · {_localization.FormatCurrency(_order.LaborCost)} · theme \"{Application.Theme?.Name}\" (no code of this dialog knows either)");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderDetail.Load", ex);
            }
        }

        private string T(string key) => _localization.Text(key);

        private void ApplyLocalizedText()
        {
            this.labelCreatedCaption.Text = T("Detail.Created");
            this.labelDueCaption.Text = T("Detail.Due");
            this.labelCostCaption.Text = T("Detail.Cost");
            this.labelHoursCaption.Text = T("Detail.Hours");
            this.labelHint.Text = _localization.Format("Detail.Hint", _localization.Culture.NativeName);
            this.buttonNextStatus.Text = T("Button.NextStatus");
            this.buttonClose.Text = T("Button.Close");
        }

        /// <summary>data → UI: raw values in, culture-formatted text out.</summary>
        private void Bind()
        {
            this.Text = _localization.Format("Detail.Title", _order.Id);
            this.labelTitle.Text = _order.Title;
            this.chipStatus.Show(_order.Status, _localization.StatusText(_order.Status));
            this.labelCreatedValue.Text = _localization.FormatDateTime(_order.CreatedOn);
            this.labelDueValue.Text = _localization.FormatDate(_order.DueOn);
            this.labelCostValue.Text = _localization.FormatCurrency(_order.LaborCost);
            this.labelHoursValue.Text = _localization.FormatNumber(_order.Hours);
        }

        private async void buttonNextStatus_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "WorkOrderDetail.buttonNextStatus_Click", $"→ IWorkOrderService.AdvanceStatusAsync(#{_order.Id})");
                var result = await _workOrders.AdvanceStatusAsync(_order.Id);
                if (result.Succeeded)
                {
                    _order = result.Value;
                    Bind();
                    _log.Info(LogLayer.UI, "WorkOrderDetail.ShowResult", $"OK · {result.Message}");
                }
                else
                {
                    _log.Warn(LogLayer.UI, "WorkOrderDetail.ShowResult", $"FAIL · {result.Message}");
                    AlertBox.Show(result.Message, MessageBoxIcon.Warning,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                }
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderDetail.buttonNextStatus_Click", ex);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }
    }
}
