/*!
 * jquery-lite 1.0 — a deliberately tiny jQuery-compatible subset used by the
 * course samples so that "jQuery-style" plugins can be demonstrated without
 * downloading the real library. It exposes window.$ / window.jQuery, the
 * $.fn plugin mechanism, and the handful of methods the sample plugins use.
 *
 * Swap this file for the real jquery.min.js in production: the plugins only
 * rely on the standard API surface implemented here.
 */
(function (global) {
  "use strict";

  function Lite(nodes) {
    this.length = nodes.length;
    for (var i = 0; i < nodes.length; i++) this[i] = nodes[i];
  }

  function $(arg, context) {
    if (arg instanceof Lite) return arg;
    if (typeof arg === "function") { if (document.readyState !== "loading") arg($); else document.addEventListener("DOMContentLoaded", function () { arg($); }); return; }
    if (typeof arg === "string") {
      if (arg.charAt(0) === "<") { var t = document.createElement("div"); t.innerHTML = arg.trim(); return new Lite(Array.prototype.slice.call(t.children)); }
      return new Lite(Array.prototype.slice.call((context || document).querySelectorAll(arg)));
    }
    if (arg && arg.nodeType) return new Lite([arg]);
    if (arg && typeof arg.length === "number") return new Lite(Array.prototype.slice.call(arg));
    return new Lite([]);
  }

  $.fn = Lite.prototype;
  $.extend = function (target) {
    for (var i = 1; i < arguments.length; i++) { var src = arguments[i]; if (src) for (var k in src) target[k] = src[k]; }
    return target;
  };

  $.fn.each = function (fn) { for (var i = 0; i < this.length; i++) fn.call(this[i], i, this[i]); return this; };
  $.fn.get = function (i) { return i === undefined ? Array.prototype.slice.call(this) : this[i]; };
  $.fn.find = function (sel) { var out = []; this.each(function () { out.push.apply(out, Array.prototype.slice.call(this.querySelectorAll(sel))); }); return new Lite(out); };
  $.fn.html = function (v) { if (v === undefined) return this.length ? this[0].innerHTML : undefined; return this.each(function () { this.innerHTML = v; }); };
  $.fn.text = function (v) { if (v === undefined) return this.length ? this[0].textContent : undefined; return this.each(function () { this.textContent = v; }); };
  $.fn.append = function (child) { var nodes = typeof child === "string" ? $(child).get() : (child instanceof Lite ? child.get() : [child]); return this.each(function () { for (var i = 0; i < nodes.length; i++) this.appendChild(nodes[i]); }); };
  $.fn.empty = function () { return this.each(function () { this.innerHTML = ""; }); };
  $.fn.remove = function () { return this.each(function () { if (this.parentNode) this.parentNode.removeChild(this); }); };
  $.fn.addClass = function (c) { return this.each(function () { this.classList.add(c); }); };
  $.fn.removeClass = function (c) { return this.each(function () { this.classList.remove(c); }); };
  $.fn.attr = function (n, v) { if (v === undefined) return this.length ? this[0].getAttribute(n) : undefined; return this.each(function () { this.setAttribute(n, v); }); };
  $.fn.css = function (n, v) {
    if (typeof n === "object") { var o = n; return this.each(function () { for (var k in o) this.style[k] = o[k]; }); }
    if (v === undefined) return this.length ? getComputedStyle(this[0])[n] : undefined;
    return this.each(function () { this.style[n] = v; });
  };
  $.fn.width = function () { return this.length ? this[0].getBoundingClientRect().width : 0; };
  $.fn.height = function () { return this.length ? this[0].getBoundingClientRect().height : 0; };
  $.fn.data = function (k, v) {
    if (v === undefined) return this.length ? (this[0].__liteData || {})[k] : undefined;
    return this.each(function () { (this.__liteData = this.__liteData || {})[k] = v; });
  };
  $.fn.on = function (name, fn) { return this.each(function () { this.addEventListener(name, fn); (this.__liteHandlers = this.__liteHandlers || []).push([name, fn]); }); };
  $.fn.off = function (name, fn) {
    return this.each(function () {
      var list = this.__liteHandlers || [], keep = [];
      for (var i = 0; i < list.length; i++) {
        var h = list[i];
        if ((!name || h[0] === name) && (!fn || h[1] === fn)) this.removeEventListener(h[0], h[1]); else keep.push(h);
      }
      this.__liteHandlers = keep;
    });
  };
  $.fn.trigger = function (name, detail) { return this.each(function () { this.dispatchEvent(new CustomEvent(name, { detail: detail, bubbles: true })); }); };

  global.$ = global.jQuery = $;
})(window);
