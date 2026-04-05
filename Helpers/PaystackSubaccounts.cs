namespace GlobalFlameMinistry.API.Helpers
{
    public static class PaystackSubaccounts
    {
        // ✅ Replace these with real subaccount codes from Paystack dashboard
        // Format: SUB_xxxxxxxxxxxx
        private static readonly Dictionary<string, string> _subaccounts = new()
        {
            { "Tithe & Offering",              "SUB_placeholder_tithe" },
            { "Building Projects",  "SUB_placeholder_building" },
            { "Children Ministry",  "SUB_placeholder_children" },
            { "Global & Community Outreach", "SUB_placeholder_community" },
            { "Event",              "SUB_placeholder_events" },
            { "General",            "SUB_placeholder_general" },
        };

        public static string? GetSubaccount(string donationType)
        {
            _subaccounts.TryGetValue(donationType, out var code);
            return code;
        }
    }
}