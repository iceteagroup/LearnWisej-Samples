# Module 1 lab note - which of these is data, and which is presentation

The dashboard shows four rows. Two of them are the same thing wearing different clothes, and
telling them apart is the decision the rest of the course hangs on.

## What the page stores

```csharp
private readonly DateTime dueDate      = new DateTime(2026, 3, 15);
private readonly int      unitsOrdered = 12500;
private readonly decimal  orderTotal   = 1850.75m;
```

Three values, three types, no strings. That is what a database column holds, what an API returns
and what a calculation operates on. None of it belongs in a `.resx` file, because none of it is
words.

## What the page shows

Run it under `en-US` and the status line reports the stored values beside the formatted ones:

| Stored | Shown (en-US) | Shown (de-DE) |
|---|---|---|
| `2026-03-15` | Sunday, March 15, 2026 | Sonntag, 15. März 2026 |
| `12500` | 12,500 | 12.500 |
| `1850.75` | $1,850.75 | 1.850,75 € |

Same three values. The separators moved, the currency symbol changed sides and changed character,
and the date word order is different. Nothing in the model changed at all.

The code that does it is three lines, and what matters is what is *absent* from them:

```csharp
var culture = Application.CurrentCulture;

this.lblDateValue.Text     = this.dueDate.ToString("D", culture);
this.lblQuantityValue.Text = this.unitsOrdered.ToString("N0", culture);
this.lblAmountValue.Text   = this.orderTotal.ToString("C", culture);
```

No `"dd/MM/yyyy"`. No `"€ " + amount`. No thousands separator typed by hand. The standard
specifiers - `D`, `N0`, `C` - hand the decision to the culture, which is the only version of this
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

Nineteen keys in `Resources/Strings.resx`, and every visible word on the page comes from one of
them through `Texts.Get`. `DashboardPage.Designer.cs` contains no English sentence at all - the
captions are assigned in `ApplyTextResources`, which exists as its own method because from Module 4
it is called again whenever the session's culture changes.

Keys are named for **meaning**, not wording: `CustomerEditor.Save`, `Navigation.Customers`,
`Validation.Required`. Using the English sentence as the key means every reword breaks every
translation file.

## The bracketed marker

`Texts.Get` returns `[Key.Name]` when a key is missing, rather than null or an empty string:

```csharp
public static string Get(string key) => Resources.GetString(key) ?? $"[{key}]";
```

An empty caption is a defect nobody reports - it reads as a design choice. `[Dashboard.Welcome]`
on screen is something a tester can see, describe and file. Try it: ask for a key that is not in
the file and the page still renders, with the gap visible.

This matters more once translations exist. .NET's fallback means a missing German value quietly
shows English, and the user may never mention it. The marker only catches keys missing from the
**neutral** file - which is why that file must be complete - and Module 7's pseudo-localized
resource set is the tool that catches the rest.

## What is not localized, on purpose

The customer code, log identifiers, exception type names and database column names stay as they
are. A customer code is an identifier: translating it would break the thing it identifies. Module 5
takes this further and keeps that one field left-to-right even under an Arabic layout.
