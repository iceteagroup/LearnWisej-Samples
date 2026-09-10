// ===========================================================================================
// palette.client.js — EnterpriseOps command palette (browser side).
//
// Loaded as a Wisej Widget PACKAGE from the project folder: the static file server serves the
// project directory, so Package { Source = "Interop/palette.client.js" } is fetched as
// /Interop/palette.client.js. No CDN, no build step, no framework.
//
// WHAT THIS FILE IS ALLOWED TO DO
//   • capture Ctrl+K, filter a list while the user types, move the selection, highlight a match
//   • detect what the browser can do (clipboard, camera, storage, time zone, viewport…)
//   • hand a NAMED command id back to its host
//
// WHAT THIS FILE MUST NEVER DO
//   • decide whether a command may run       → the server checks the session's permission
//   • change a work order's state            → the server owns every state transition
//   • believe its own "allowed" flag         → it is a display hint, re-checked server-side
//
// The test from the lesson: "what would break if the browser lied?" Everything here is either a
// keystroke or a pixel, so the answer is "the screen looks wrong", never "the wrong thing ran".
// ===========================================================================================

(function (global) {
    "use strict";

    if (global.EnterpriseOpsPalette) return;      // packages load once per page; stay idempotent

    var CSS_ID = "enterpriseops-palette-css";
    var CSS = [
        ".eop-overlay{position:fixed;inset:0;background:rgba(13,27,42,.34);display:none;z-index:99000;",
        "font-family:-apple-system,Segoe UI,Roboto,Helvetica,Arial,sans-serif;}",
        ".eop-overlay.eop-open{display:block;}",
        ".eop-card{position:absolute;left:50%;top:14%;transform:translateX(-50%);width:620px;max-width:92vw;",
        "background:#fff;border:1px solid #c6d2de;border-radius:10px;box-shadow:0 30px 80px rgba(13,27,42,.35);overflow:hidden;}",
        ".eop-search{display:flex;align-items:center;gap:10px;padding:12px 16px;border-bottom:1px solid #e2e9f1;}",
        ".eop-search input{flex:1;border:0;outline:0;font-size:14.5px;color:#1f2d3a;background:transparent;}",
        ".eop-search input::placeholder{color:#9aa7b4;}",
        ".eop-esc{padding:3px 9px;border-radius:6px;background:#f1f5fa;border:1px solid #d9e1ea;",
        "font:700 11px/1.4 ui-monospace,Consolas,monospace;color:#6a7d92;}",
        ".eop-list{max-height:280px;overflow:auto;}",
        ".eop-row{display:flex;align-items:center;gap:12px;padding:11px 16px;border-bottom:1px solid #f1f5f9;cursor:pointer;}",
        ".eop-row:last-child{border-bottom:0;}",
        ".eop-row.eop-sel{background:#eaf3ff;}",
        ".eop-row .eop-title{font-size:14px;color:#1f2d3a;}",
        ".eop-row.eop-sel .eop-title{font-weight:700;}",
        ".eop-row .eop-keys{margin-left:auto;font:11.5px ui-monospace,Consolas,monospace;color:#8a97a4;}",
        ".eop-row.eop-locked .eop-title{color:#8a97a4;}",
        ".eop-row .eop-lock{font-size:11px;color:#9c4a4a;}",
        ".eop-empty{padding:16px;font-size:13.5px;color:#8a97a4;}",
        ".eop-foot{display:flex;align-items:center;gap:8px;padding:9px 16px;background:#f7fafd;border-top:1px solid #e2e9f1;",
        "font:11.5px ui-monospace,Consolas,monospace;color:#6a7d92;}",
        ".eop-result{margin-left:auto;font-weight:700;}",
        ".eop-result.eop-ok{color:#1f8a4c;}",
        ".eop-result.eop-bad{color:#c0392b;}"
    ].join("");

    function ensureStyles() {
        if (document.getElementById(CSS_ID)) return;
        var style = document.createElement("style");
        style.id = CSS_ID;
        style.textContent = CSS;
        document.head.appendChild(style);
    }

    function el(tag, className, text) {
        var node = document.createElement(tag);
        if (className) node.className = className;
        if (text !== undefined && text !== null) node.textContent = text;
        return node;
    }

    // -----------------------------------------------------------------------------------
    // The palette itself. It owns ONE overlay element appended to document.body and removes
    // it in destroy(): the widget's own DOM subtree is not where a modal belongs, and a node
    // parked on <body> is exactly the kind of leftover the lesson warns about.
    // -----------------------------------------------------------------------------------
    function Palette(options) {
        options = options || {};
        ensureStyles();

        this._placeholder = options.placeholder || "Type a command…";
        this._items = [];
        this._selected = 0;
        this._handlers = {};
        this._target = options.entityId || "";
        this._open = false;

        var overlay = el("div", "eop-overlay");
        var card = el("div", "eop-card");

        var search = el("div", "eop-search");
        search.appendChild(el("span", "eop-mag", "⌕"));
        var input = document.createElement("input");
        input.type = "text";
        input.setAttribute("aria-label", "Command palette");
        input.placeholder = this._placeholder;
        search.appendChild(input);
        search.appendChild(el("span", "eop-esc", "Esc"));

        var list = el("div", "eop-list");
        var foot = el("div", "eop-foot");
        var footTarget = el("span", "eop-target", "");
        var footResult = el("span", "eop-result", "");
        foot.appendChild(footTarget);
        foot.appendChild(footResult);

        card.appendChild(search);
        card.appendChild(list);
        card.appendChild(foot);
        overlay.appendChild(card);
        document.body.appendChild(overlay);

        this.overlay = overlay;
        this.input = input;
        this.list = list;
        this._footTarget = footTarget;
        this._footResult = footResult;

        var me = this;
        this._onInput = function () { me._emit("query", { query: input.value }); };
        this._onKeyDown = function (e) { me._keydown(e); };
        this._onOverlayClick = function (e) { if (e.target === overlay) me.close("backdrop"); };

        input.addEventListener("input", this._onInput);
        input.addEventListener("keydown", this._onKeyDown);
        overlay.addEventListener("mousedown", this._onOverlayClick);

        this._renderTarget();
    }

    Palette.prototype.on = function (name, handler) { this._handlers[name] = handler; };

    Palette.prototype._emit = function (name, data) {
        var handler = this._handlers[name];
        if (typeof handler === "function") handler(data || {});
    };

    Palette.prototype.isOpen = function () { return this._open; };

    Palette.prototype.open = function (via) {
        if (this._open) return;
        this._open = true;
        this.overlay.classList.add("eop-open");
        this.input.value = "";
        this._footResult.textContent = "";
        this._footResult.className = "eop-result";
        this._emit("query", { query: "" });
        var input = this.input;
        setTimeout(function () { try { input.focus(); } catch (ex) { /* focus is best effort */ } }, 0);
        this._emit("opened", { via: via || "hotkey" });
    };

    Palette.prototype.close = function (via) {
        if (!this._open) return;
        this._open = false;
        this.overlay.classList.remove("eop-open");
        this._emit("closed", { via: via || "escape" });
    };

    Palette.prototype.setTarget = function (entityId) {
        this._target = entityId || "";
        this._renderTarget();
    };

    Palette.prototype._renderTarget = function () {
        this._footTarget.textContent = this._target
            ? "target " + this._target + "  ·  ↑↓ select  ↵ run"
            : "no target selected  ·  ↑↓ select  ↵ run";
    };

    /**
     * Rows come from the server (GetCommandCatalog). `allowed` only decides how a row LOOKS;
     * a locked row can still be run, and the server answers PERMISSION_DENIED — that is the
     * failure path the walkthrough shows, and the reason the flag is safe to send at all.
     */
    Palette.prototype.setItems = function (items) {
        this._items = Array.isArray(items) ? items : [];
        this._selected = 0;
        this._render();
    };

    Palette.prototype._render = function () {
        var me = this;
        this.list.innerHTML = "";

        if (this._items.length === 0) {
            this.list.appendChild(el("div", "eop-empty", "No command matches."));
            return;
        }

        this._items.forEach(function (item, index) {
            var row = el("div", "eop-row" + (index === me._selected ? " eop-sel" : "") + (item.allowed ? "" : " eop-locked"));
            row.appendChild(el("span", "eop-title", item.title));
            if (!item.allowed) row.appendChild(el("span", "eop-lock", "needs a higher role"));
            row.appendChild(el("span", "eop-keys", item.shortcut || ""));
            row.addEventListener("mousedown", function (e) {
                e.preventDefault();
                me._selected = index;
                me._render();
                me._run("mouse");
            });
            me.list.appendChild(row);
        });
    };

    Palette.prototype._keydown = function (e) {
        if (e.key === "Escape") { e.preventDefault(); this.close("escape"); return; }
        if (e.key === "ArrowDown") { e.preventDefault(); this._move(1); return; }
        if (e.key === "ArrowUp") { e.preventDefault(); this._move(-1); return; }
        if (e.key === "Enter") { e.preventDefault(); this._run("keyboard"); return; }
    };

    Palette.prototype._move = function (delta) {
        if (this._items.length === 0) return;
        this._selected = (this._selected + delta + this._items.length) % this._items.length;
        this._render();
        var row = this.list.children[this._selected];
        if (row && row.scrollIntoView) row.scrollIntoView({ block: "nearest" });
    };

    Palette.prototype._run = function (via) {
        var item = this._items[this._selected];
        if (!item) return;
        // Hand the NAME to the host. The palette never performs the command itself.
        this._emit("run", { id: item.id, requiresEntity: !!item.requiresEntity, entityId: this._target, via: via });
    };

    /** Shows the server's named answer in the footer. The palette reports; it does not interpret. */
    Palette.prototype.showResult = function (code, message) {
        this._footResult.textContent = code + (message ? " · " + message : "");
        this._footResult.className = "eop-result " + (code === "OK" ? "eop-ok" : "eop-bad");
    };

    /** Every attach above has its detach here. Called from the widget adapter's dispose(). */
    Palette.prototype.destroy = function () {
        try {
            this.input.removeEventListener("input", this._onInput);
            this.input.removeEventListener("keydown", this._onKeyDown);
            this.overlay.removeEventListener("mousedown", this._onOverlayClick);
            if (this.overlay.parentNode) this.overlay.parentNode.removeChild(this.overlay);
        } finally {
            this._handlers = {};
            this._items = [];
            this.overlay = this.input = this.list = null;
        }
    };

    // -----------------------------------------------------------------------------------
    // Feature detection. Browser-side VALUE only: the result shapes the screen (offer manual
    // entry instead of a camera capture) and is reported to the server as DATA. Never derive a
    // permission from it, and never read the user-agent string to guess a capability.
    // -----------------------------------------------------------------------------------
    function detectCapabilities() {
        var readings = [];

        function add(key, value) { readings.push(key + "=" + value); }
        function flag(key, test) {
            var ok = false;
            try { ok = !!test(); } catch (ex) { ok = false; }
            add(key, ok ? "1" : "0");
        }

        add("viewport", (window.innerWidth || 0) + "x" + (window.innerHeight || 0));
        add("language", ((navigator.language || "").replace(/[^A-Za-z-]/g, "") || "unknown"));

        var zone = "unknown";
        try { zone = (Intl.DateTimeFormat().resolvedOptions().timeZone || "unknown"); } catch (ex) { zone = "unknown"; }
        add("timeZone", zone.replace(/[^A-Za-z0-9_/+-]/g, ""));

        add("online", navigator.onLine === false ? "0" : "1");
        flag("touch", function () { return (navigator.maxTouchPoints || 0) > 0 || "ontouchstart" in window; });
        flag("keyboardShortcuts", function () { return typeof document.addEventListener === "function"; });
        flag("clipboard", function () { return !!(navigator.clipboard && navigator.clipboard.writeText); });
        flag("localStorage", function () {
            var probe = "__eop__";
            window.localStorage.setItem(probe, "1");
            window.localStorage.removeItem(probe);
            return true;
        });
        flag("notifications", function () { return typeof window.Notification === "function"; });
        flag("camera", function () { return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia); });
        flag("barcodeScanner", function () { return typeof window.BarcodeDetector === "function"; });

        return readings.join(";");
    }

    /** 8 lower-case hex characters — the shape the server's contract accepts. */
    function newCorrelationId() {
        var out = "";
        for (var i = 0; i < 8; i++) out += Math.floor(Math.random() * 16).toString(16);
        return out;
    }

    global.EnterpriseOpsPalette = {
        version: "1.0",
        create: function (options) { return new Palette(options); },
        detectCapabilities: detectCapabilities,
        newCorrelationId: newCorrelationId
    };
})(window);
