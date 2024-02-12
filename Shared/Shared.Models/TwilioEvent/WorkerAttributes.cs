namespace Shared.Models.TwilioEvent;

public class WorkerAttributes
{
    public Routing Routing { get; set; }
    public string FullName { get; set; }
    public List<string> Roles { get; set; }
    public string ContactUri { get; set; }
    public string SelectedCallerId { get; set; }
    public DisabledSkills DisabledSkills { get; set; }
    public string Email { get; set; }
}