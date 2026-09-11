# Browser capability panel

**Detection shapes the offer. It never grants a permission.**

`Interop/palette.client.js` → `detectCapabilities()` runs in the browser.
`Services/BrowserCapabilityService.cs` decides what to keep and which fallback to use.
`UI/BrowserCapabilityPanel.cs` draws what was kept — never what was sent.

---

## The report

One compact string of `key=value` pairs separated by `;`, capped at 800 characters:

```
viewport=1512x842;language=en-GB;timeZone=Europe/Rome;online=1;touch=0;keyboardShortcuts=1;
clipboard=1;localStorage=1;notifications=1;camera=0;barcodeScanner=0
```

A flat, bounded string rather than an object graph, for the same reason the command payload has
three fields: it can be validated in a loop, and it cannot smuggle structure the server does not
expect.

### Detected values

| Key | Detected with | Type |
| --- | --- | --- |
| `viewport` | `window.innerWidth × innerHeight` | fact |
| `language` | `navigator.language`, stripped to letters and `-` | fact |
| `timeZone` | `Intl.DateTimeFormat().resolvedOptions().timeZone` | fact |
| `online` | `navigator.onLine` | flag |
| `touch` | `navigator.maxTouchPoints > 0 \|\| "ontouchstart" in window` | flag |
| `keyboardShortcuts` | `document.addEventListener` present | flag |
| `clipboard` | `navigator.clipboard.writeText` present | flag |
| `localStorage` | a write/remove probe inside `try` | flag |
| `notifications` | `window.Notification` is a function | flag |
| `camera` | `navigator.mediaDevices.getUserMedia` present | flag |
| `barcodeScanner` | `window.BarcodeDetector` is a function | flag |

Every probe is wrapped in `try/catch`: `localStorage` throws outright in some privacy modes, and a
capability probe that throws must read as "unavailable", not as a broken screen. **The user-agent
string is never parsed** — it says what a browser claims to be, not what it can do, and both `camera`
and `clipboard` depend on the security context (a page served over plain HTTP has neither) rather
than on the vendor.

## What the server does with it

`BrowserCapabilityService.Accept(report)`:

1. truncates the report at 800 characters and traces the raw length;
2. splits on `;`, then on the first `=`;
3. **sanitises every value**: control characters, `;` and `=` removed, capped at 40 characters — the
   value is printed into a `ListBox`, and untrusted text does not get to decide how long a line is;
4. keeps a key only if the **server** published it (`Known` for flags, `KnownFacts` for facts);
5. counts and drops everything else, with a trace line naming the ignored key;
6. picks a **fallback** for each missing capability and traces the choice.

| Missing capability | Fallback the server chooses |
| --- | --- |
| Camera | manual entry offered |
| Barcode scanner | asset code typed by hand |
| Clipboard | copy buttons hidden, text stays selectable |
| Notifications | in-page banner instead of a toast |
| Local storage | draft kept in the session on the server |
| Network online | queue actions until the socket is back |
| Keyboard shortcuts | palette reachable from the toolbar button |

The fallback is a **server** decision for a reason: it is the server that knows which alternative
flow exists, and the choice must be logged next to the command that used it.

## The rule this panel exists to make visible

`BrowserCapabilityService` is never read by `ClientCommandService`. Grep the project for
`_capabilities` and you will find it only in `CommandCenterShell` (rendering) and in
`BrowserCapabilityService` itself. There is no path from a detected capability to a permission — and
even a report edited to say `canApprove=1;role=Admin` loses both keys at step 4, and neither would
have been consulted if it had survived.

---

## Evidence — what the running app shows

| Path | How to reproduce | What proves it |
| --- | --- | --- |
| First report | load the page | the card fills a moment after `paletteReady`: one `✓`/`✕` row per capability (which ones are present depends on the browser and the security context) |
| Fallback chosen server-side | run over plain `http://localhost:5209` — `camera` is unavailable in an insecure context | the row reads `✕ Camera   manual entry offered`; server log `Service: fallback · Camera unavailable → manual entry offered` |
| Detection ≠ permission | whatever the panel shows, press Ctrl+K → *approve* → Enter | still `PERMISSION_DENIED` — the report changed nothing |
