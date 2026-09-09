/*!
 * VendorKnob 1.0 — a stand-in for a jQuery-style knob plugin.
 * REQUIRES jQuery (or jquery-lite.js) to be loaded first: it registers itself
 * as $.fn.vendorKnob, exactly like a real plugin. Loading it before jQuery
 * throws "ReferenceError: jQuery is not defined" — the load-order failure
 * the course talks about.
 *
 * Usage (plugin style):
 *   $(input).vendorKnob({ value: 40, min: 0, max: 100, label: "Pressure" });
 *   var knob = $(input).data("vendorKnob");     // the instance
 *   knob.setValue(55); knob.setOptions({...}); knob.pulse(); knob.destroy();
 *   $(input).on("knobchange", function (e) { e.detail.value });   // vendor event
 *
 * The plugin enhances an <input> element in place (it hides the input and
 * draws a dial next to it) — the "library that wants a specific element" case.
 */
(function (factory) {
  if (typeof jQuery === "undefined")
    throw new ReferenceError("jQuery is not defined — vendor-knob.js must be loaded after jQuery.");
  factory(jQuery);
})(function ($) {
  "use strict";

  var SVG = "http://www.w3.org/2000/svg";
  var DEFAULTS = { value: 0, min: 0, max: 100, step: 1, label: "", units: "", color: "#1a86ff" };

  function el(name, attrs, parent) {
    var n = document.createElementNS(SVG, name);
    for (var k in attrs) n.setAttribute(k, attrs[k]);
    if (parent) parent.appendChild(n);
    return n;
  }
  function clamp(v, a, b) { return Math.max(a, Math.min(b, v)); }
  function polar(cx, cy, r, deg) { var a = deg * Math.PI / 180; return [cx + r * Math.cos(a), cy - r * Math.sin(a)]; }
  function arc(cx, cy, r, a0, a1) {
    var s = polar(cx, cy, r, a0), e = polar(cx, cy, r, a1), large = Math.abs(a0 - a1) > 180 ? 1 : 0;
    return "M " + s[0].toFixed(1) + " " + s[1].toFixed(1) + " A " + r + " " + r + " 0 " + large + " 1 " + e[0].toFixed(1) + " " + e[1].toFixed(1);
  }

  function VendorKnob(input, options) {
    if (!input || input.tagName !== "INPUT")
      throw new Error("VendorKnob: the plugin must be applied to an <input> element.");
    this.input = input;
    this.opts = $.extend({}, DEFAULTS, options);
    this._destroyed = false;
    this._build();
    this.setValue(this.opts.value, true);
  }

  VendorKnob.prototype._build = function () {
    var host = document.createElement("div");
    host.className = "vendor-knob";
    host.style.cssText = "position:relative;width:100%;height:100%;user-select:none;";
    this.input.style.display = "none";
    this.input.parentNode.insertBefore(host, this.input.nextSibling);
    this.host = host;

    var svg = el("svg", { viewBox: "0 0 200 200", preserveAspectRatio: "xMidYMid meet" });
    svg.style.cssText = "display:block;width:100%;height:100%;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;";
    el("path", { d: arc(100, 100, 70, 225, -45), fill: "none", stroke: "#e6ebf1", "stroke-width": 14, "stroke-linecap": "round" }, svg);
    this.valueArc = el("path", { fill: "none", stroke: this.opts.color, "stroke-width": 14, "stroke-linecap": "round" }, svg);
    this.dot = el("circle", { r: 7, fill: "#16314e" }, svg);
    this.readout = el("text", { x: 100, y: 108, "text-anchor": "middle", "font-size": 30, "font-weight": 800, fill: "#0d1b2a" }, svg);
    this.labelText = el("text", { x: 100, y: 132, "text-anchor": "middle", "font-size": 11, "font-weight": 700, fill: "#8a98a8", "letter-spacing": 1 }, svg);
    host.appendChild(svg);
    this.svg = svg;

    // drag / wheel interaction → vendor "knobchange" event on the input
    var me = this;
    this._onWheel = function (e) { e.preventDefault(); me.setValue(me.value + (e.deltaY < 0 ? me.opts.step : -me.opts.step)); };
    this._onPointer = function (e) {
      if (e.type === "pointerdown") { me._dragging = true; host.setPointerCapture(e.pointerId); }
      if (e.type === "pointerup") { me._dragging = false; return; }
      if (!me._dragging && e.type !== "pointerdown") return;
      var r = host.getBoundingClientRect(), x = e.clientX - (r.left + r.width / 2), y = (r.top + r.height / 2) - e.clientY;
      var deg = Math.atan2(y, x) * 180 / Math.PI; if (deg < -90) deg += 360;      // 225 (min) → -45 (max)
      var p = clamp((225 - deg) / 270, 0, 1);
      me.setValue(me.opts.min + Math.round(p * (me.opts.max - me.opts.min) / me.opts.step) * me.opts.step);
    };
    host.addEventListener("wheel", this._onWheel, { passive: false });
    host.addEventListener("pointerdown", this._onPointer);
    host.addEventListener("pointermove", this._onPointer);
    host.addEventListener("pointerup", this._onPointer);
  };

  VendorKnob.prototype.getValue = function () { return this.value; };

  VendorKnob.prototype.setValue = function (v, silent) {
    if (this._destroyed) throw new Error("VendorKnob.setValue: instance has been destroyed.");
    if (typeof v !== "number" || !isFinite(v))
      throw new Error("VendorKnob.setValue: value must be a finite number, received " + JSON.stringify(v) + ".");
    v = clamp(v, this.opts.min, this.opts.max);
    var changed = v !== this.value;
    this.value = v;
    this.input.value = v;
    this._draw();
    if (changed && !silent) this.input.dispatchEvent(new CustomEvent("knobchange", { detail: { value: v }, bubbles: true }));
  };

  VendorKnob.prototype.setOptions = function (partial) {
    if (this._destroyed) throw new Error("VendorKnob.setOptions: instance has been destroyed.");
    var hasValue = partial && partial.value !== undefined;
    for (var k in partial) if (k !== "value" && partial[k] !== undefined) this.opts[k] = partial[k];
    if (!(this.opts.min < this.opts.max)) throw new Error("VendorKnob.setOptions: min must be less than max.");
    this.valueArc.setAttribute("stroke", this.opts.color);
    this.setValue(hasValue ? partial.value : this.value, true);
  };

  // imperative vendor method (used by the course's Call examples)
  VendorKnob.prototype.pulse = function () {
    var svg = this.svg;
    svg.style.transition = "transform 120ms ease-out";
    svg.style.transform = "scale(1.08)";
    setTimeout(function () { svg.style.transform = "scale(1)"; }, 140);
  };

  VendorKnob.prototype.resize = function () { this._draw(); };

  VendorKnob.prototype.destroy = function () {
    if (this._destroyed) return;
    this._destroyed = true;
    this.host.removeEventListener("wheel", this._onWheel);
    this.host.removeEventListener("pointerdown", this._onPointer);
    this.host.removeEventListener("pointermove", this._onPointer);
    this.host.removeEventListener("pointerup", this._onPointer);
    if (this.host.parentNode) this.host.parentNode.removeChild(this.host);
    this.input.style.display = "";
    $(this.input).data("vendorKnob", null);
    this.host = null; this.svg = null;
  };

  VendorKnob.prototype._draw = function () {
    var p = clamp((this.value - this.opts.min) / (this.opts.max - this.opts.min), 0, 1);
    var deg = 225 - p * 270, tip = polar(100, 100, 70, deg);
    this.valueArc.setAttribute("d", p > 0.002 ? arc(100, 100, 70, 225, deg) : "");
    this.dot.setAttribute("cx", tip[0]); this.dot.setAttribute("cy", tip[1]);
    this.readout.textContent = this.value + (this.opts.units || "");
    this.labelText.textContent = (this.opts.label || "").toUpperCase();
  };

  // ---- the jQuery plugin entry point ---------------------------------------
  $.fn.vendorKnob = function (options) {
    return this.each(function () {
      var existing = $(this).data("vendorKnob");
      if (existing) { existing.setOptions(options || {}); return; }
      $(this).data("vendorKnob", new VendorKnob(this, options || {}));
    });
  };
  $.fn.vendorKnob.Constructor = VendorKnob;
});
