# Module 1 lab note - which of these is data, and which is presentation

The culture preview card shows three rows. Each one is a stored value wearing clothes the session's
culture chose, and telling the two apart is the decision the rest of the course hangs on.

## What the page stores

```csharp
private readonly DateTime previewDate   = new DateTime(2025, 10, 14);
private readonly decimal  previewCount  = 1234567.89m;
private readonly decimal  previewAmount = 1850.75m;
```

Three values, three types, no strings. That is what a database column holds, what an API returns
and what a calculation operates on. None of it belongs in a `.resx` file, because none of it is
words.

## What the page shows

| Stored | Shown (en-US) | Shown (de-DE) |
|---|---|---|
| `2025-10-14` | Tuesday, October 14, 2025 | Dienstag, 14. Oktober 2025 |
| `1234567.89` | 1,234,567.89 | 1.234.567,89 |
| `1850.75` | $1,850.75 | 1.850,75 € |

Same three values. The separators swapped roles, the currency symbol changed sides and changed
character, and the date word order is different. Nothing in the model changed at all.

The code that does it is three lines, and what matters is what is *absent* from them:

```csharp
var culture = Application.CurrentCulture;

this.lblDateValue.Text   = this.previewDate.ToString("D", culture);
this.lblCountValue.Text  = this.previewCount.ToString("N2", culture);
this.lblAmountValue.Text = this.previewAmount.ToString("C", culture);
```

No `"dd/MM/yyyy"`. No `"€ " + amount`. No thousands separator typed by hand. The standard
specifiers - `D`, `N2`, `C` - hand the decision to the culture, which is the only version of this
code that works in a country nobody on the team has thought about yet.

`"€ " + amount` is the specific mistake worth naming. It compiles, it looks right in the office,
and it has hard-coded a currency for every market the product will ever be sold in.

## The two categories, as a rule

| It is **data** if | It is **presentation** if |
|---|---|
| A calculation uses it | Only a human reads it |
| It round-trips to storage or an API | It is produced at display time |
| Changing the user's language must not change it | Changing the user's language must change it |

`DateTime`, `decimal`, `int`, an enum value, a customer code: data. A formatted date, a caption,
a validation message, a currency string: presentation.

The test that settles arguments: **if you stored it, could you still sort by it?** `1.850,75 €`
sorts as text and is useless. `1850.75m` sorts as a number. Store the one you can sort.

## Where the words live

Fourteen keys in `Resources/Strings.resx`, and every visible word on the page comes from one of
them through `Texts.Get`. `DashboardPage.Designer.cs` contains no English sentence at all - the
captions are assigned in `ApplyTextResources`, which exists as its own method because from Module 4
it is called again whenever the session's culture changes.

Keys are named for **meaning**, not wording: `CustomerEditor.Save`, `Navigation.Customers`,
`Validation.Required`. Using the English sentence as the key means every reword breaks every
translation file.

Twelve of the fourteen are the keys the walkthrough lists. The other two are the ones this screen
needs and the walkthrough assumes: `App.Title` for the application bar and `Status.Ready` for the
status line, which is a composed sentence with two placeholders rather than three fragments glued
together.

## The bracketed marker

`Texts.Get` returns `[Key.Name]` when a key is missing, rather than null or an empty string:

```csharp
var value = Resources.GetString(key);
if (value == null) { Missing.Add(key); return "[" + key + "]"; }
```

An empty caption is a defect nobody reports - it reads as a design choice. `[Dashboard.Welcome]`
on screen is something a tester can see, describe and file. Try it: ask for a key that is not in
the file and the page still renders, with the gap visible.

The helper also counts what it resolved, which is why the status line can say *eight keys
resolved* instead of *ready*. A number is checkable; a word is not.

This matters more once translations exist. .NET's fallback means a missing German value quietly
shows English, and the user may never mention it. The marker only catches keys missing from the
**neutral** file - which is why that file must be complete - and Module 7's pseudo-localized
resource set is the tool that catches the rest.

## What is not localized, on purpose

The culture chip in the card header (`Application.CurrentCulture = en-US`), the three window
glyphs, log identifiers, exception type names and database column names stay as they are. A
culture name is an identifier, not a word: `de-DE` is `de-DE` in every language. Module 5 takes
this further and keeps the customer code field left-to-right even under an Arabic layout.
