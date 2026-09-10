using System.Collections.Generic;

namespace OrderDesk.Migration
{
    /// <summary>What kind of static the audit found.</summary>
    public enum StaticKind
    {
        ImmutableLookup,
        PerUserState,
        PerProcessCache,
        Registry
    }

    /// <summary>Where the value goes on the server.</summary>
    public enum StaticVerdict
    {
        KeepStatic,
        MoveToSession,
        MoveToProfileStore,
        MoveToBrowserStorage,
        Remove
    }

    /// <summary>One static field, singleton or static-backed setting found in LegacyOrderDesk.</summary>
    public sealed class StaticStateItem
    {
        public string Member { get; set; }        // AppState.CurrentUser
        public string DeclaredIn { get; set; }    // LegacyOrderDesk/AppState.cs
        public StaticKind Kind { get; set; }
        public StaticVerdict Verdict { get; set; }
        public string Why { get; set; }
        public string Replacement { get; set; }

        public string KindText => Kind switch
        {
            StaticKind.ImmutableLookup => "immutable lookup",
            StaticKind.PerUserState => "per-user state",
            StaticKind.PerProcessCache => "per-process cache",
            StaticKind.Registry => "registry (HKCU)",
            _ => Kind.ToString()
        };

        public string VerdictText => Verdict switch
        {
            StaticVerdict.KeepStatic => "Keep static",
            StaticVerdict.MoveToSession => "Move to session",
            StaticVerdict.MoveToProfileStore => "Move to profile store",
            StaticVerdict.MoveToBrowserStorage => "Move to browser storage",
            _ => "Remove"
        };
    }

