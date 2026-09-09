# Reaching into the client object model · when it is fine and when it is a review finding

Wisej.NET reflects its component tree into the browser. From a page script you can reach:

| Expression | What it returns |
|---|---|
| `App.MainPage` | the current main page (or `App.Desktop` / the main window, depending on the startup) |
| `App.MainPage.panelGauge` | a named child control as a property of its container (named components are reflected) |
| `App.MainPage.panelGauge.gauge` | the gauge wrapper itself, two levels down |
| `widget("id_123")` | any component by its client identifier |

This is convenient in a demo: a script can grab a button, read a text box, or find the gauge by
walking the tree. It is also fragile: the path `App.MainPage.panelGauge.gauge` breaks the moment
somebody renames `panelGauge`, moves the gauge into a `GroupBox`, hosts the window as a child of a
`Desktop`, or reuses the control on a second page where the container is called something else.

## The rule this lab follows

**Page-level scripts under the application's control may use the object model. Integration
wrappers never do.**

The wrapper (`wwwroot/gauge-init.js`) already has everything it needs:

- `this` — itself; every command from the server arrives as a call on `this` (`Call("setValue")` → `this.setValue`)
- `this.container` — the element the framework gave it; `this.host` — the child it created
- `this.widget` — the vendor instance it created
- `options` — every piece of state the server wants it to have, delivered to `init` / `update`
- `fireWidgetEvent` / the `_addListener` handlers — the only way it talks back

None of the gauge commands is looked up by path. `SimpleGauge.SetValue` calls
`this.Call("setValue", v)` on the control, and the framework routes it to that control's wrapper
wherever it sits. Move the gauge to another panel, rename every container, drop it on a second
window: nothing in `gauge-init.js` changes.

### What a review flags

```js
// REVIEW FINDING — inside a wrapper
this.setValue = function (v) {
    var trace = App.MainPage.panelTrace.listTrace;     // depends on a page it does not own
    trace.addItem("client set " + v);
    App.MainPage.panelGauge.gauge.widget.setValue(v);  // depends on a path AND on its own name
};
```

Three fragile assumptions in four lines: that there is a `MainPage`, that it has `panelTrace` and
`panelGauge`, and that the gauge is *this* gauge. It also reaches into another control's client
object, which the framework may replace at any time.

### What is fine

```js
// OK — a page-level script (e.g. in Default.html or a Page's InitScript), owned by the application
document.addEventListener("keydown", function (e) {
    if (e.ctrlKey && e.key === "r") {
        var gauge = App.MainPage && App.MainPage.panelGauge && App.MainPage.panelGauge.gauge;
        if (gauge && gauge.resetAnimation) gauge.resetAnimation();   // calls the wrapper's public command
    }
});
```

The application owns both the page and the path; if the path changes, the same developer changes
the shortcut. Note that even the page script only calls the wrapper's **public command** — it does
not touch `gauge.widget`.

## Server side: the same principle

The server never reaches for a client identifier either. `SimpleGauge` calls its own functions on
its own wrapper (`this.Call`, `this.CallAsync`, `this.EvalAsync` with `this` = the wrapper). It does
not `Eval("widget('id_7').setValue(72)")`, and Window1 does not evaluate JavaScript against the
page to find the gauge. The control is the address.

## Checklist for a wrapper review

- [ ] no `App.`, `widget(`, `qx.core.Init`, `document.getElementById` of other components
- [ ] no `.parent` / `getLayoutParent()` walking to reach siblings
- [ ] every command is a function on `this`, callable from C# with `Call`/`CallAsync`
- [ ] every piece of state arrives through `options`
- [ ] every message back goes through `fireWidgetEvent` / wired events
- [ ] the wrapper works unchanged when dropped on a second page with different container names

## Evidence

`wwwroot/gauge-init.js` contains no reference to `App`, `widget(`, `MainPage` or any component name;
searching the file for those strings finds only this rule in its header comment. The four commands
on the finished screen are all issued on the control (`this.gauge.SetValue(72)`, …) and land on the
control's own wrapper.
