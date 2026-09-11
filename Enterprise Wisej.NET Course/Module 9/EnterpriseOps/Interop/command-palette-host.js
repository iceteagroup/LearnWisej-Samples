// ===========================================================================================
// command-palette-host.js — InitScript of EnterpriseOps.Interop.CommandPaletteHost.
//
// Shipped as an EMBEDDED RESOURCE (see the csproj) and handed to the widget as InitScript;
// the palette library itself arrives as a Package from /Interop/palette.client.js.
// `this` is the wisej.web.Widget wrapper; `this.container` is the DOM element the framework owns.
//
// Framework call order:  packages load → init(options) → "loaded" → _addListener(...) per
// WiredEvents entry → update(options, old) on every first-level Options change → dispose().
//
// LIFECYCLE — the module's review question, answered in code:
//   • nothing is attached at page load; the document keydown handler is attached HERE, inside
//     init(), i.e. after the host widget exists;
//   • paletteReady is fired one tick later (a fireWidgetEvent raised synchronously while the
//     framework is still initialising is dropped), which is the signal the server waits for
//     before flushing the calls it deferred;
//   • dispose() removes the keydown handler and destroys the palette (which removes its overlay
//     from document.body) BEFORE delegating to the framework's own dispose.
//
// THE CONTRACT (docs/InteropContract.md, v1.0)
//   list    this.GetCommandCatalogAsync(query)                         → [{Id,Title,Shortcut,RequiresEntity,Allowed}]
//   run     App.MainPage.RunClientCommandAsync(name, entityId, corr)   → {Succeeded,Code,Message,CorrelationId}
//   events  paletteReady {hotkey,contractVersion} · capabilities {report} · error {phase,message}
// ===========================================================================================

this._handlers = {};

this.init = function (options) {
    var me = this;
    options = options || {};

    this._hotkey = options.hotkey || "Ctrl+K";
    this._contractVersion = options.contractVersion || "1.0";
    this._entityId = options.entityId || "";
    this._busy = false;

    if (!window.EnterpriseOpsPalette) {
        this._reportError("init", "palette.client.js did not load (package /Interop/palette.client.js).");
        return;
    }

    // ---- the resting card the widget shows on the page --------------------------------
    var host = document.createElement("div");
    host.style.cssText =
        "position:absolute;left:0;top:0;right:0;bottom:0;padding:14px 16px;box-sizing:border-box;" +
        "font:13px -apple-system,Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#1f2d3a;" +
        "background:#fff;border:1px solid #d9e1ea;border-radius:8px;overflow:hidden;";
    host.innerHTML =
        '<div style="font:700 12px/1.4 -apple-system,Segoe UI,Roboto,sans-serif;letter-spacing:.04em;' +
        'text-transform:uppercase;color:#4a5a6a;">Command palette host</div>' +
        '<div style="margin-top:8px;font-size:14px;">Press <b class="eop-hint-key"></b> anywhere on this page.</div>' +
        '<div class="eop-hint-last" style="margin-top:10px;font:12px ui-monospace,Consolas,monospace;color:#6a7d92;">' +
        'no command sent yet</div>';
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;
    this._elKey = host.querySelector(".eop-hint-key");
    this._elLast = host.querySelector(".eop-hint-last");
    this._elKey.textContent = this._hotkey;

    // ---- the palette (vendor-style library instance) -----------------------------------
    try {
        this.widget = window.EnterpriseOpsPalette.create({
            placeholder: options.placeholder || "Type a command…",
            entityId: this._entityId
        });
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", ex.message);
        return;
    }

    // open() emits "query" itself: one keystroke, one catalogue call. Opening and closing the
    // palette are browser concerns and do not cross the wire.
    this.widget.on("query", function (e) { me._loadCatalog(e.query); });
    // A run closes the palette when the server said OK; on a failure it stays open, so the
    // failure code stays readable in the footer.
    this.widget.on("run", function (e) { me._send(e.id, e.requiresEntity ? (e.entityId || "") : ""); });

    // ---- the document-level hotkey: attached now, detached in dispose ------------------
    this._onDocumentKeyDown = function (e) { me._hotkeyPressed(e); };
    document.addEventListener("keydown", this._onDocumentKeyDown, true);

    // ---- one tick later: tell the server the widget exists, and report the browser -----
    setTimeout(function () {
        if (!me.widget) return;
        me._fire("paletteReady", { hotkey: me._hotkey, contractVersion: me._contractVersion });
        me.paletteCollect();
    }, 0);

    // ---- wrap (never replace) the framework's dispose -----------------------------------
    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._onDocumentKeyDown) {
                document.removeEventListener("keydown", me._onDocumentKeyDown, true);
                me._onDocumentKeyDown = null;
            }
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

