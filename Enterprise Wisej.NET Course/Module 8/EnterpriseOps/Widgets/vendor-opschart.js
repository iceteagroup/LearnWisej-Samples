/*!
 * EnterpriseOpsChart 1.2 — the "third-party" status chart library of the Module 8 lab.
 *
 * It is deliberately written the way a vendor library is written: a self-contained IIFE, one
 * global constructor, vendor-style lower-case event names, its own option names, its own
 * error messages, and no idea that Wisej.NET exists. Nothing in this file may reference
 * Wisej, the wrapper, the server or the EnterpriseOps domain — that is exactly the point of
 * the wrapper (Widgets/WorkOrderChartWidget.cs) and of its adapter (Widgets/opschart-init.js).
 *
 * Public API
 *   var chart = new EnterpriseOpsChart(hostElement, options)
 *   chart.setOptions({...})        replace any subset of the options, re-draws
 *   chart.getSelectedKey()         the key of the highlighted slice, or null
 *   chart.select(key)              highlight a slice without firing an event
 *   chart.on(name, fn)             "pointclick" | "renderfail"
 *   chart.off(name, fn)            remove one handler (or all handlers of a name)
 *   chart.resize()                 re-draw at the host's current size
 *   chart.destroy()                remove the SVG and drop every handler
 *
 * Options
 *   segments   [{ key, label, value }]  required, values must be finite and >= 0
 *   palette    "ops" | "mono" | "highcontrast"   (default "ops")
 *   caption    string shown top-left               (default "")
 *   showLegend boolean                             (default true)
 *   badge      string shown as a pill next to the caption, "" = hidden (default "")
 *
 * Events
 *   pointclick { key, label, value, percent }   a slice was clicked
 *   renderfail { message }                      drawing failed after construction
 *
 * Misuse throws synchronously: `new Error("EnterpriseOpsChart: ...")`. The Wisej adapter is
 * expected to catch those and turn them into the wrapper's "error" event.
 */
