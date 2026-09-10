/* ============================================================================================
   rating.js — a small, self-contained star-rating library.
   OperationsConsole · Mastering the Control Library · Module 7

   This file plays the part of the "third-party JavaScript library" in the module reading. It
   knows nothing about Wisej.NET: no fireWidgetEvent, no this.container, no server. It renders,
   it listens to clicks and keys, and it raises its own "change" event. That separation is the
   whole point of the Widget control — rating-init.js is the adapter that joins the two worlds.

   It also contains NO business rule. "A rating is 1..5" is enforced on the server, in
   RatingService.TryNormalize / RatingService.Save, because a browser payload can be edited by
   anyone with developer tools. The clamp below is rendering hygiene, not a rule.

   And it contains NO colours: every colour and size comes from rating.css through CSS custom
   properties, so Application.LoadTheme and the StyleSheet extender can still reach the widget.

   ------------------------------------------------------------------------------------------
   Public API (this is the "library" half of docs/WidgetContract.md)

     var r = new RatingWidget(hostElement, { value: 3, max: 5, label: "Northwind Traders",
                                             saved: false, theme: "bootstrap", narrow: false });
     r.setValue(4)            set the value and raise "change"
     r.setValue(4, true)      set the value silently (used when the SERVER pushes a value, so a
                              server-driven change never bounces back as a client event)
     r.setSaved(4)            show the confirmed/saved state for that value
     r.clearSaved()           back to "edited, not saved"
     r.setOptions({...})      max / label / theme / narrow / saved / value
     r.getState()             { value, savedValue, saved, max, label, theme, narrow }
     r.on("change", fn)       fn({ value: n, previous: n })
     r.off("change", fn)
     r.destroy()              unhook everything and empty the host element

   Events: "change" only. Fired for a real user gesture (click, Enter/Space, arrow keys).
   ============================================================================================ */

