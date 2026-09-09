// Wisej.NET Widget InitScript for a simple status gauge.
// Runs in the browser. "this" is the client-side widget object.
// Options from C# arrive as: { percent: 0-100, label: "text", status: "ok|warn|error" }

// Called ONCE, after Packages have loaded and the widget element exists.
this.init = function (options) {

    // Keep a reference to the widget: inside DOM callbacks "this" changes.
    var me = this;

    // Build the visual inside the container Wisej.NET reserves for us.
    me.container.innerHTML =
        '<div class="lw-gauge">' +
        '  <div class="lw-gauge-track"><div class="lw-gauge-fill"></div></div>' +
        '  <div class="lw-gauge-label"></div>' +
        '</div>';

    // Cache the elements we will update later.
    me.fillEl = me.container.querySelector('.lw-gauge-fill');
    me.labelEl = me.container.querySelector('.lw-gauge-label');

    // Wire ONE event that supports a real workflow: click -> tell the server.
    me.container.addEventListener('click', function () {
        // Small payload only. Arrives in C# as WidgetEvent with e.Type == "gaugeClick".
        me.fireWidgetEvent('gaugeClick', { percent: me.lastPercent || 0 });
    });

    // First draw with the values C# sent.
    me.render(options);
};

// Called EVERY TIME C# changes Options and calls widStatus.Update().
this.update = function (options, old) {
    // Redraw only if something we show actually changed.
    if (!old || options.percent !== old.percent ||
        options.label !== old.label || options.status !== old.status) {
        this.render(options);
    }
};

// Optional helper: a named function the server can call with widStatus.Call("pulse").
this.pulse = function () {
    var el = this.container.querySelector('.lw-gauge');
    if (!el) return;
    el.classList.remove('lw-gauge-pulse');
    void el.offsetWidth; // restart the CSS animation
    el.classList.add('lw-gauge-pulse');
};

// Our own drawing routine: pure display logic, no business rules.
this.render = function (options) {
    options = options || {};
    var percent = Math.max(0, Math.min(100, Number(options.percent) || 0));
    this.lastPercent = percent;
    this.fillEl.style.width = percent + '%';
    this.labelEl.textContent = (options.label || 'Status') + ' — ' + percent + '%';
    // Status only picks a colour class; C# decided WHICH status applies.
    this.fillEl.className = 'lw-gauge-fill lw-gauge-' + (options.status || 'ok');
};
