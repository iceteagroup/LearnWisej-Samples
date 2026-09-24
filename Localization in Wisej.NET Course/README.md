# Localization in Wisej.NET - lab samples

One runnable application per module, built cumulatively. **GlobalDesk** starts as an English
dashboard and ends speaking five languages, one of them right to left, with a QA matrix it can run
on itself.

| Module | Folder | Port | What it adds |
|---|---|---|---|
| - | `_template` | 6100 | The dashboard shell the course starts from |
| 1 | `Module 1` | 6101 | Resource keys, the `Texts` helper, culture-formatted values |
| 2 | `Module 2` | 6102 | The designer-localized `CustomerEditor` and its German variant |
| 3 | `Module 3` | 6103 | `Strings.de.resx`, composed sentences, a save service that returns codes |
| 4 | `Module 4` | 6104 | `culture: auto`, the `CultureChanged` switch, `?lang=`, fallback |
| 5 | `Module 5` | 6105 | Right-to-left, the pinned identifier, pseudo-localization |
| 6 | `Module 6` | 6106 | Italian, translator comments, invariant markers, the export |
| 7 | `Module 7` | 6107 | `LocalizationService`, `TicketStatus`, the QA matrix |

Each folder is the complete application at that point in the course, with its own solution.

```bash
cd "Localization in Wisej.NET Course/Module 1/GlobalDesk"
dotnet run -f net10.0 --urls http://localhost:6101
```

Every module builds for `net10.0-windows` and `net10.0`. Wisej-4 is pinned to **4.1.4** to match
the other new course.

## The findings worth knowing before you start

Each of these came out of running the code. `_template/COOKBOOK.md` has them with the evidence.

- **Designer resources are applied once, at construction.** A culture change does nothing to a
  designer-localized control until it is rebuilt. Two identical symptoms - "the caption did not
  change" - have two different causes and two different fixes.
- **Designer text and shared text are separate translation jobs.** `Strings.ar.resx` being
  complete does not translate `CustomerEditor`; the fallback is silent and the screen looks
  intentional.
- **A missing-key policy and pseudo-localization catch different defects.** The policy sees a key
  nobody ever wrote. Pseudo-localization sees a key missing from a *translation*, which falls back
  silently. Neither sees the other's.
- **`de-AT` separates language from formatting** in one screenshot: German words, Austrian numbers.
- **`DataGridView` does mirror under RTL** - column order and the row indicator - but cell contents
  align by content, so a phone number ends up left-aligned.
- **A `.resx` with a hyphen before the culture is not a culture-specific resource.** MSBuild only
  builds a satellite for `Name.<culture>.resx`.

## Known gap

The Wisej system-label override in Module 3 does not work. Two namings were tried and verified
against the built assembly; the file, the evidence and what is left to check are in
`Module 3/GlobalDesk/docs/TextKinds.md`, and the application reports what is really on the
MessageBox buttons rather than claiming a success it did not have. Everything else is verified.
