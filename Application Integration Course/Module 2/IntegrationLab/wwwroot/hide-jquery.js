/*!
 * hide-jquery.js — used ONLY by the "Wrong package order" button in Window1.
 *
 * vendor-knob.js checks the jQuery global exactly once, at the moment its script executes.
 * On the Knob Demo page jQuery is already loaded (gaugeKnob listed it first, in the right
 * order), so a third widget that lists vendor-knob.js BEFORE jquery-lite.js would silently
 * work here and teach nothing. This prop PARKS the two globals under window.__jqParked for
 * the duration of the wrong-order load, so the plugin script runs, finds no jQuery and throws
 *
 *     ReferenceError: jQuery is not defined — vendor-knob.js must be loaded after jQuery.
 *
 * The server restores the globals right after its IsLoaded check (Eval in Window1.cs), so the
 * two working knobs are never affected. It is a lab prop: not part of the fixed widget, never ships.
 */
window.__jqParked = { jQuery: window.jQuery, $: window.$ };
delete window.jQuery;
delete window.$;
