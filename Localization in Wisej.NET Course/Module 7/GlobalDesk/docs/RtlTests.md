# Module 5 lab note - right-to-left, and the five-case test list

Open `http://localhost:6105/?lang=ar-EG`. Everything below was read off that screen.

## How the mirroring is switched on

Two settings and nothing else:

```jsonc
"rightToLeft": "auto"        // Default.json - the session mirrors when its culture is RTL
```

```csharp
this.RightToLeftLayout = true;   // DashboardPage, and CustomerEditor
```

Every child control is left on `RightToLeft.Inherit`, which is the default. That is the whole
technique: **set the container, leave the children alone.** A screen where every control carries
its own `RightToLeft` is a screen where one of them will be missed, and `auto` means the
application never has to keep a list of which languages are right-to-left.

`Application.RightToLeft` reports `True` under `ar-EG` without this code testing for Arabic
anywhere - the status line says so on every culture change.

## The one control that must not flip

`txtCode`, the customer code, is pinned:

```csharp
this.txtCode.RightToLeft = Wisej.Web.RightToLeft.No;
```

Verified side by side under Arabic: type `Anna Vogel` into the name and `AT0417` into the code.
The name is **right-aligned** - it mirrored with everything else. The code is **left-aligned** -
it did not.

The reason is not aesthetic. A customer code is an identifier: it is typed, read aloud, quoted in
an email and compared character by character. Latin letters and digits are *neutral* characters in
the bidirectional algorithm, so in an RTL paragraph the browser is entitled to reorder the run -
and a user reading `AT0417` back down the phone would say the wrong thing. Forcing LTR keeps the
identifier identical in every language.

The same applies to IBANs, part numbers, licence keys, version strings and anything a user has to
transcribe. It does **not** apply to names or addresses, which are prose and must mirror.

## Layout that survives a long translation

Nothing on the dashboard is positioned absolutely:

- `pnlActions` is a `FlowLayoutPanel` with `WrapContents`, so a longer caption pushes the next
  button onto a second row instead of off the edge.
- `btnCustomers`, `btnRecreate` and `btnSystemText` are `AutoSize` with a `MinimumSize`.
- The preview is a `TableLayoutPanel` with percentage columns, so the caption column takes what
  the longest translation needs.
- The contacts grid and the editor host are docked.

The pseudo-locale is the test for it. `qps-ploc` in the picker returns every value bracketed and
lengthened - `[Wëlcomë to GlobalDësk ~~~]` - so a layout that only just fits English visibly does
not fit, and any caption that is **not** bracketed is a string that never went through a resource.

## The five-case test list

Rerun this on any screen before calling it localized.

| # | Case | How | What passing looks like |
|---|---|---|---|
| 1 | **Short text** | `?lang=en-US` | The baseline. Nothing clipped, nothing wrapped oddly. |
| 2 | **Long text** | `?lang=qps-ploc` | Every caption is bracketed and ~40% longer. Nothing clips; anything unbracketed is a hard-coded string. |
| 3 | **Narrow window** | Any culture, drag the browser to ~600 px | The action bar wraps onto a second row. No horizontal scrollbar, no button off the edge. |
| 4 | **RTL culture** | `?lang=ar-EG` | The whole screen mirrors. Labels right, fields left, buttons in reverse reading order - except `txtCode`. |
| 5 | **The grid** | `?lang=ar-EG`, look at the contacts | See below. |

## What the DataGridView actually does - recorded, not assumed

It **does** mirror, which is worth stating because it is the control people expect to be the
exception:

- Column order runs right to left. Under Arabic the first column, الاسم (Name), is on the **right**
  and الهاتف (Phone) on the left.
- The row indicator moves to the right-hand edge.
- Header text is right-aligned.

What it does **not** do is impose a direction on cell contents. Each cell aligns by what is in it:

| Column | Content | Alignment under `ar-EG` |
|---|---|---|
| الاسم | `Anna Vogel` | right |
| الدور | `Purchasing` | right |
| الهاتف | `+49 30 5550 118` | **left** |

The phone numbers are the same neutral-character problem as the customer code, one column wide.
They are left-aligned, which is legible here - but the direction is being decided by the content,
not by you. If a phone number or an account number has to read identically for every user, set the
column's alignment explicitly rather than accepting what the bidi algorithm gives you.

## The trap this module actually exposed

Under Arabic the dashboard is fully translated - and **the customer editor is still in English**:
`Customer`, `Name`, `Customer code`, `Country`, `Save`, `Cancel`.

Nothing is broken. There is no `CustomerEditor.ar.resx`, so the designer resource fell back to the
neutral file, exactly as designed. But notice what that means: the shared text and the designer
text are **two separate translation jobs**, and translating one does not translate the other.
`Strings.ar.resx` being complete made the page look finished.

This is the hardest kind of localization gap to catch, because fallback is silent and the screen
looks deliberate. Two defences:

1. **Pseudo-localization catches it.** Under `qps-ploc` the dashboard captions are bracketed and
   the editor's are not, and the difference is visible in one glance - which is exactly what
   Module 7's QA matrix uses it for.
2. **Count the files.** One `.resx` per language per designer-localized control, plus one shared
   file per language. If a language has fewer files than the others, something was missed.
