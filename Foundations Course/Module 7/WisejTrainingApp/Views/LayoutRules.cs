namespace WisejTrainingApp.Views
{
    /// <summary>
    /// The spacing rules every page follows (lab step 7 · "Consistent spacing"). The Designer files use these
    /// numbers as literals — this class is the one place that says what they are, so the pages feel like one app:
    ///
    ///   PagePadding       32 px  left padding of every heading and every card; right/bottom margin of the last card
    ///   HeadingTop        24 px  the page title sits 24 px from the top of the content area
    ///   DescriptionTop    60 px  the one-line description sits under the title
    ///   FirstRowTop      100 px  the first row of cards starts here on every page
    ///   CardGap           24 px  horizontal AND vertical gap between cards
    ///   CardPadding       20 px  inner padding of every card (card title at 20,14 · card content from y = 52)
    ///   MetricCardWidth  250 px  the four metric cards (4 × 250 + 3 × 24 = 1072 px of the 1148 px content width)
    ///   MetricCardHeight 110 px
    ///   WideCardWidth    524 px  two metric cards plus one gap — commands, activity, customers, reports, settings cards
    ///   ButtonHeight      36 px  every command button; command buttons in a row are 12 px apart
    ///
    /// Content area = 1148 × 620 (window 1348 × 720 minus the 200 px navigation, the 64 px header and the 36 px status bar).
    /// Cards that should grow with the window use Anchor (Top | Bottom | Left | Right); the metric cards stay fixed.
    /// </summary>
    public static class LayoutRules
    {
        public const int PagePadding = 32;
        public const int HeadingTop = 24;
        public const int DescriptionTop = 60;
        public const int FirstRowTop = 100;
        public const int CardGap = 24;
        public const int CardPadding = 20;
        public const int MetricCardWidth = 250;
        public const int MetricCardHeight = 110;
        public const int WideCardWidth = 524;
        public const int ButtonHeight = 36;
    }
}
