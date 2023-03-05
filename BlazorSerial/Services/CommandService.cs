namespace BlazorSerial.Services;

public class CommandService
{
    public IEnumerable<Command> GetCommands()
    {
        return new[]
        {
            new Command()
            {
                Name = "Show environment variables",
                CommandString = "env show"
            },
            new Command()
            {
                Name = "Show gateway paths",
                CommandString = "gateway paths"
            },
            new Command()
            {
                Name = "Show version",
                CommandString = "version"
            }
        };
    }
}