this.update = function (options, old) {
    if (!this.widget) return;
    try {
        if (options.hotkey && options.hotkey !== this._hotkey) {
            this._hotkey = options.hotkey;
            if (this._elKey) this._elKey.textContent = this._hotkey;
        }
        if (options.entityId !== undefined && options.entityId !== this._entityId) {
            this._entityId = options.entityId || "";
            this.widget.setTarget(this._entityId);
        }
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// ---------------------------------------------------------------------------------------
// Hotkey

this._hotkeyPressed = function (e) {
    if (!this.widget) return;
    var parts = String(this._hotkey || "Ctrl+K").toLowerCase().split("+");
    var key = parts[parts.length - 1];
    var wantCtrl = parts.indexOf("ctrl") >= 0;
    var wantShift = parts.indexOf("shift") >= 0;
    var wantAlt = parts.indexOf("alt") >= 0;

    if (String(e.key || "").toLowerCase() !== key) return;
    if (wantCtrl !== !!(e.ctrlKey || e.metaKey)) return;
    if (wantShift !== !!e.shiftKey) return;
    if (wantAlt !== !!e.altKey) return;

    e.preventDefault();
    e.stopPropagation();
    if (this.widget.isOpen()) this.widget.close("hotkey");
    else this.widget.open("hotkey");
};

// ---------------------------------------------------------------------------------------
// (1) List — the widget's own [WebMethod], registered by RegisterWebMethods(config).
//     The server filters for DISPLAY; "Allowed" is a hint, never an authorisation.

this._loadCatalog = function (query) {
    var me = this;
    if (!this.widget) return;

    var call = this.GetCommandCatalogAsync;
    if (typeof call !== "function") {
        // Registration failed: show an honest empty palette instead of a broken one.
        this.widget.setItems([]);
        this._reportError("catalog", "GetCommandCatalog is not registered on this widget.");
        return;
    }

    call.call(this, String(query || "")).then(function (rows) {
        if (!me.widget) return;
        // WebMethod return values are NOT camel-cased; tolerate either shape.
        var items = (rows || []).map(function (r) {
            return {
                id: r.Id !== undefined ? r.Id : r.id,
                title: r.Title !== undefined ? r.Title : r.title,
                shortcut: r.Shortcut !== undefined ? r.Shortcut : r.shortcut,
                requiresEntity: r.RequiresEntity !== undefined ? r.RequiresEntity : r.requiresEntity,
                allowed: (r.Allowed !== undefined ? r.Allowed : r.allowed) !== false
            };
        });
        me.widget.setItems(items);
    }).catch(function (ex) {
        me._reportError("catalog", ex && ex.message ? ex.message : String(ex));
    });
};

// ---------------------------------------------------------------------------------------
// (2) Run — the ONE remote method the palette may call. Three named fields, nothing else.

this._send = function (commandName, entityId) {
    var me = this;
    if (this._busy) return;

    var target = (window.App && window.App.MainPage) || null;
    if (!target || typeof target.RunClientCommandAsync !== "function") {
        this._reportError("run", "App.MainPage.RunClientCommand is not available.");
        return;
    }

    // Never pass null to a WebMethod: the client wrapper calls getId on every argument.
    var name = String(commandName === null || commandName === undefined ? "" : commandName);
    var entity = String(entityId === null || entityId === undefined ? "" : entityId);
    var corr = window.EnterpriseOpsPalette.newCorrelationId();

    this._busy = true;
    this._setLast("→ " + (name || "(empty)") + " " + (entity || "—") + " corr " + corr);

    target.RunClientCommandAsync(name, entity, corr).then(function (result) {
        me._busy = false;

        // A null result means the server threw: treat it as a failure, never as a success.
        var code = "SERVER_ERROR", message = "The server did not answer.";
        if (result) {
            code = result.Code !== undefined ? result.Code : (result.code !== undefined ? result.code : code);
            message = result.Message !== undefined ? result.Message : (result.message !== undefined ? result.message : message);
        }

        if (me.widget) {
            me.widget.showResult(code, message);
            if (code === "OK") me.widget.close("ran");
        }
        me._setLast("← " + code + " · " + message);
    }).catch(function (ex) {
        me._busy = false;
        me._reportError("run", ex && ex.message ? ex.message : String(ex));
    });
};

// ---------------------------------------------------------------------------------------
// (3) Capabilities — detection here, decisions on the server.

this.paletteCollect = function () {
    if (!window.EnterpriseOpsPalette) return;
    var report = window.EnterpriseOpsPalette.detectCapabilities();
    this._fire("capabilities", { report: report });
};

// ---------------------------------------------------------------------------------------
// Functions the server reaches with Control.Call("name", args…).

this.paletteOpen = function () { if (this.widget) this.widget.open("server"); };
this.paletteClose = function () { if (this.widget) this.widget.close("server"); };

this.paletteShowResult = function (code, message) {
    if (this.widget) this.widget.showResult(String(code || ""), String(message || ""));
    this._setLast("← " + code + " · " + message);
};

this.paletteSetTarget = function (entityId) {
    this._entityId = String(entityId || "");
    if (this.widget) this.widget.setTarget(this._entityId);
};

// ---------------------------------------------------------------------------------------
// Events out. The framework hands one handler per WiredEvents entry to _addListener; calling
// that handler is what actually crosses the wire (a synchronous fireWidgetEvent during
// init/update is dropped, so the handler — or a deferred fire — is the reliable route).

this._addListener = function (name, handler) { this._handlers[name] = handler; };
this._removeListener = function (name, handler) { if (this._handlers[name] === handler) this._handlers[name] = null; };
this._getEventData = function (type, e) { return e || {}; };

this._fire = function (name, data) {
    var me = this, handler = this._handlers[name];
    if (typeof handler === "function") { handler(data || {}); return; }
    setTimeout(function () { try { me.fireWidgetEvent(name, data || {}); } catch (ex) { /* disposed */ } }, 0);
};

this._reportError = function (phase, message) {
    this._setLast("! " + phase + " · " + message);
    this._fire("error", { phase: String(phase || ""), message: String(message || "") });
};

this._setLast = function (text) {
    if (this._elLast) this._elLast.textContent = text;
};

//# sourceURL=enterpriseops.interop.CommandPaletteHost.js
