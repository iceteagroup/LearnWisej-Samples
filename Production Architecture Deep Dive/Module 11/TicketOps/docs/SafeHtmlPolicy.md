# Safe text / HTML policy

*Module 11 deliverable · implemented in `Security/HtmlPolicy.cs`*

## The rule

1. **User text is data, not markup.** Anything a person can type (notes, titles, comments, user names)
   is rendered as text. Labels keep `AllowHtml = false` — the Wisej.NET default — and the framework
   escapes the string, so `<img src=x onerror=alert(1)>` is shown as those characters.
2. **Every HTML-capable surface is a review item.** A label with `AllowHtml = true`, a `ListBox` (renders
   HTML by default — the activity trace and the audit list are ListBoxes), a tooltip, a grid cell style
   with HTML enabled, a message box with markup. Before user text reaches one of these it goes through
   `HtmlPolicy.Encode` (encode on output: `<`, `>`, `&`, quotes → entities).
3. **Formatting, if truly needed, comes from an allow-list.** `HtmlPolicy.RenderWithAllowList` encodes
   the whole string first and then restores only the exact, attribute-free tags below. Anything else
   stays encoded.
4. **Every `AllowHtml = true` in the code base carries a comment saying why**, and a reviewer can find
   them all with `grep -rn "AllowHtml = true"`.

## The allow-list

| Tag | Restored forms | Why it is safe |
|---|---|---|
| `b` | `<b>`, `</b>` | literal tag, no attributes possible |
| `i` | `<i>`, `</i>` | literal tag, no attributes possible |
| `br` | `<br>` | literal tag, no attributes possible |

Not on the list and therefore always inert text: `<img>`, `<a>`, `<script>`, `<style>`, `<iframe>`,
`<svg>`, any tag with an attribute (`<b onclick=…>` stays `&lt;b onclick=…&gt;`), any other case (`<B>`).
Adding a tag to `HtmlPolicy.AllowedTags` is a security review item.

## Where it is applied in this sample

| Surface | `AllowHtml` | What it receives |
|---|---|---|
| `WorkOrdersView.labelNotePlain` | `false` (explicit) | `ticket.Note` raw — the framework escapes it |
| `WorkOrdersView.labelNoteAllowList` | **`true` — REVIEWED** (comment in the Designer) | `HtmlPolicy.RenderWithAllowList(ticket.Note)` only |
| Activity trace `ListBox` (`ActivityTracePanel`) | HTML-capable | log lines; user text is encoded **before logging** (`TicketService.AddNoteAsync`, `AuthenticationService.SignInAsync`) |
| Audit list `ListBox` (`WorkOrdersView.listAudit`) | HTML-capable | `AuditLog.Format(entry)` — entries hold no user text (a note is audited as "N chars") |
| `DataGridView` cells | default | ids, seeded titles, status, `"N chars"`, closed-by user names — no free text |
| `LoginView.labelIdentity`, `labelSignedIn`, `labelSelected` | default (`false`) | server values / seeded titles |

## Evidence

Select #2002, type `Pump failed <b>again</b> <img src=x onerror=alert(1)>`, click **Render ticket note**:

- left box (escaped): `Pump failed <b>again</b> <img src=x onerror=alert(1)>` shown literally;
- right box (allow-list): **again** in bold, then the literal text `<img src=x onerror=alert(1)>`;
- trace: `[SVC] TicketService.AddNoteAsync — #2002 note (53 chars, contains markup — stored as text): Pump failed &lt;b&gt;again&lt;/b&gt; …` rendered by the ListBox as the visible characters, and
  `[UI] WorkOrdersView.RenderNote — note contains markup → rendered as text (AllowHtml = false) and through the allow-list … — nothing executes`;
- audit: `[AUDIT] ✓ AddNote #2002 — l.romero (Technician): 53 chars` — no note body.

No alert appears; nothing in the page changed except the two boxes.
