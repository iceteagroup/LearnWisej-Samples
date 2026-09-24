# GlobalDesk · Localization in Wisej.NET · Module 6

Local lab build for **Module 6 · Translation Workflow: ResX Resource Manager and Other Tools**.
The GlobalDesk service desk exactly as the walkthrough shows it: the blue application bar, the
toolbar with the product name and the **Language** picker, the side rail (**Customers** /
**Tickets** and the **Culture** block), the customer card with **Name** and **Email**, the
last-order line, **Save** / **Cancel**, the **Culture preview** heading with its two status chips,
and the three value tiles — Date, Quantity, Amount.

What Module 6 added is the Italian column: `Strings.it.resx` and `CustomerEditor.it.resx`, with
comments on the ambiguous keys, invariant markers on the ones nobody may translate, and a
developer report in the rail that measures the round on the running application instead of in the
tool.

## Run it

```bash
cd "Localization in Wisej.NET Course/Module 6/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6106
```

Open <http://localhost:6106>, or go straight to a culture with `?lang=it-IT` in a **new tab**.

## What to try

| Action | Expected result |
|---|---|
| Pick **Italiano (it-IT)** | `Servizio assistenza`, `Bentornata, Ana.`, `Ultimo ordine: 4187 il 23/09/2026`, `Salva` / `Annulla`, `Aperto` / `Chiuso`, and `23/09/2026` · `1.250,00` · `1.250,00 €`. |
| Read the rail | `it-IT · Strings.it.resx` and the report underneath. |
| Hover the report | The keys it counted. |
| Read the last-order line in both languages | `Last order: 4187 on 9/23/2026` and `Ultimo ordine: 4187 il 23/09/2026` — `{0}` is the order **number** in both. A machine draft that swaps them compiles and ships. |
| Look at `App.ProductName` in every file | `GlobalDesk`, unchanged, and marked `IsInvariant` in the neutral file so no translator or round trip can touch it. |
| Pick **Pseudo (qps-ploc)** | Every caption bracketed. The company name and the email address are not, because they are data. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Italian added as a language column | [Resources/Strings.it.resx](GlobalDesk/Resources/Strings.it.resx), [CustomerEditor.it.resx](GlobalDesk/CustomerEditor.it.resx) |
| Comments on the ambiguous keys | `CustomerEditor.Open` (verb) beside `Ticket.Open` (state), `Customer.LastOrder`, `Rail.Culture` — in the neutral file |
| Invariant markers | `App.ProductName`, `Product.Framework`, `Format.CustomerCode` and the six `Language.*` entries carry `<metadata name="…IsInvariant">` |
| The untranslated filter, rerun on the application | `Texts.UntranslatedKeys()` in [Texts.cs](GlobalDesk/Texts.cs), reported in the rail |
| The reviewed round | [docs/TranslationWorkflow.md](GlobalDesk/docs/TranslationWorkflow.md) |
| The export / import round trip | [docs/translation/Strings.export.csv](GlobalDesk/docs/translation/Strings.export.csv) |

## Notes

- Every language file carries the **whole** key list. A key missing from a translation falls back
  to the neutral value in silence, and the screen still looks finished.
- The report in the rail counts keys whose value under this culture is still the neutral one. It
  is a heuristic, not a verdict: Italian legitimately says **Email**, and *Ana* is *Ana* in every
  language. That is why it is amber rather than red, and why the tooltip names the keys — the
  point is to make a reviewer look, not to fail a build.
- The pseudo-locale files are **generated** from the neutral values, never hand written. Adding a
  key and regenerating is the whole maintenance cost.
- `Customer.LastOrder` is the key the review exists for. Both `{0}` and `{1}` must survive, in an
  order that makes sense in the target language.
