namespace WisejPerfLab.Shell
{
    /// <summary>
    /// A screen that owns one measured scenario. The shell drives them for the warm-up run and for the
    /// three runs a median is taken from — the same code path the user's own click takes, so the numbers
    /// are comparable.
    /// </summary>
    public interface IScenarioPage
    {
        /// <summary>The scenario name, as the probe records it ("Dashboard").</summary>
        string Scenario { get; }

        /// <summary>The user action inside it, as the probe records it ("Refresh").</summary>
        string UserAction { get; }

        /// <summary>Runs the scenario exactly as the button on the screen runs it.</summary>
        void RunScenario();
    }
}
