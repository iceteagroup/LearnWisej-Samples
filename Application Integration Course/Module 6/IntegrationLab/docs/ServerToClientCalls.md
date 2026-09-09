# Server-to-client method calls · `IntegrationLab.Controls.SimpleGauge`

Deliverable 1 of the Module 6 lab. The gauge exposes four commands the server invokes on the
client wrapper. They differ in exactly one thing: **whether the server waits**.

| C# method | Wisej.NET call | Client function (gauge-init.js) | Waits? | Result |
|---|---|---|---|---|
| `SetValue(double)` | `Call("setValue", v)` | `this.setValue(v)` | no — queued | none |
| `ResetAnimation()` | `Call("resetAnimation")` | `this.resetAnimation()` | no — queued | none |
| `GetRenderedSizeAsync()` | `await CallAsync("getRenderedSize")` | `this.getRenderedSize()` | yes | `{width,height}` → `RenderedSize` |
| `GetWidthViaEvalAsync()` | `await EvalAsync("this.measureWidth()")` | `this.measureWidth()` | yes | number → `double` |
| `GetSelectedStateAsync()` | `await CallAsync("getSelectedState")` | `this.getSelectedState()` | yes | `{value,isAboveThreshold,width,height,isAnimating}` → `GaugeStateDto` |

The names are constants on `SimpleGauge` (`JsSetValue`, `JsResetAnimation`, `JsGetRenderedSize`,
`JsGetSelectedState`, `JsGetWidthExpression`) so the two sides can be kept in sync from one place.

## Call — fire and continue

```csharp
public void SetValue(double value)
{
    ValidateValue(value);                 // server validates BEFORE anything crosses the wire
    if (_value != value)
    {
        _value = value;
        ((dynamic)this.Options).value = value;   // state: {"value":72} rendered with this response
    }
    this.Call("setValue", value);         // command: queued, flushed with the same response, no result
}

public void ResetAnimation() => this.Call("resetAnimation");   // no result needed
```

`Call` names a client function, passes serialized arguments, and returns immediately. The
invocation is **queued** and sent with the response of the request that is being processed (a
button click here). The client runs it as `wrapper.setValue(72)` with `this` = the wrapper.

Use it for every command the server does not need an answer to. The trace shows why it is cheap:

```
14:02:11.318  → .NET→JS  update(options)                     {"value":72}
14:02:11.318  → .NET→JS  Call("setValue", 72)                one-way · queued · no result
14:02:11.318  • server   next statement                      runs immediately — nothing was awaited
```

Three lines, one timestamp: the handler never stopped.

### Why `SetValue` writes `Options.value` *and* issues `Call("setValue")`

They do different jobs. `Options` is **state**: durable, diffed by the framework, re-rendered by
`init(options)` on a page refresh or for a late-joining client, and what the server validates and
audits. `Call` is a **command**: transient, executed once by the widget that exists right now, never
replayed. The gauge value is state, but *changing* it is a command the operator wants to see as a
needle sweep. The client wrapper dedupes the two by target value (`update()` skips a value that a
`setValue` command already targeted, and vice versa), so it does not matter which one the browser
applies first.

### `Call("setValue")` vs `Instance.setValue()`

`this.Call("setValue", v)` runs the **wrapper** function in gauge-init.js (which animates and keeps
its bookkeeping). `this.Instance.setValue(v)` would be translated into `this.widget.setValue(v)` —
the **vendor** method — bypassing the wrapper. Both are legal; the wrapper functions are the
documented contract, `Instance.*` is a shortcut around it.

## CallAsync / EvalAsync — when the workflow needs the answer

```csharp
public async Task<RenderedSize> GetRenderedSizeAsync()
{
    dynamic result = await this.CallAsync("getRenderedSize");     // handler suspends here
    return new RenderedSize { Width = ToInt(result.width), Height = ToInt(result.height) };
}

public async Task<double> GetWidthViaEvalAsync()
{
    object result = await this.EvalAsync("this.measureWidth()");
    return Convert.ToDouble(result, CultureInfo.InvariantCulture);
}
```

`await` suspends the server-side handler until the browser round trip completes: the response goes
out with the pending call, the browser evaluates it, posts the value back, and the handler resumes on
the Wisej.NET synchronization context (so it can keep touching controls). The trace makes the pause
visible: the `→` line and the `←` line carry different timestamps and the round trip is measured.

```
14:02:19.402  → .NET→JS  CallAsync("getRenderedSize")        awaiting the browser…
14:02:19.431  ← JS→.NET  result                              {"width":480,"height":244}  (29 ms round trip)
14:02:19.431  • server   next statement                      RenderedSize 480×244 → "wide" layout chosen, Caption updated
```

See `CallAsyncExample.md` for the full awaited example.

## Queueing and batching order

- Every `Call` issued during one request is queued in order and shipped **in one response**. The
  stream button demonstrates it: each timer tick is one request with one `update(options)` and one
  `Call("setValue")`.
- An `Options` change and a `Call` issued in the same handler travel in the same response. The lab
  does not depend on which the client applies first (see the dedupe note above); a wrapper that did
  would be fragile.
- `await CallAsync(...)` ends the current batch: everything queued so far (including earlier `Call`s)
  is flushed so the browser can answer. Statements after the `await` start a new batch.
- Nothing is sent before the widget is created. `Call` before `IsLoaded` targets a wrapper that does
  not exist yet; the state path (`Options`) is what makes the first render right. That is the second
  reason `SetValue` writes the state and not only the command.

## The lab's rule and the three review questions

**Use `Call` by default; reach for `CallAsync`/`EvalAsync` only when the next server statement
cannot run without the answer.** For every awaited call in a review:

| Question | Answer in this lab |
|---|---|
| Is the client result used in the same handler? If not, use `Call`. | `Read rendered size`: yes — the next statement picks a "wide"/"compact" layout and updates `Caption`. `Get selected state`: yes — the DTO is mapped, validated and shown. `Set value` / `Reset animation`: no — so they are `Call`. |
| What happens if the browser never replies (tab closed)? | The `Task` never completes and the handler stays suspended. `SimpleGauge` bounds every awaited call with `ClientReplyTimeout` (5 s, `Task.WhenAny` + `Task.Delay`) and turns "stuck forever" into a `TimeoutException` that the handler reports in the trace and the banner. |
| Is the awaited value validated before it is trusted? | Yes. `RenderedSize` must be positive (a hidden element reports 0×0); `GaugeStateDto.Value` must be inside `Minimum..Maximum` or the state is rejected; a client value that differs from the server value is logged as "server wins". |

## Evidence

| Button | Trace evidence |
|---|---|
| Set value (72) / (104 · alarm) | `update(options) {"value":…}` + `Call("setValue", …)` + `next statement runs immediately` on the same timestamp; the needle sweeps; at 104 `← thresholdExceeded` arrives on a later request and `ThresholdExceeded` fires in C# |
| Reset animation | one `Call("resetAnimation")` line, `next statement runs immediately`; the gauge plays its settle animation |
| Read rendered size | `CallAsync("getRenderedSize")` → `← result {"width":…,"height":…} (n ms round trip)` → `next statement … layout chosen`; the caption changes to "Boiler 3 · wide" |
| Eval width | `EvalAsync("this.measureWidth()")` → `← result 480 (n ms round trip)` |
| Get selected state | `CallAsync("getSelectedState")` → `← result {…}` → the DTO as camelCase JSON in the result box; during a stream `isAnimating` is `true` and a `contract check` line shows the mid-sweep value vs the server value |
| ▶ Stream values | one request per tick, each with a queued `Call`; the status counts ticks; nothing is awaited |
