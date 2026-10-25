using RoleClaimLab;

var tickets = new[]
{
    new PartnerIdentityTicket("reviewer-104", ["Reviewer"]),
    new PartnerIdentityTicket("reader-208", ["Reader"]),
    new PartnerIdentityTicket("reviewer-310", ["Auditor", "Reviewer"])
};

foreach (var ticket in tickets)
{
    var principal = PartnerIdentityAdapter.FromValidatedTicket(ticket);
    var granted = await ReviewAuthorization.IsReviewerAsync(principal);
    Console.WriteLine(
        $"subject={ticket.Subject}; authenticated={principal.Identity?.IsAuthenticated}; " +
        $"claims=[{string.Join(", ", principal.Claims.Select(c => $"{c.Type}:{c.Value}"))}]; " +
        $"reviewAllowed={granted}");
}
