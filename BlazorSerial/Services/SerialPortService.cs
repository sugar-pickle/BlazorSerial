namespace BlazorSerial.Services;

public class SerialPortService
{
    public IEnumerable<SerialPort> GetSerialPorts()
    {
        return new[]
        {
            new SerialPort()
            {
                DisplayName = "Dummy"
            },
            new SerialPort()
            {
                DisplayName = "Dummy2"
            }
        };
    }
}