    /// <summary>
    /// The static-state audit of LegacyOrderDesk — lab step 1 (search static fields and singleton
    /// state) and step 2 (classify) as data, so the console can show, filter and count it. The same
    /// rows are in docs/StaticStateAudit.md.
    ///
    /// Found with: grep -rn --include=*.cs --exclude=*.Designer.cs "static" LegacyOrderDesk. Of its
    /// hits, 8 are static FIELDS (the six in AppState, SampleData.Customers, InMemoryOrderRepository.Shared)
    /// — those are the rows. The remaining hits are static classes and methods with no static fields
    /// (InvoiceDocument.Build, ExcelExport.ExportOrders, InvoicePrinter.Print, SampleData.Orders,
    /// CustomerService.Clone, RegistrySettings.Load/Save, Program.Main): they hold no state, nothing is
    /// shared between sessions, so they are out of scope and get no verdict. The 3 HKCU-backed settings
    /// of RegistrySettings were added by hand: they are instance properties, but Registry.CurrentUser on
    /// a server is one hive for every visitor — process-global state in disguise. 11 rows in total.
    ///
    /// The rule applied to every row: would the value differ if two users opened the app at the
    /// same time? Yes → it is not a safe global; No, and it is never written after startup → it may stay.
    /// </summary>
    public static class StaticStateAudit
    {
        public static readonly IReadOnlyList<StaticStateItem> Items = new List<StaticStateItem>
        {
            new StaticStateItem { Member = "AppState.CurrentUser", DeclaredIn = "LegacyOrderDesk/AppState.cs", Kind = StaticKind.PerUserState, Verdict = StaticVerdict.MoveToSession,
                Why = "Set by LoginForm; two users signed in at the same time need two different values. One static slot means the second sign-in overwrites the first for everyone.",
                Replacement = "UserSessionContext.UserName via SessionContext.Current (Application.Session)." },
            new StaticStateItem { Member = "AppState.CurrentCompany", DeclaredIn = "LegacyOrderDesk/AppState.cs", Kind = StaticKind.PerUserState, Verdict = StaticVerdict.MoveToSession,
                Why = "kelly works for Acme, sam for Globex — differs per user, so it cannot be process-wide.",
                Replacement = "UserSessionContext.Company." },
            new StaticStateItem { Member = "AppState.CurrentCustomer", DeclaredIn = "LegacyOrderDesk/AppState.cs", Kind = StaticKind.PerUserState, Verdict = StaticVerdict.MoveToSession,
                Why = "Workflow state of one user's Orders screen. sam selecting Fabrikam silently replaces kelly's Northwind; passes every single-user test.",
                Replacement = "UserSessionContext.CurrentCustomerId (store the id, resolve through CustomerService)." },
            new StaticStateItem { Member = "AppState.CurrentFilter", DeclaredIn = "LegacyOrderDesk/AppState.cs", Kind = StaticKind.PerUserState, Verdict = StaticVerdict.MoveToSession,
                Why = "A view filter is per screen, per user. Changing it in tab B re-filters tab A on its next refresh.",
                Replacement = "UserSessionContext.CurrentFilter (OrderStatus?)." },
            new StaticStateItem { Member = "AppState.LastSearch", DeclaredIn = "LegacyOrderDesk/AppState.cs", Kind = StaticKind.PerUserState, Verdict = StaticVerdict.MoveToSession,
                Why = "Per-user workflow state; also a small privacy leak if another session can read what a user searched for.",
                Replacement = "UserSessionContext.LastSearch." },
            new StaticStateItem { Member = "AppState.Countries", DeclaredIn = "LegacyOrderDesk/AppState.cs", Kind = StaticKind.ImmutableLookup, Verdict = StaticVerdict.KeepStatic,
                Why = "readonly, never written after startup, identical for every user. Two users opening the app see the same list — the rule says it may stay global.",
                Replacement = "None. (Would become a read-only collection if anything ever mutated it.)" },
            new StaticStateItem { Member = "SampleData.Customers", DeclaredIn = "LegacyOrderDesk/Domain/Services/SampleData.cs", Kind = StaticKind.ImmutableLookup, Verdict = StaticVerdict.KeepStatic,
                Why = "Reference data, user-independent, read-only in practice: CustomerService hands out clones so no caller can mutate the shared array.",
                Replacement = "None — but keep the clone-on-read discipline; a shared mutable object would be the same bug as CurrentCustomer." },
            new StaticStateItem { Member = "InMemoryOrderRepository.Shared", DeclaredIn = "LegacyOrderDesk/Domain/Services/OrderService.cs", Kind = StaticKind.PerProcessCache, Verdict = StaticVerdict.KeepStatic,
                Why = "The lab's stand-in for the database: ONE store for every session is the intended semantics, and every access is under a lock, so it is thread-safe and user-independent.",
                Replacement = "None. It plays the database; the real system replaces it with the database." },
            new StaticStateItem { Member = "RegistrySettings.GridDensity", DeclaredIn = "LegacyOrderDesk/Settings/RegistrySettings.cs", Kind = StaticKind.Registry, Verdict = StaticVerdict.MoveToBrowserStorage,
                Why = "HKCU on the server is the service account's hive, shared by every visitor. Density is a pure UI preference that may differ per device.",
                Replacement = "localStorage['orderdesk.density'] via Application.Eval/EvalAsync (BrowserPreferences) — browser storage first; the profile store keeps a roaming copy only while a user is signed in (a new device would start from the profile value when the page seeds — not built)." },
            new StaticStateItem { Member = "RegistrySettings.ExportFolder", DeclaredIn = "LegacyOrderDesk/Settings/RegistrySettings.cs", Kind = StaticKind.Registry, Verdict = StaticVerdict.MoveToProfileStore,
                Why = "\"C:\\Orders\" is a path on the user's PC; the server cannot write there. Per user, must follow the user to any device, business-relevant → server-owned.",
                Replacement = "UserProfile.ExportFolder = a folder under the storage root (UserProfileStore, App_Data/profiles/<user>.json); files reach the user via Application.Download." },
            new StaticStateItem { Member = "RegistrySettings.WindowWidth / WindowHeight", DeclaredIn = "LegacyOrderDesk/Settings/RegistrySettings.cs", Kind = StaticKind.Registry, Verdict = StaticVerdict.Remove,
                Why = "The browser window belongs to the user; the server neither knows nor sets its size.",
                Replacement = "Remove. Responsive layout instead (Module 7)." },
        };
    }
}
