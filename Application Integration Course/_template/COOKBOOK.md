# Wisej.NET integration cookbook (verified on Wisej-4 4.1.0, .NET 10)

Everything below was verified by building and running Module 1 in the browser.
Follow it exactly. Where something is marked **(unverified)** it comes from the Wisej XML
docs and has not been executed yet: implement it, make sure `dotnet build` passes, and say so
in your final report so it can be checked at runtime.

## Project layout (copy `_template`)

```
Module N/
  IntegrationLab.slnx               (from _template; add the project with `dotnet sln IntegrationLab.slnx add IntegrationLab/IntegrationLab.csproj`)
  .gitignore                        (from _template)
  README.md                         (what it shows, how to run, what to click, self-check answers)
  IntegrationLab/
    IntegrationLab.csproj           (from _template; add <EmbeddedResource> lines for every InitScript / Platform file)
    Program.cs  Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json  (set the port: Module N uses http://localhost:507N, e.g. Module 4 → 5074, Module 10 → 5080)
    Window1.cs + Window1.Designer.cs (or a Page)   — the demo screen
    Widgets/ or Controls/           — server classes
    wwwroot/                        — vendor libraries + adapter InitScripts (served as static files at /wwwroot/...)
    docs/                           — the lab deliverables as Markdown (+ SVG diagrams when the lab asks for one)
```

- Namespace is always `IntegrationLab` (course convention). Course class names: `IntegrationLab.Controls.SimpleGauge`, `IntegrationLab.Controls.SimpleGaugeControl`, etc.
- Run: `dotnet run -f net10.0 --urls http://localhost:507N` (the csproj multi-targets net10.0-windows;net10.0) from the project folder. The static file server serves the project folder, so a package `Source = "wwwroot/vendor-gauge.js"` is fetched as `/wwwroot/vendor-gauge.js`.
- Build with `dotnet build -nologo -v q` and fix every error. Warning CS7022 (Program.Main ignored) is expected and harmless.
- Do not run the app yourself and do not start servers; the reviewer runs it in the browser.

## Shared vendor libraries in `_template/IntegrationLab/wwwroot`

| File | What it is | API |
|---|---|---|
| `vendor-gauge.js` | stand-in gauge library, global `VendorGauge` | `new VendorGauge(hostElement, {value,min,max,warnAt,threshold,label,units})`, `.setValue(n)`, `.setOptions({...})`, `.getValue()`, `.on("rangechange"|"thresholdexceeded", fn)`, `.off()`, `.resize()`, `.destroy()`. Events: `rangechange {range, previous, value}`, `thresholdexceeded {value, threshold}` (rising edge only). Throws on non-numeric value. |
| `jquery-lite.js` | tiny jQuery-compatible subset (`window.$`, `$.fn` plugins, `.on/.off/.data/.css/.html/.trigger`) | loads first; real jQuery can replace it |
| `vendor-knob.js` | jQuery-style plugin, **throws `ReferenceError: jQuery is not defined` if loaded before jQuery** | `$(input).vendorKnob({value,min,max,step,label,units,color})`; instance via `$(input).data("vendorKnob")`: `.setValue(n)`, `.setOptions({...})`, `.pulse()`, `.resize()`, `.destroy()`; vendor event `knobchange` dispatched on the `<input>` with `e.detail.value`. It enhances an `<input>` in place (create `<input>` inside `this.container`, then apply the plugin). |

Write any other vendor library you need (chart, grid, pivot, heatmap…) in the same style: a small
self-contained IIFE, a global constructor, `setOptions/setValue`, `.on/.off`, `.destroy()`, vendor-style
lower-case event names, explicit `throw new Error("Vendor…: clear message")` on misuse. Draw with SVG or
plain DOM. Never fetch from a CDN.

## Wisej.Web.Widget — server side (verified)

