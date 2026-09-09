# One `CallAsync` example, awaited · `GetRenderedSizeAsync`

Deliverable 2 of the Module 6 lab: a server handler that genuinely needs a client value before its
next statement can run, and the evidence that the round trip happened.

## Why this one is awaited

The gauge is laid out by the browser. Only the browser knows the pixel box it ended up with, and
the server wants to pick a layout ("wide" vs "compact") and update the caption accordingly. The next
statement cannot run without the number, so waiting is justified. Logging the size would not be.

## Server side

```csharp
// Controls/SimpleGauge.cs
public async Task<RenderedSize> GetRenderedSizeAsync()
{
    RaiseTrace(TraceDirection.ServerToClient, "CallAsync(\"getRenderedSize\")", "awaiting the browser…");
    var watch = Stopwatch.StartNew();

    dynamic result = await AwaitClient(this.CallAsync("getRenderedSize"), "getRenderedSize");

    watch.Stop();
    RaiseTrace(TraceDirection.ClientToServer, "result", $"{ToWireJson((object)result)}  ({watch.ElapsedMilliseconds} ms round trip)");

    if (result == null)
        throw new InvalidOperationException("getRenderedSize returned null.");

    var size = new RenderedSize                 // read by the JavaScript names
    {
        Width = ToInt(result.width),
        Height = ToInt(result.height)
    };

    if (size.Width <= 0 || size.Height <= 0)    // validate before trusting
        throw new InvalidOperationException($"getRenderedSize reported {size}: the gauge is not laid out.");

    return size;
}

// bound the wait: a closed tab never replies
private async Task<object> AwaitClient(Task<object> call, string what)
{
    var completed = await Task.WhenAny(call, Task.Delay(this.ClientReplyTimeout));
    if (completed != call)
        throw new TimeoutException($"The browser did not answer {what} within {ClientReplyTimeout.TotalSeconds:0.#} s.");
    return await call;
}
```

```csharp
// Window1.cs — an async void event handler owns its try/catch
private async void buttonReadSize_Click(object sender, EventArgs e)
{
    BeginAwait();
    try
    {
        RenderedSize size = await this.gauge.GetRenderedSizeAsync();

        string layout = size.Width >= 400 ? "wide" : "compact";      // the next statement needs the value
        this.gauge.Caption = $"Boiler 3 · {layout}";
        ShowResult("RenderedSize ← await CallAsync(\"getRenderedSize\")", SimpleGauge.ToWireJson(size));
        EndAwait();
    }
    catch (Exception ex)
    {
        FailAwait("getRenderedSize", ex);       // trace line + banner; the page keeps working
    }
}
```

## Client side

```js
// wwwroot/gauge-init.js — this = the wrapper; returns a small plain object, never a DOM node
this.getRenderedSize = function () {
    var r = this.host ? this.host.getBoundingClientRect() : { width: 0, height: 0 };
    return { width: Math.round(r.width), height: Math.round(r.height) };
};
```

## The EvalAsync variant

Same mechanism, arbitrary expression, primitive back:

```csharp
object result = await this.EvalAsync("this.measureWidth()");
double width = Convert.ToDouble(result, CultureInfo.InvariantCulture);
```

## Evidence (what the trace shows)

```
14:02:19.402  → .NET→JS  CallAsync("getRenderedSize")        awaiting the browser…
14:02:19.431  ← JS→.NET  result                              {"width":480,"height":244}  (29 ms round trip)
14:02:19.431  → .NET→JS  update(options)                     {"label":"Boiler 3 · wide"}
14:02:19.431  • server   next statement                      RenderedSize 480×244 → "wide" layout chosen, Caption updated
```

Compare with a one-way command, where all lines share one timestamp and the handler never paused:

```
14:02:11.318  → .NET→JS  Call("setValue", 72)                one-way · queued · no result
14:02:11.318  • server   next statement                      runs immediately — nothing was awaited
```

While the handler is suspended the status label reads `● awaiting the browser… (handler suspended)`.
The result box under the gauge shows the mapped DTO as wire JSON: `{"width":480,"height":244}`.

## Failure paths exercised

| Path | What happens |
|---|---|
| Browser never replies | `ClientReplyTimeout` (5 s) elapses, `TimeoutException`, trace line `timeout getRenderedSize: no reply within 5 s`, red banner, status `fault`. Reproduce by closing the tab mid-call only if the session survives; otherwise read the code path. |
| Result is `null` or `0×0` | `InvalidOperationException` before the value is used; nothing is trusted. |
| Wrong-case read (`result.Width`) | see the "Camel-case pitfall" button and `DtoContract.md`: `null`, no error, zeros in the DTO. |