(function (global) {
    "use strict";

    var STAR_PATH = "M12 2.6l2.95 5.98 6.6.96-4.78 4.66 1.13 6.58L12 17.68 6.1 20.78l1.13-6.58L2.45 9.54l6.6-.96z";

    function clampInt(value, min, max) {
        var n = parseInt(value, 10);
        if (isNaN(n)) return min;
        if (n < min) return min;
        if (n > max) return max;
        return n;
    }

    function RatingWidget(host, options) {
        if (!host) throw new Error("RatingWidget: a host element is required.");
        options = options || {};

        this.host = host;
        this.max = options.max == null ? 5 : clampInt(options.max, 1, 10);
        this.value = clampInt(options.value, 0, this.max);
        this.savedValue = options.saved ? this.value : 0;
        this.label = options.label == null ? "" : String(options.label);
        this.theme = options.theme || "bootstrap";
        this.narrow = !!options.narrow;

        this._handlers = { change: [] };
        this._stars = [];
        this._destroyed = false;

        this._build();
        this._render();
    }

    // ---- construction ----------------------------------------------------------------------

    RatingWidget.prototype._build = function () {
        var me = this;

        // Everything is created here; the host element itself is never styled inline, so a
        // stylesheet (rating.css) or the Wisej StyleSheet extender stays in control.
        this.host.innerHTML = "";

        this.root = document.createElement("div");
        this.root.className = "opc-rating";
        this.root.setAttribute("role", "radiogroup");

        this.starsEl = document.createElement("div");
        this.starsEl.className = "opc-rating__stars";
        this.root.appendChild(this.starsEl);

        for (var i = 1; i <= this.max; i++) {
            var star = document.createElement("button");
            star.type = "button";
            star.className = "opc-rating__star";
            star.setAttribute("data-value", String(i));
            star.setAttribute("role", "radio");
            star.setAttribute("aria-label", i + " of " + this.max);
            star.innerHTML = "<svg viewBox='0 0 24 24' aria-hidden='true'><path d='" + STAR_PATH + "'/></svg>";
            this.starsEl.appendChild(star);
            this._stars.push(star);
        }

        this.captionEl = document.createElement("div");
        this.captionEl.className = "opc-rating__caption";
        this.captionEl.innerHTML =
            "<span class='opc-rating__label'></span>" +
            "<span class='opc-rating__value'></span>" +
            "<span class='opc-rating__badge'>✓ Saved</span>";
        this.root.appendChild(this.captionEl);

        this.labelEl = this.captionEl.querySelector(".opc-rating__label");
        this.valueEl = this.captionEl.querySelector(".opc-rating__value");
        this.badgeEl = this.captionEl.querySelector(".opc-rating__badge");

        // One delegated listener per gesture: cheap, and destroy() has three things to unhook.
        this._onClick = function (e) {
            var star = e.target.closest ? e.target.closest(".opc-rating__star") : null;
            if (!star) return;
            e.preventDefault();
            me.setValue(parseInt(star.getAttribute("data-value"), 10));
        };
        this._onKeyDown = function (e) {
            if (e.key === "ArrowRight" || e.key === "ArrowUp") { e.preventDefault(); me.setValue(Math.min(me.max, me.value + 1)); }
            else if (e.key === "ArrowLeft" || e.key === "ArrowDown") { e.preventDefault(); me.setValue(Math.max(1, me.value - 1)); }
        };
        this._onOver = function (e) {
            var star = e.target.closest ? e.target.closest(".opc-rating__star") : null;
            me._preview(star ? parseInt(star.getAttribute("data-value"), 10) : 0);
        };
        this._onOut = function () { me._preview(0); };

        this.starsEl.addEventListener("click", this._onClick);
        this.starsEl.addEventListener("keydown", this._onKeyDown);
        this.starsEl.addEventListener("mouseover", this._onOver);
        this.starsEl.addEventListener("mouseleave", this._onOut);

        this.host.appendChild(this.root);
    };

    // ---- rendering (class names only — never a colour) --------------------------------------

    RatingWidget.prototype._render = function () {
        var i, star;

        for (i = 0; i < this._stars.length; i++) {
            star = this._stars[i];
            var on = (i + 1) <= this.value;
            star.className = "opc-rating__star" + (on ? " is-on" : "");
            star.setAttribute("aria-checked", (i + 1) === this.value ? "true" : "false");
            star.setAttribute("tabindex", (i + 1) === Math.max(1, this.value) ? "0" : "-1");
        }

        var saved = this.savedValue > 0 && this.savedValue === this.value;
        this.root.className = "opc-rating opc-rating--" + this.theme +
            (this.narrow ? " is-narrow" : "") +
            (saved ? " saved" : "") +
            (this._busy ? " is-busy" : "");

        this.root.setAttribute("aria-label", (this.label || "Rating") + ", " + this.value + " of " + this.max);
        this.labelEl.textContent = this.label;
        this.valueEl.textContent = this.value > 0 ? (this.value + " / " + this.max) : "not rated";
        this.badgeEl.textContent = "✓ Saved " + this.savedValue + "/" + this.max;
    };

    RatingWidget.prototype._preview = function (upTo) {
        for (var i = 0; i < this._stars.length; i++) {
            var on = upTo > 0 && (i + 1) <= upTo;
            this._stars[i].classList.toggle("is-preview", on);
        }
    };

    // ---- public API -------------------------------------------------------------------------

    RatingWidget.prototype.setValue = function (value, silent) {
        var next = clampInt(value, 0, this.max);
        var previous = this.value;
        this.value = next;

        // A user gesture always drops the saved marker until the server confirms again — and it
        // always raises "change", even when the value did not move. That is what lets the user
        // retry the SAME rating after a rejected payload or a failed save.
        if (!silent) this.savedValue = 0;
        else if (this.savedValue !== next) this.savedValue = 0;

        this._render();

        if (!silent && next > 0) this._emit("change", { value: next, previous: previous });
        return this;
    };

    /** Called (through the adapter) by the SERVER after RatingService accepted and stored the value. */
    RatingWidget.prototype.setSaved = function (value) {
        this.savedValue = clampInt(value, 0, this.max);
        if (this.savedValue > 0) this.value = this.savedValue;
        this._busy = false;
        this._render();
        return this;
    };

    RatingWidget.prototype.clearSaved = function () {
        this.savedValue = 0;
        this._busy = false;
        this._render();
        return this;
    };

    RatingWidget.prototype.setBusy = function (busy) {
        this._busy = !!busy;
        this._render();
        return this;
    };

    RatingWidget.prototype.setOptions = function (options) {
        options = options || {};
        if (options.label !== undefined) this.label = options.label == null ? "" : String(options.label);
        if (options.theme !== undefined && options.theme) this.theme = String(options.theme);
        if (options.narrow !== undefined) this.narrow = !!options.narrow;
        if (options.saved !== undefined && !options.saved) this.savedValue = 0;
        if (options.value !== undefined) this.setValue(options.value, true);
        if (options.saved !== undefined && options.saved) this.savedValue = this.value;
        this._render();
        return this;
    };

    RatingWidget.prototype.getState = function () {
        return {
            value: this.value,
            savedValue: this.savedValue,
            saved: this.savedValue > 0 && this.savedValue === this.value,
            max: this.max,
            label: this.label,
            theme: this.theme,
            narrow: this.narrow
        };
    };

    RatingWidget.prototype.on = function (name, fn) {
        if (this._handlers[name] && typeof fn === "function") this._handlers[name].push(fn);
        return this;
    };

    RatingWidget.prototype.off = function (name, fn) {
        var list = this._handlers[name];
        if (!list) return this;
        if (!fn) { this._handlers[name] = []; return this; }
        var i = list.indexOf(fn);
        if (i >= 0) list.splice(i, 1);
        return this;
    };

    RatingWidget.prototype._emit = function (name, payload) {
        var list = (this._handlers[name] || []).slice();
        for (var i = 0; i < list.length; i++) {
            try { list[i](payload); }
            catch (ex) { if (global.console && console.error) console.error("RatingWidget: a '" + name + "' handler threw.", ex); }
        }
    };

    RatingWidget.prototype.destroy = function () {
        if (this._destroyed) return;
        this._destroyed = true;
        if (this.starsEl) {
            this.starsEl.removeEventListener("click", this._onClick);
            this.starsEl.removeEventListener("keydown", this._onKeyDown);
            this.starsEl.removeEventListener("mouseover", this._onOver);
            this.starsEl.removeEventListener("mouseleave", this._onOut);
        }
        this._handlers = { change: [] };
        this._stars = [];
        if (this.host) this.host.innerHTML = "";
        this.root = this.starsEl = this.captionEl = this.labelEl = this.valueEl = this.badgeEl = null;
    };

    global.RatingWidget = RatingWidget;

})(window);

//# sourceURL=rating.js