```csharp
using System.ComponentModel;
using Wisej.Web;

public class TemperatureGauge : Widget
{
    public TemperatureGauge()
    {
        this.Packages.Add(new Package { Name = "vendor-gauge", Source = "wwwroot/vendor-gauge.js" });   // loaded once per page, in list order
        this.InitScript = GetResourceString("IntegrationLab.wwwroot.temperature-gauge.js");          // embedded resource, see csproj
        this.WiredEvents = new[] { "thresholdExceeded", "rangeChanged", "error" };                   // the events the client may raise
        this.Size = new System.Drawing.Size(360, 230);
    }

    public double Value
    {
        get => _value;
        set
        {
            if (value < _minimum || value > _maximum) throw new ArgumentOutOfRangeException(nameof(Value), value, "...");
            if (_value == value) return;
            _value = value;
            dynamic options = this.Options;   // Options is typed object; cast to dynamic
            options.value = value;            // first-level field ⇒ Wisej renders {"value":…} and calls update(options, old) on the client
        }
    }

    protected override void OnWidgetEvent(WidgetEventArgs e)   // every fireWidgetEvent / wired event lands here
    {
        dynamic data = e.Data;
        switch (e.Type)
        {
            case "thresholdExceeded": ThresholdExceeded?.Invoke(this, new GaugeEventArgs(Convert.ToDouble(data?.value))); break;
            default: base.OnWidgetEvent(e); break;
        }
    }
}
```

csproj entry for the InitScript (the logical name must match the string passed to `GetResourceString`):

```xml
<ItemGroup>
  <EmbeddedResource Include="wwwroot\temperature-gauge.js" LogicalName="IntegrationLab.wwwroot.temperature-gauge.js" />
</ItemGroup>
```