(function (global) {
    "use strict";

    var NS = "http://www.w3.org/2000/svg";

    var PALETTES = {
        ops: { open: "#1a86ff", onhold: "#b9770e", escalated: "#c0392b", done: "#1f8a4c", other: "#8a97a4" },
        mono: { open: "#5b6b7c", onhold: "#8a97a4", escalated: "#2f3d4b", done: "#aab6c2", other: "#c3ccd5" },
        highcontrast: { open: "#0050c8", onhold: "#a04a00", escalated: "#a30000", done: "#00602a", other: "#3a3a3a" }
    };

    function isFiniteNumber(n) {
        return typeof n === "number" && isFinite(n);
    }

    function validate(options) {
        if (!options || typeof options !== "object")
            throw new Error("EnterpriseOpsChart: options must be an object.");

        if (!Array.isArray(options.segments))
            throw new Error("EnterpriseOpsChart: options.segments must be an array of {key,label,value}.");

        if (options.segments.length === 0)
            throw new Error("EnterpriseOpsChart: options.segments must contain at least one segment.");

        for (var i = 0; i < options.segments.length; i++) {
            var s = options.segments[i];
            if (!s || typeof s.key !== "string" || s.key === "")
                throw new Error("EnterpriseOpsChart: segment " + i + " has no key.");
            if (!isFiniteNumber(s.value) || s.value < 0)
                throw new Error("EnterpriseOpsChart: segment '" + s.key + "' has a value that is not a number >= 0.");
        }

        if (options.palette != null && !PALETTES[options.palette])
            throw new Error("EnterpriseOpsChart: unknown palette '" + options.palette + "'. Known palettes: " +
                Object.keys(PALETTES).join(", ") + ".");
    }

    function EnterpriseOpsChart(host, options) {
        if (!host || !host.appendChild)
            throw new Error("EnterpriseOpsChart: the first argument must be a DOM element.");

        validate(options);

        this.version = "1.2";
        this._host = host;
        this._handlers = { pointclick: [], renderfail: [] };
        this._selected = null;
        this._options = {
            segments: options.segments.slice(),
            palette: options.palette || "ops",
            caption: options.caption || "",
            showLegend: options.showLegend !== false,
            badge: options.badge || ""
        };

        this._root = document.createElement("div");
        this._root.className = "eops-chart";
        host.appendChild(this._root);

        this._draw();
    }

    EnterpriseOpsChart.prototype.setOptions = function (patch) {
        var merged = {
            segments: patch && patch.segments !== undefined ? patch.segments : this._options.segments,
            palette: patch && patch.palette !== undefined ? patch.palette : this._options.palette,
            caption: patch && patch.caption !== undefined ? patch.caption : this._options.caption,
            showLegend: patch && patch.showLegend !== undefined ? patch.showLegend : this._options.showLegend,
            badge: patch && patch.badge !== undefined ? patch.badge : this._options.badge
        };

        validate(merged);                          // throws before anything on screen changes

        merged.segments = merged.segments.slice();
        this._options = merged;

        if (this._selected && !this._find(this._selected))
            this._selected = null;

        this._draw();
    };

    EnterpriseOpsChart.prototype.getSelectedKey = function () {
        return this._selected;
    };

    EnterpriseOpsChart.prototype.select = function (key) {
        if (key != null && !this._find(key))
            throw new Error("EnterpriseOpsChart: no segment with key '" + key + "'.");
        this._selected = key == null ? null : key;
        this._draw();
    };

    EnterpriseOpsChart.prototype.on = function (name, fn) {
        if (!this._handlers[name])
            throw new Error("EnterpriseOpsChart: unknown event '" + name + "'. Known events: pointclick, renderfail.");
        this._handlers[name].push(fn);
        return this;
    };

    EnterpriseOpsChart.prototype.off = function (name, fn) {
        if (!this._handlers[name]) return this;
        if (!fn) { this._handlers[name] = []; return this; }
        var i = this._handlers[name].indexOf(fn);
        if (i >= 0) this._handlers[name].splice(i, 1);
        return this;
    };

    EnterpriseOpsChart.prototype.resize = function () {
        this._draw();
    };

    EnterpriseOpsChart.prototype.destroy = function () {
        this._handlers = { pointclick: [], renderfail: [] };
        if (this._root && this._root.parentNode)
            this._root.parentNode.removeChild(this._root);
        this._root = null;
        this._host = null;
    };

    // ---- internals ---------------------------------------------------------

    EnterpriseOpsChart.prototype._find = function (key) {
        for (var i = 0; i < this._options.segments.length; i++)
            if (this._options.segments[i].key === key) return this._options.segments[i];
        return null;
    };

    EnterpriseOpsChart.prototype._emit = function (name, payload) {
        var list = this._handlers[name] || [];
        for (var i = 0; i < list.length; i++) {
            try { list[i](payload); } catch (ignored) { /* a subscriber must not break the chart */ }
        }
    };

    EnterpriseOpsChart.prototype._draw = function () {
        if (!this._root) return;

        try {
            var me = this;
            var opts = this._options;
            var colors = PALETTES[opts.palette];
            var total = 0, i;

            for (i = 0; i < opts.segments.length; i++) total += opts.segments[i].value;
            if (total <= 0) total = 1;                          // an all-zero chart draws one empty rail

            this._root.innerHTML = "";

            // caption row
            var head = document.createElement("div");
            head.className = "eops-chart-head";
            var title = document.createElement("span");
            title.className = "eops-chart-title";
            title.textContent = opts.caption;
            head.appendChild(title);
            if (opts.badge) {
                var badge = document.createElement("span");
                badge.className = "eops-chart-badge";
                badge.textContent = opts.badge;
                head.appendChild(badge);
            }
            var vendorTag = document.createElement("span");
            vendorTag.className = "eops-chart-vendor";
            vendorTag.textContent = "EnterpriseOpsChart " + this.version;
            head.appendChild(vendorTag);
            this._root.appendChild(head);

            // the bar itself, drawn as SVG
            var width = Math.max(120, this._host ? this._host.clientWidth : 400);
            var barHeight = 34;
            var svg = document.createElementNS(NS, "svg");
            svg.setAttribute("class", "eops-chart-bar");
            svg.setAttribute("width", "100%");
            svg.setAttribute("height", String(barHeight));
            svg.setAttribute("viewBox", "0 0 " + width + " " + barHeight);
            svg.setAttribute("preserveAspectRatio", "none");

            var x = 0;
            for (i = 0; i < opts.segments.length; i++) {
                var s = opts.segments[i];
                var w = Math.max(0, (s.value / total) * width);
                var percent = Math.round((s.value / total) * 100);
                var selected = this._selected === s.key;

                var rect = document.createElementNS(NS, "rect");
                rect.setAttribute("x", String(x));
                rect.setAttribute("y", "0");
                rect.setAttribute("width", String(w));
                rect.setAttribute("height", String(barHeight));
                rect.setAttribute("fill", colors[s.key] || colors.other);
                rect.setAttribute("opacity", selected ? "1" : "0.82");
                rect.setAttribute("cursor", "pointer");
                rect.setAttribute("data-key", s.key);
                svg.appendChild(rect);

                if (selected) {
                    var frame = document.createElementNS(NS, "rect");
                    frame.setAttribute("x", String(x + 1.5));
                    frame.setAttribute("y", "1.5");
                    frame.setAttribute("width", String(Math.max(0, w - 3)));
                    frame.setAttribute("height", String(barHeight - 3));
                    frame.setAttribute("fill", "none");
                    frame.setAttribute("stroke", "#0d1b2a");
                    frame.setAttribute("stroke-width", "3");
                    frame.setAttribute("pointer-events", "none");
                    svg.appendChild(frame);
                }

                if (w > 54) {
                    var text = document.createElementNS(NS, "text");
                    text.setAttribute("x", String(x + w / 2));
                    text.setAttribute("y", String(barHeight / 2 + 4));
                    text.setAttribute("text-anchor", "middle");
                    text.setAttribute("font-size", "11");
                    text.setAttribute("font-weight", "700");
                    text.setAttribute("fill", "#ffffff");
                    text.setAttribute("pointer-events", "none");
                    text.textContent = w > 96 ? (s.label + " " + percent + "%") : (percent + "%");
                    svg.appendChild(text);
                }

                (function (segment, pct) {
                    rect.addEventListener("click", function () {
                        me._selected = segment.key;
                        me._draw();
                        me._emit("pointclick", { key: segment.key, label: segment.label, value: segment.value, percent: pct });
                    });
                })(s, percent);

                x += w;
            }
            this._root.appendChild(svg);

            // legend
            if (opts.showLegend) {
                var legend = document.createElement("div");
                legend.className = "eops-chart-legend";
                for (i = 0; i < opts.segments.length; i++) {
                    var item = document.createElement("span");
                    item.className = "eops-chart-legend-item";
                    var swatch = document.createElement("span");
                    swatch.className = "eops-chart-swatch";
                    swatch.style.background = colors[opts.segments[i].key] || colors.other;
                    item.appendChild(swatch);
                    item.appendChild(document.createTextNode(opts.segments[i].label + " · " + opts.segments[i].value));
                    legend.appendChild(item);
                }
                this._root.appendChild(legend);
            }
        }
        catch (ex) {
            this._emit("renderfail", { message: ex && ex.message ? ex.message : String(ex) });
        }
    };

    global.EnterpriseOpsChart = EnterpriseOpsChart;

})(window);
