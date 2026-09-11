using System;
using System.Globalization;
using System.IO;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Knob Demo: a plain Wisej.Web.Widget (gaugeKnob) hosting the jQuery-style VendorKnob plugin.
    /// Packages (jQuery → vendor CSS → vendor JS) are declared in the designer file; the
    /// context-safe InitScript is wwwroot/gauge-init.js, embedded in the assembly.
    /// </summary>
    public partial class Window1 : Form
    {
        // The server copy of the pressure value: the knob is an input control, so the server
        // records what the user turned it to (valueChanged).
        private double _value = 40;

        public Window1()
        {
            InitializeComponent();

            // The InitScript text and the Options object are set here: the Designer would keep the
            // same script in a .resx; reading the embedded resource keeps the .js file editable.
            this.gaugeKnob.InitScript = LoadScript("IntegrationLab.wwwroot.gauge-init.js");
            SetKnobOptions(this.gaugeKnob, "gaugeKnob", _value);
        }

        private void gaugeKnob_WidgetEvent(object sender, WidgetEventArgs e)
        {
            dynamic data = e.Data;

            switch (e.Type)
            {
                case "valueChanged":
                    {
                        double reported = ToDouble(data?.value);
                        if (double.IsNaN(reported) || reported < 0 || reported > 100)
                            break;

                        _value = reported;
                        dynamic options = this.gaugeKnob.Options;
                        options.value = reported;     // server state follows the input control; update() re-applies it silently
                        HideAlarm();
                        SetStatus($"{F(reported)} psi", error: false);
                        break;
                    }

                case "error":
                    {
                        // The adapter caught a vendor failure during init or update.
                        string phase = ToStr(data?.phase);
                        string message = ToStr(data?.message);
                        ShowAlarm($"✖ Pressure knob: {phase} failed — {message}");
                        SetStatus("fault", error: true);
                        break;
                    }
            }
        }

        /// <summary>Reads an InitScript embedded in this assembly (see the csproj EmbeddedResource entry).</summary>
        private static string LoadScript(string resourceName)
        {
            using (var stream = typeof(Window1).Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException("Embedded InitScript not found: " + resourceName);
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// The Options object the knob receives in init(options). First-level fields only, so a
        /// later change to any of them makes Wisej.NET call update(options, old) on the client.
        /// "name" is debugging metadata: the InitScript registers the widget under it for DevTools.
        /// </summary>
        private static void SetKnobOptions(Widget widget, string name, double value)
        {
            dynamic options = widget.Options;
            options.name = name;
            options.value = value;
            options.min = 0D;
            options.max = 100D;
            options.step = 1D;
            options.label = "Pressure";
            options.units = " psi";
            options.color = "#1a86ff";
        }

        private void SetStatus(string text, bool error)
        {
            this.labelStatus.Text = "● " + text;
            this.labelStatus.ForeColor = error
                ? System.Drawing.Color.FromArgb(224, 86, 59)
                : System.Drawing.Color.FromArgb(31, 157, 87);
        }

        private void ShowAlarm(string text)
        {
            this.labelAlarm.Text = text;
            this.labelAlarm.Visible = true;
        }

        private void HideAlarm()
        {
            this.labelAlarm.Visible = false;
        }

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        private static string ToStr(object value) => value == null ? "" : Convert.ToString(value, CultureInfo.InvariantCulture);

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }
    }
}