Other verified facts:
- `Packages` is `List<Widget.Package>` (`Name`, `Source`, `Integrity`). Stylesheets are packages too (`Source = "wwwroot/vendor.css"`).
- `WiredEvents` is `string[]`. `Options` returns a `Wisej.Core.DynamicObject`; nested changes need `this.Update()` or `((dynamic)Options).Notify("toolbar.show")`; replacing a whole nested object/array counts as a first-level change.
- Anonymous objects assigned into Options are serialized; **property names are camel-cased** (`new { MaxValue = 120 }` → `maxValue`).
- `Instance` is a dynamic proxy for the vendor object stored in `this.widget` on the client: `this.Instance.setValue(72)` (one-way) and `var r = await this.Instance.getStateAsync()` (await). **(unverified)** — Module 1 used Options only.
- `Control.Call("fn", args)` runs `fn` in the client wrapper's context (`this` = wrapper) and returns immediately (queued, flushed with the next response). `Control.CallAsync("fn", args)` returns `Task<dynamic>` with the return value. `Control.EvalAsync("return this.getWidth();")` evaluates JS. Callback variant: `Call("fn", result => {...}, args)`. **(unverified beyond the docs)**
- `IsLoaded` tells whether the client widget has initialized.
- `WidgetEventArgs`: `Type` (string), `Data` (dynamic; read fields by their JavaScript names, convert with `Convert.ToDouble/ToString`).
- A plain `Wisej.Web.Widget` instance (no subclass) works too: set `Packages`, `Options`, `InitScript` from the page/designer code and handle `widget.WidgetEvent += (s, e) => ...` (`e.Type`, `e.Data`).
- Postback data: `Widget` implements `Wisej.Core.IWisejHandler`; URL: `using Wisej.Core; string url = ((IWisejHandler)this).GetPostbackURL();` (append `&action=load`); the request raises `this.WebRequest += (s, e) => { e.Request.QueryString["action"]; e.Response.ContentType = "application/json"; e.Response.Write(json); }` (`WebRequestEventArgs.Request/Response`). Client side: `this.getPostbackUrl()` on the wrapper. **(unverified)**
- **Verified in Modules 8/9/10:** the postback URL (`((IWisejHandler)this).GetPostbackURL()` on the server, `this.getPostbackUrl()` on the client, served at `postback.wx?res=…&x=…`) raises `WebRequest` for **GET only**. A POST to `postback.wx` is consumed by the framework pipeline and answered with `[{"type":0}]`; it never reaches the handler. Send modify operations as `GET …&action=update&payload=<url-encoded JSON>` and read `e.Request.QueryString["payload"]`. `e.Response.StatusCode = 400/404`, `ContentType` and `Write(json)` all pass through to the browser.
- **Verified in Modules 8/9:** `[Wisej.Core.WebMethod]` on instance methods of a top-level container (Page, Form, Desktop) or static methods; child controls register with `OnWebRender(dynamic config) { base.OnWebRender((object)config); RegisterWebMethods(config); }`. The client gets **two** functions per method: `App.MainPage.Name(args…, callback)` (callback style, returns nothing) and `App.MainPage.NameAsync(args…)` → Promise; on a registered child widget `this.NameAsync(args…)`. Never pass `null` arguments (the client calls `arg.getId` on each). **Return values are NOT camel-cased** (a `PageResult` arrives as `{Rows, Total, …}`), unlike Options; a server `ArgumentException` shows the Wisej exception popup and the Promise resolves to `null`. Default parameter values are not supported.
- Browser note for reviewers: a hidden browser tab never fires the qooxdoo "appear" queue, so widgets stay uninitialized (`IsLoaded=false`) until the tab is visible; give a freshly loaded page ~5–8 s before driving it.
- `AlertBox.Show(text, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000)` — always use TopRight so toasts don't cover buttons.
- Fonts: `new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold)`, monospace: `new System.Drawing.Font("monospace", 9F)`. `Label.TextAlign` uses `System.Drawing.ContentAlignment`. `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.
- `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`, `Enabled` — a Component with no visual surface.
- Background work: `Application.StartTask(() => { ...change controls...; Application.Update(this); })`. `Application.Update(component)` pushes pending changes over WebSocket from a non-request thread. Bound the frequency; catch `ObjectDisposedException`/check `IsDisposed`.
- Session entry: `Program.Main(NameValueCollection args)` does `new Window1().Show();` or `Application.MainPage = new MainPage();`.

## Wisej.Web.Widget — client adapter (verified; copy this shape)

`this` is the `wisej.web.Widget` wrapper. `this.container` is the DOM element the framework owns. The framework calls, in order: packages load → `init(options)` → "loaded" → `_addListener(name, handler)` once per `WiredEvents` entry → later `update(options, old)` on every first-level Options change.

```js
// contract event name → vendor event name
this._vendorEvents = { thresholdExceeded: "thresholdexceeded", rangeChanged: "rangechange" };

this.init = function (options) {
    var me = this;                                  // keep the widget context for vendor callbacks
    var host = document.createElement("div");       // vendor lives in a CHILD element, never on this.container itself
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;
    try { this.widget = new VendorGauge(host, options); }      // this.widget = the vendor instance (Instance.xxx() targets it)
    catch (ex) { this.widget = null; this._reportError("init", ex.message); return; }

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.widget) me.widget.resize(); });
        this._resizeObserver.observe(host);
    }
    var frameworkDispose = this.dispose;            // wrap, never replace, the framework dispose
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null;
        } finally { if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments); }
    };
};

this.update = function (options, old) {             // re-sync only what changed
    if (!this.widget) return;
    try { this.widget.setOptions({ min: options.min, max: options.max, value: options.value /* ... */ }); }
    catch (ex) { this._reportError("update", ex.message); }
};

