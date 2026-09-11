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
    dynamic result = await AwaitClient(this.CallAsync("getRenderedSize"), "getRenderedSize");

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
    try
    {
        RenderedSize size = await this.gauge.GetRenderedSizeAsync();

        // The next statement needs the value: choose the caption layout from the rendered width.
        this.gauge.Caption = size.Width >= 400 ? "Boiler 3 · wide" : "Boiler 3 · compact";
    }
    catch (Exception ex)
    {
        ShowAlarm($"✖ getRenderedSize failed: {ex.Message}", AlarmKind.Error);   // the page keeps working
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

## The EvalAsync alternative

`EvalAsync` is the same mechanism for an arbitrary JavaScript **expression** evaluated with
`this` = the wrapper (an expression, not a statement: `"return …;"` fails). The sample does not wire
it; the equivalent read would be:

```csharp
object width = await this.EvalAsync("this.getRenderedSize().width");
```

## Evidence (what the command trace shows)

```
14:02:19.402  → .NET→JS  await CallAsync("getRenderedSize")
14:02:19.431  ← JS→.NET  result {"width":480,"height":244}
```

The `←` line arrives on a later timestamp than the `→` line: the handler was suspended until the
browser replied. Right after it the gauge caption changes to "Boiler 3 · wide". Compare with a
one-way command, which produces a single line and never pauses the handler:

```
14:02:11.318  → .NET→JS  Call("setValue", 72)
```

## Failure paths handled

| Path | What happens |
|---|---|
| Browser never replies | `ClientReplyTimeout` (5 s) elapses, `TimeoutException`, the banner under the gauge shows `✖ getRenderedSize failed: …`. |
| Result is `null` or `0×0` | `InvalidOperationException` before the value is used; nothing is trusted. |
| Wrong-case read (`result.Width`) | `null`, no error, zeros in the DTO; see `DtoContract.md`. The mapping reads `result.width`. |
