# Module 1 lab note - which mechanism carries which icon

Written from the running application, not from the documentation. Every number and every
behaviour below was read back through `IImage` on the IconDesk command panel.

## The four buttons

| Control | Mechanism | What crosses the network |
|---|---|---|
| `btnOpen` | `Image` | `Images/open.png` is read into a `System.Drawing.Image` on the server. The browser never sees that file: Wisej.NET sends the decoded image as PNG data under a `component.wx` URL. |
| `btnSave` | `ImageSource` | The string `"icon-check"`. Nothing is decoded on the server; the client resolves the name against the theme it has already downloaded, and the picture arrives as an inline `data:image/svg+xml` URL. |
| `btnPrint` | `ImageList` + `ImageKey` | The shared collection holds the picture; the button holds the key `"print"`. |
| `btnDelete` | `ImageList` + `ImageKey` | Same collection, key `"delete"`. `lblDeleteEcho` - a Label, not a Button - points at the same key. |

`btnConflict` starts with no image property set at all, so the diagnostics have something honest
to say about an empty control.

## What happened when the key was replaced

Pressing **Replace the image under the key** assigns a different `System.Drawing.Image` to the
entry stored under `"delete"`:

```csharp
var entry = this.imagesCommands.Images["delete"];
entry.Image = LoadServerImage("delete-alt.png");
```

No control is touched. `btnDelete` and `lblDeleteEcho` both change, because both hold the key and
not the picture. That indirection is the whole reason to prefer `ImageKey` over a direct `Image`
assignment when more than one control shows the same concept: one edit, every user of the key.

The string indexer is read-only - `Images["delete"] = image` does not compile. It hands back the
live `ImageListEntry`, and assigning that entry's `Image` is how a key is repointed.

`ImageKey` was chosen over `ImageIndex` for the usual reason: `"delete"` still means delete after
somebody inserts an image above it in the collection, and `3` does not.

## What happened when two image properties were set on one button

Pressing **Set Image and ImageSource on one button** does exactly that, in that order, and then
reads the button back:

```
Image assigned (True), then ImageSource assigned - Image still set afterwards: False
```

Wisej.NET does not keep both and does not merge them. Assigning `ImageSource` **cleared** `Image`.
They feed one visual slot, so the later assignment does not win an argument - it ends one, and the
earlier property is gone by the time anything reads it back.

That is worth knowing because of how the bug usually presents. Nobody writes both lines together;
one is in the designer and the other in a helper method that runs later, and the reported symptom
is "the icon disappeared after we added the theme support". The diagnostics label finds it in one
click, because it reports the properties rather than guessing from the picture.

## One more thing the diagnostics had to learn

A control that takes its picture from an `ImageList` also returns that picture from `Image`. A
naive report that tests `Image` first therefore calls every keyed control an image-object control.
`Describe` asks about the list first for that reason, and says so in a comment - the test order is
part of the answer, not an implementation detail.