// the framework registers one handler per WiredEvents entry; the handler defers the round trip itself
this._addListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) { if (this.widget) this.widget.on(vendorName, handler); return; }
    if (name === "error") this._errorHandler = handler;
};
this._removeListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) { if (this.widget) this.widget.off(vendorName, handler); return; }
    if (name === "error" && this._errorHandler === handler) this._errorHandler = null;
};
this._getEventData = function (type, e) {           // translate vendor payload → small contract payload
    switch (type) {
        case "thresholdExceeded": return { value: e.value };
        case "rangeChanged": return { range: e.range, value: e.value };
        case "error": return { phase: e.phase, message: e.message };
    }
    return null;
};
this._reportError = function (phase, message) {
    var me = this, data = { phase: phase, message: message };
    if (this._errorHandler) { this._errorHandler(data); return; }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

// functions the server reaches with Call("flash") / CallAsync("getRenderedSize")
this.flash = function () { /* imperative vendor behavior */ };
this.getRenderedSize = function () { var r = this.host.getBoundingClientRect(); return { width: Math.round(r.width), height: Math.round(r.height) }; };

//# sourceURL=integrationlab.widgets.TemperatureGauge.js
```

**Verified in Module 6:** `Call("fn", args)` runs `this.fn(args)` on the wrapper (one-way); `CallAsync("fn")` returns the
function's return value as a dynamic object readable by JavaScript names (`result.width`); `EvalAsync` takes an
**expression** (`"this.measureWidth()"`), never a statement — `"return …;"` fails with "Illegal return statement".

**Critical gotcha (verified in Module 6):** the wrapper is a qooxdoo widget, so a function defined in the InitScript with the
name of a framework method **replaces** it. `this.getWidth = function…` made the layout engine read width 0 and the widget
never initialized. Never define `getWidth`, `getHeight`, `getBounds`, `getLayoutParent`, `destroy`, `dispose` (wrap, don't replace),
`setWidth/setHeight`, `resize`, `show/hide`, `getName`, `getValue`… on the wrapper. Prefix your own functions (`measureWidth`, `gaugeSetValue`).

**Critical gotcha (verified):** `this.fireWidgetEvent(name, data)` called **synchronously inside a vendor
callback that runs during `update()`** (i.e. the vendor event was caused by a server-side Options change)
is silently dropped. Either use the `_addListener` pattern above (preferred) or defer:
`setTimeout(function () { me.fireWidgetEvent(...); }, 0)`. Events caused by user interaction
(drag, click) fire fine synchronously — that is the pattern the course shows in Modules 2 and 7.

## Custom control (Module 5 pattern) — **verified at runtime in Module 5**

Verified: `[assembly: WisejResources]` + embedded `Platform\*.js` are bundled into the client; `config.className`/`config.appearance`;
`config.wiredEvents = new WiredEvents(); config.wiredEvents.Add("thresholdExceeded(Data)")` delivers `fireDataEvent` payloads to
`OnWebEvent` as `e.Parameters.Data`; a `Themes\*.mixin.theme` file in the app folder is merged into the active theme and
`Application.LoadTheme("Material-3")` restyles the control live (available built-in themes in Wisej-4 4.1.0 include Bootstrap-4, Material-3, FluentDark-5).

Server:
```csharp
public class SimpleGaugeControl : Wisej.Web.Control
{
    public SimpleGaugeControl() { this.AppearanceKey = "simplegauge"; this.Size = new System.Drawing.Size(320, 220); }
    public double Value { get => _value; set { if (_value != value) { _value = value; Update(); } } }   // Update() schedules OnWebUpdate/OnWebRender
    protected override void OnWebRender(dynamic config)
    {
        base.OnWebRender((object)config);
        config.className = "integrationlab.controls.SimpleGaugeControl";   // the client qx class
        config.value = _value; config.minimum = _minimum; config.maximum = _maximum; config.caption = _caption;
        config.wiredEvents = new Wisej.Base.WiredEvents(); config.wiredEvents.Add("thresholdExceeded(Data)");   // (Data) = carry e.getData()
    }
    protected override void OnWebEvent(Wisej.Core.WisejEventArgs e)
    {
        switch (e.Type)
        {
            case "thresholdExceeded": { dynamic data = e.Parameters.Data; ...raise .NET event...; break; }
            default: base.OnWebEvent(e); break;      // NEVER swallow unknown events
        }
    }
}
```
Client class file `Platform/SimpleGaugeControl.js`, embedded (`<EmbeddedResource Include="Platform\*.js" />`) and enabled with
`[assembly: Wisej.Core.WisejResources]` in `Properties/AssemblyInfo.cs` (Wisej bundles every embedded `/Platform/*.js` into the client):
```js
qx.Class.define("integrationlab.controls.SimpleGaugeControl", {
  extend: wisej.web.Control,
  construct: function () {
    this.base(arguments);
    this.addListenerOnce("appear", this._createVendor, this);   // DOM exists only after "appear"
    this.addListener("resize", function () { if (this.__gauge) this.__gauge.resize(); }, this);
  },
  properties: {
    appearance: { init: "simplegauge", refine: true },
    value:   { init: 0,   check: "Number", apply: "_applyValue" },
    minimum: { init: 0,   check: "Number", apply: "_applyRange" },
    maximum: { init: 100, check: "Number", apply: "_applyRange" },
    caption: { init: "",  check: "String", apply: "_applyCaption" }
  },
  members: {
    __gauge: null,
    _createVendor: function () {
      var el = this.getContentElement().getDomElement();
      this.__gauge = new VendorGauge(el, { value: this.getValue(), min: this.getMinimum(), max: this.getMaximum(), label: this.getCaption() });
      var me = this;
      this.__gauge.on("thresholdexceeded", function (e) { me.fireDataEvent("thresholdExceeded", { value: e.value }); });
    },
    _applyValue: function (value) { if (this.__gauge) this.__gauge.setValue(value); },
    _applyRange: function () { if (this.__gauge) this.__gauge.setOptions({ min: this.getMinimum(), max: this.getMaximum() }); },
    _applyCaption: function (value) { if (this.__gauge) this.__gauge.setOptions({ label: value }); }
  },
  destruct: function () { if (this.__gauge) { this.__gauge.destroy(); this.__gauge = null; } }
});
```
Theme entry: `Themes/simplegauge.mixin.theme` (JSON) — `{"name":"simplegauge","appearances":{"simplegauge":{"states":{"default":{"styles":{"backgroundColor":"white","radius":10,"border":[1,"solid","#dce4ec"]},"properties":{"textColor":"@text-primary"}}}}}}` — mixin files in the app's Themes folder are merged into the active theme.
Put the vendor library under `Platform/` too (embedded) so it is bundled with the client class.

## UI conventions used by every sample (so the samples feel like one course)

- A `Form` named `Window1` (or a `Page`) sized about 1348×680, light grey background `Color.FromArgb(238,242,247)`, white cards with `BorderStyle.Solid`.
- Right-hand card: **"Server ⇄ Client · live message trace"** — a `ListBox` with monospace font; every message in both directions is logged as `HH:mm:ss.fff  → .NET→JS name {json}` / `← JS→.NET name {json}` / `• server …`. Select the last item after adding.
- Bottom bar of buttons that exercise: the **success path**, a **progress path** (Timer stream), at least one **failure path** (server-side validation rejection and/or a caught vendor error) and **recovery** (resync from server state).
- A status label (● streaming / warm / alarm / fault) and an alarm/error banner label that appears and disappears.
- Designer-style `Window1.Designer.cs` with `InitializeComponent()` so the file opens in the Wisej Designer; code-behind in `Window1.cs`.

## Docs (`docs/`)

Write each lab deliverable as its own Markdown file, named as the course names it (e.g. `WidgetTriage.md`, `IntegrationDecisionRecord.md`, `ClientServerContract.md`, `DebuggingNotes.md`, `PayloadContract.md`, `ComparisonNote.md`, `ProductionChecklist.md`). Include a short **Evidence** section describing what the running app shows for each path, and the **self-check answers** from the lab guide in the README.
