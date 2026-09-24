# Module 5 lab note - right-to-left, and the five-case test list

Open `http://localhost:6105/?lang=ar-SA`, or pick العربية in the language list. Everything below
was read off that screen.

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

`Application.RightToLeft` reports `true` under `ar-SA` without this code testing for Arabic
anywhere - the status strip says so on every culture change.

One container is pinned the other way: `pnlWindowGlyphs`, with `RightToLeft.No`. The minimise,
maximise and close glyphs are window chrome rather than content, and they stay where the operating
system puts them in every language.

## The one field that must not flip

`txtCustomerCode`:

```csharp
this.txtCustomerCode.RightToLeft = Wisej.Web.RightToLeft.No;
```

Under Arabic the contact names are right-aligned - they mirrored with everything else - and
`GD-40117-AR` is still left-aligned and still reads in that order.

The reason is not aesthetic. A customer code is an identifier: it is typed, read aloud, quoted in
an email and compared character by character. Latin letters and digits are *neutral* characters in
the bidirectional algorithm, so inside a right-to-left paragraph the browser is entitled to reorder
the run - and a user reading the code back down the phone would say the wrong thing. Forcing LTR
keeps the identifier identical in every language.

The same applies to IBANs, part numbers, licence keys and version strings. It does **not** apply to
names or addresses, which are prose and must mirror.

## Layout that survives a long translation

Nothing on this screen is positioned absolutely:

- `pnlNav` is docked and 210 px wide, which is what `Kontaktverzeichnis` needs. The walkthrough
  shows the same column at 146 px, where German is cut in half.
- `pnlCodeRow` and `pnlButtons` are `FlowLayoutPanel`s with `WrapContents`, so a longer caption
  pushes the next control along, or onto a second row in a narrow window, instead of off the edge.
- `btnSave` and `btnCancel` are `AutoSize` with a `MinimumSize`.
- The grid's company column is `AutoSizeColumnMode.Fill`; the other two are fixed because their
  content is short in every language.

The measurable result is in the resource files: `CustomerEditor.de.resx` in this module holds
**three entries, all of them text**. Module 4's held eight, three of which were `Size` and
`Location`. A layout that flows does not need a geometry override per language.

## The five-case test list

Rerun this on any screen before calling it localized.

| # | Case | How | What passing looks like |
|---|---|---|---|
| 1 | **Short text** | `?lang=en-US` | The baseline. Nothing clipped, nothing wrapped oddly. |
| 2 | **Long text** | `?lang=qps-ploc` | Every caption bracketed and about 40% longer. Nothing clips; anything **unbracketed** is a string that never went through a resource. |
| 3 | **Narrow window** | any culture, drag the browser to ~600 px | The button row wraps onto a second line. No horizontal scrollbar, no button off the edge. |
| 4 | **RTL culture** | `?lang=ar-SA` | The whole screen mirrors: navigation on the right, grid columns reversed, buttons in reverse reading order - except `txtCustomerCode`. |
| 5 | **The grid** | `?lang=ar-SA`, look at the contacts | See below. |

## What the DataGridView actually does - recorded, not assumed

It **does** mirror, which is worth stating because it is the control people expect to be the
exception:

- Column order runs right to left. Under Arabic the first column, جهة الاتصال (Contact), is on the
  **right** and المدينة (City) on the left.
- Header text follows the mirrored direction.

What it does **not** do is impose a direction on cell contents. Each cell aligns by what is in it,
which is why the company column is the interesting one:

| Column | Content | Alignment under `ar-SA` |
|---|---|---|
| جهة الاتصال | `دانا روسي` | right |
| الشركة | `Northwind Traders` | **left** |
| المدينة | `ميلانو` | right |

The Latin company name is the same neutral-character problem as the customer code, one column
wide. It is legible here - but the direction is being decided by the content, not by you. If an
account number or a part code has to read identically for every user, set that column's alignment
explicitly rather than accepting what the bidirectional algorithm gives you.

`ListView` and `PropertyGrid` have their own documented RTL limits. **Do not promise any of this
until you have run it on the version and configuration you ship.**

## The other thing `ar-SA` changes, which has nothing to do with direction

`ar-SA`'s default calendar is **Umm al-Qura**, not the Gregorian one, and its digits use the
Arabic decimal and group separators. A `DateTime` formatted under it comes back as
`12/4/1448 بعد الهجرة`, and that is correct rather than broken - `CultureInfo.DateTimeFormat.Calendar`
decided it and no code here mentions calendars.

Two consequences: never store a formatted date (a Hijri string will not parse back or sort), and
if a date has to be Gregorian for every reader, format it with a culture you chose on purpose.
`ar-EG` is Arabic, right-to-left, and Gregorian - two Arabic cultures that do not render the same
date. Module 7's QA checklist has the long version.

## Two translation jobs, counted

This module is the first with four resource files per language:

```
Resources/Strings.<lang>.resx        shared sentences, grid headers, city and contact names
CustomerEditor.<lang>.resx           the designed captions on the customer screen
```

`ar` and `qps-ploc` have both. If they had only the first, the Arabic screen would show Arabic
navigation and an English **Customer code** label, falling back silently and looking entirely
deliberate. That is the hardest kind of localization gap to catch.

Two defences:

1. **Pseudo-localization catches it.** Under `qps-ploc` every caption is bracketed. One that is
   not, is not coming from a resource - which is exactly what Module 7's QA matrix uses it for.
2. **Count the files.** One `.resx` per language per designer-localized control, plus one shared
   file per language. A language with fewer files than the others has a hole.

The pseudo files are generated from the neutral values rather than written by hand, so they cannot
drift: adding a key to `Strings.resx` and regenerating is the whole maintenance cost.
