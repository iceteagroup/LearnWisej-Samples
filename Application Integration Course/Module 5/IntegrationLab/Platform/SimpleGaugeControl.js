///////////////////////////////////////////////////////////////////////////////
//
// integrationlab.controls.SimpleGaugeControl — client half of the custom control.
//
// Lives under /Platform and is embedded in IntegrationLab.dll; Wisej.NET bundles it
// into the client because the assembly carries [assembly: WisejResources]. It is
// paired with the server class IntegrationLab.Controls.SimpleGaugeControl, which
// renders config.className = "integrationlab.controls.SimpleGaugeControl".
//
// Rules this class follows:
//   1. It translates between the framework and the vendor library (VendorGauge) and
//      nothing else: no application logic, so every server control that renders to
//      it can reuse it.
//   2. Properties mirror what the server writes in OnWebRender (camel-cased); each
//      apply method forwards the change to the vendor object.
//   3. The vendor lives in a child element of the content element, created on the
//      first "appear" (the DOM exists only then) and destroyed in destruct.
//   4. Colours come from the theme: the "simplegauge" appearance sets textColor
//      (needle + readout) and the mixin defines simplegauge-accent / simplegauge-track.
//   5. Works in the Designer: no server round trip is needed to draw, placeholder
//      data is fine, and repeated appear/resize is handled.
//
///////////////////////////////////////////////////////////////////////////////

