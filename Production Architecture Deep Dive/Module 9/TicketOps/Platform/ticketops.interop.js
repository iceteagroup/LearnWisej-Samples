// ticketops.interop.js — the only JavaScript in the TicketOps Console.
//
// Packaging: this file is an embedded resource under /Platform and the assembly carries
// [assembly: Wisej.Core.WisejResources], so Wisej.NET bundles it into the client and `window.ticketOps`
// exists before any widget renders. Nothing here decides anything: it focuses, copies and shows.
// The server re-validates every value that crosses back (see docs/InteropSecurityNotes.md).
(function (global) {
    "use strict";

    var ticketOps = global.ticketOps = global.ticketOps || {};

    var SHORTCUTS = [
        { keys: "Ctrl+K", what: "Focus the global search" },
        { keys: "Esc", what: "Clear the search box" },
        { keys: "?", what: "Show or hide this list" }
    ];

    function isCtrlK(ev) {
        return (ev.ctrlKey || ev.metaKey) && !ev.altKey && String(ev.key).toLowerCase() === "k";
    }

    function isTypingTarget(el) {
        var tag = el && el.tagName ? el.tagName.toLowerCase() : "";
        return tag === "input" || tag === "textarea" || tag === "select" || (el && el.isContentEditable === true);
    }

    // ── Ctrl+K (client-side key handling) ────────────────────────────────────────────────────────────
    // Called by the Wisej JavaScript extender attached to GlobalSearchBox; `widget` is the widget
    // (`this` in the extender script), so the handler is registered only once the widget exists.
    // Idempotent: a refresh re-runs the extender, and the previous listener is removed first.
    ticketOps.attachSearchShortcuts = function (widget) {
        if (!widget || typeof widget.focus !== "function") {
            console.warn("[ticketOps] attachSearchShortcuts: expected the search widget as `this`.");
            return false;
        }

        if (widget.__ticketOpsKeydown)
            global.removeEventListener("keydown", widget.__ticketOpsKeydown, true);

        var handler = function (ev) {
            if (isCtrlK(ev)) {
                ev.preventDefault();                 // keep the browser's own Ctrl+K (address-bar search) out of the way
                ev.stopPropagation();
                widget.focus();
                if (typeof widget.selectAllText === "function") {
                    try { widget.selectAllText(); } catch (ignored) { /* selection is a nicety */ }
                }
                ticketOps._reportShortcut(widget, "ctrl+k");   // the server is told
                return;
            }

            if (ev.key === "Escape") {
                if (ticketOps._hideShortcutList()) { ev.preventDefault(); return; }
                var focused = typeof widget.hasState === "function" && widget.hasState("focused");
                if (focused && typeof widget.getValue === "function" && widget.getValue()) {
                    ev.preventDefault();
                    widget.setValue("");             // fires changeValue → Wisej raises TextChanged on the server
                }
                return;
            }

            if (ev.key === "?" && !ev.ctrlKey && !ev.metaKey && !ev.altKey && !isTypingTarget(ev.target)) {
                ev.preventDefault();
                ticketOps.toggleShortcutList();
            }
        };

        widget.__ticketOpsKeydown = handler;
        global.addEventListener("keydown", handler, true);   // capture phase: runs before any widget handler
        return true;
    };

    // ── client → server callback ─────────────────────────────────────────────────────────────────────
    // GlobalSearchBox.ReportShortcut is a [WebMethod] registered on the widget by Wisej.NET
    // (RegisterWebMethods). Two shapes exist: NameAsync(args) → Promise, or Name(args, callback).
    ticketOps._reportShortcut = function (widget, name) {
        try {
            if (typeof widget.ReportShortcutAsync === "function") {
                widget.ReportShortcutAsync(name).then(
                    function (accepted) { if (accepted === false) console.warn("[ticketOps] the server refused the shortcut report."); },
                    function (err) { console.warn("[ticketOps] ReportShortcut failed:", err); });
            }
            else if (typeof widget.ReportShortcut === "function") {
                widget.ReportShortcut(name, function () { /* accepted or refused: the server decided */ });
            }
            else {
                console.warn("[ticketOps] ReportShortcut is not registered on the search widget; focus still works.");
            }
        }
        catch (ex) {
            console.warn("[ticketOps] ReportShortcut threw:", ex);
        }
    };

    // ── clipboard (server → browser → server) ────────────────────────────────────────────────────────
    // Invoked by BrowserApi.CopyToClipboardAsync through Application.EvalAsync. `text` is the finished,
    // server-built link (this code never assembles a URL). Returns a Promise that ALWAYS resolves to
    // { ok: true } or { ok: false, reason, detail } — it never rejects, so the server always gets an answer.
    ticketOps.copyToClipboard = function (text) {
        var value = String(text);

        var write;
        if (global.navigator && navigator.clipboard && typeof navigator.clipboard.writeText === "function") {
            write = function () { return navigator.clipboard.writeText(value); };
        }
        else {
            write = function () { return Promise.reject(new DOMException("navigator.clipboard is unavailable (insecure context or unsupported browser).", "NotSupportedError")); };
        }

        return new Promise(function (resolve) {
            var settled = false;
            function done(result) { if (!settled) { settled = true; resolve(result); } }

            try {
                write().then(
                    function () { done({ ok: true }); },
                    function (err) { done({ ok: false, reason: (err && err.name) || "Error", detail: (err && err.message) || String(err) }); });
            }
            catch (ex) {
                done({ ok: false, reason: (ex && ex.name) || "Error", detail: (ex && ex.message) || String(ex) });
            }

            // A browser that never answers (blocked permission prompt) must not hang the server-side await.
            setTimeout(function () { done({ ok: false, reason: "Timeout", detail: "The browser did not answer within 4 seconds." }); }, 4000);
        });
    };

    // ── the shortcut list (browser only) ─────────────────────────────────────────────────────────────
    var listElement = null;

    ticketOps.toggleShortcutList = function () {
        if (listElement) { ticketOps._hideShortcutList(); return; }

        var box = document.createElement("div");
        box.setAttribute("role", "dialog");
        box.style.cssText = "position:fixed;right:24px;bottom:72px;z-index:100000;min-width:260px;padding:14px 18px;" +
            "background:#0d1b2a;color:#d6e4f3;border:1px solid rgba(90,160,255,.5);border-radius:10px;" +
            "font:13px/1.5 system-ui,Segoe UI,sans-serif;box-shadow:0 14px 36px rgba(0,0,0,.4);";

        var title = document.createElement("div");
        title.textContent = "Keyboard shortcuts";                       // textContent, never innerHTML
        title.style.cssText = "font-weight:800;color:#7fb6ff;margin-bottom:8px;letter-spacing:.04em;text-transform:uppercase;font-size:12px;";
        box.appendChild(title);

        for (var i = 0; i < SHORTCUTS.length; i++) {
            var row = document.createElement("div");
            var key = document.createElement("code");
            key.textContent = SHORTCUTS[i].keys;
            key.style.cssText = "display:inline-block;min-width:52px;margin-right:10px;padding:1px 7px;border-radius:5px;background:#1c3352;color:#fff;font-weight:700;";
            var what = document.createElement("span");
            what.textContent = SHORTCUTS[i].what;
            row.appendChild(key);
            row.appendChild(what);
            box.appendChild(row);
        }

        document.body.appendChild(box);
        listElement = box;
    };

    ticketOps._hideShortcutList = function () {
        if (!listElement) return false;
        if (listElement.parentNode) listElement.parentNode.removeChild(listElement);
        listElement = null;
        return true;
    };

    ticketOps.shortcuts = SHORTCUTS.slice();

    //# sourceURL=ticketops.interop.js
})(window);
