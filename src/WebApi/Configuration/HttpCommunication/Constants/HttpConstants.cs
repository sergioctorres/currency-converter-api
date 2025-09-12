namespace WebApi.Configuration.HttpCommunication.Constants;

public static class HttpConstants
{
    public static readonly TimeSpan[] DefaultRetryTimings =
    [
        TimeSpan.FromSeconds(1.0),
        TimeSpan.FromSeconds(2.0),
        TimeSpan.FromSeconds(4.0)
    ];

    public static readonly int HandledEventsAllowedBeforeBreaking = 3;
    public static readonly TimeSpan DurationOfBreak = TimeSpan.FromSeconds(30);
}