qx.Class.define("integrationlab.controls.SimpleGaugeControl", {

	extend: wisej.web.Control,

	construct: function () {

		this.base(arguments);

		// the DOM element exists only after the widget appears.
		this.addListener("appear", this._createVendor, this);
		this.addListener("resize", this._onResize, this);

		// follow the theme: appearance textColor and theme switches.
		this.addListener("changeTextColor", this._applyThemeColors, this);
		this.__themeListenerId = qx.theme.manager.Meta.getInstance().addListener("changeTheme", this._onChangeTheme, this);
	},

	properties: {

		// theme appearance key (the server also writes config.appearance = "simplegauge").
		appearance: { init: "simplegauge", refine: true },

		/** Current reading. */
		value: { init: 0, check: "Number", apply: "_applyValue" },

		/** Scale. */
		minimum: { init: 0, check: "Number", apply: "_applyRange" },
		maximum: { init: 100, check: "Number", apply: "_applyRange" },

		/** Rising-edge alarm level. */
		threshold: { init: 90, check: "Number", apply: "_applyRange" },

		/** Text drawn by the gauge. */
		caption: { init: "", check: "String", nullable: true, apply: "_applyCaption" },
		units: { init: "", check: "String", nullable: true, apply: "_applyCaption" }
	},

	members: {

		// the vendor instance and the child element it draws into.
		__gauge: null,
		__host: null,
		__themeListenerId: null,

		// ---------------------------------------------------------------------
		// lifecycle
		// ---------------------------------------------------------------------

		/**
		 * Creates the vendor gauge inside the content element. Called on "appear";
		 * guarded so repeated appear events (hide/show, Designer re-layout) only resize.
		 */
		_createVendor: function () {

			if (this.__gauge) {
				this._onResize();
				return;
			}

			var el = this.getContentElement().getDomElement();
			if (!el)
				return;

			var host = document.createElement("div");
			host.className = "simplegauge-host";
			this.__host = host;
			this.__layoutHost();
			el.appendChild(host);

			try {
				this.__gauge = new VendorGauge(host, this.__vendorOptions());
			}
			catch (ex) {
				this.__gauge = null;
				this.__logError(ex);
				return;
			}

			// vendor event → Wisej event. The server wires "thresholdExceeded(Data)" so the
			// data map travels to OnWebEvent as e.Parameters.Data. Deferred one tick because
			// the vendor raises it synchronously inside setValue(), i.e. while a server
			// update may still be applied.
			var me = this;
			this.__gauge.on("thresholdexceeded", function (e) {
				setTimeout(function () {
					if (!me.isDisposed())
						me.fireDataEvent("thresholdExceeded", { value: e.value, threshold: e.threshold });
				}, 0);
			});
		},

		_onResize: function () {

			this.__layoutHost();
			if (this.__gauge)
				this.__gauge.resize();
		},

		// keeps the host inside the appearance padding.
		__layoutHost: function () {

			var host = this.__host;
			if (!host)
				return;

			host.style.cssText =
				"position:absolute;" +
				"left:" + (this.getPaddingLeft() | 0) + "px;" +
				"top:" + (this.getPaddingTop() | 0) + "px;" +
				"right:" + (this.getPaddingRight() | 0) + "px;" +
				"bottom:" + (this.getPaddingBottom() | 0) + "px;";
		},

		// ---------------------------------------------------------------------
		// property → vendor
		// ---------------------------------------------------------------------

		_applyValue: function (value) {

			if (!this.__gauge)
				return;

			try {
				this.__gauge.setValue(value);
			}
			catch (ex) {
				this.__logError(ex);
			}
		},

		_applyRange: function () {

			if (!this.__gauge)
				return;

			try {
				var o = this.__vendorOptions();
				this.__gauge.setOptions({ min: o.min, max: o.max, warnAt: o.warnAt, threshold: o.threshold });
			}
			catch (ex) {
				this.__logError(ex);
			}
		},

		_applyCaption: function () {

			if (!this.__gauge)
				return;

			try {
				this.__gauge.setOptions({ label: this.getCaption() || "", units: this.getUnits() || "" });
			}
			catch (ex) {
				this.__logError(ex);
			}
		},

		// everything the vendor needs, derived from the declared properties.
		__vendorOptions: function () {

			var min = this.getMinimum(), max = this.getMaximum(), threshold = this.getThreshold();
			return {
				value: this.getValue(),
				min: min,
				max: max,
				threshold: threshold,
				warnAt: threshold - (max - min) * 0.15,     // the "warm" band starts 15% of the scale before the alarm
				label: this.getCaption() || "",
				units: this.getUnits() || "",
				colors: this._readThemeColors()
			};
		},

		// ---------------------------------------------------------------------
		// theme
		// ---------------------------------------------------------------------

		/**
		 * Reads the colours the gauge should use from the active theme.
		 *   needle/text  ← the appearance's textColor ("simplegauge" → windowText)
		 *   accent       ← theme colour "simplegauge-accent" (mixin) or "primary"
		 *   track        ← theme colour "simplegauge-track"  (mixin) or "windowFrame"
		 */
		_readThemeColors: function () {

			var mgr = qx.theme.manager.Color.getInstance();

			// resolves a theme colour name (one alias level allowed) or a CSS colour; null otherwise.
			var resolve = function (name) {
				if (!name)
					return null;
				var v = mgr.resolve(name);
				if (v === name)
					return qx.util.ColorUtil.isValidPropertyValue(name) ? name : null;
				if (mgr.isDynamic(v))
					v = mgr.resolve(v);
				return v;
			};

			var text = resolve(this.getTextColor());
			return {
				accent: resolve("simplegauge-accent") || resolve("primary") || "#1a86ff",
				needle: text || resolve("simplegauge-needle") || resolve("windowText") || "#16314e",
				text: text || resolve("windowText") || "#0d1b2a",
				track: resolve("simplegauge-track") || resolve("windowFrame") || "#e6ebf1",
				label: resolve("text-placeholder") || "#8a98a8"
			};
		},

		_applyThemeColors: function () {

			if (!this.__gauge)
				return;

			try {
				this.__gauge.setOptions({ colors: this._readThemeColors() });
			}
			catch (ex) {
				this.__logError(ex);
			}
		},

		_onChangeTheme: function () {

			// apply now and once more after the theme manager has finished switching.
			this._applyThemeColors();
			qx.event.Timer.once(this._applyThemeColors, this, 100);
		},

		__logError: function (ex) {

			if (this.core && this.core.logError)
				this.core.logError(ex);
			else if (window.console)
				console.error("SimpleGaugeControl: " + (ex && ex.message ? ex.message : ex));
		}
	},

	destruct: function () {

		if (this.__themeListenerId) {
			try { qx.theme.manager.Meta.getInstance().removeListenerById(this.__themeListenerId); } catch (ex) { }
			this.__themeListenerId = null;
		}

		if (this.__gauge) {
			try { this.__gauge.off(); this.__gauge.destroy(); } catch (ex) { }
			this.__gauge = null;
		}

		if (this.__host && this.__host.parentNode)
			this.__host.parentNode.removeChild(this.__host);
		this.__host = null;
	}
});
