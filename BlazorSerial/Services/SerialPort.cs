using System.Text;
using MudBlazor;
using RJCP.IO.Ports;

namespace BlazorSerial.Services;

public class SerialPort
{
    public Guid Guid { get; } = Guid.NewGuid();
    public string PortName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool DrawerOpen { get; set; } = true;
    public bool Running { get; set; } = false;
    private SerialPortStream? SerialPortStream { get; set; }
    private CancellationTokenSource? CancellationTokenSource { get; set; }
    private Task? ReadTask { get; set; }
    private ReaderWriterLockSlim RwLock { get; } = new();

    private List<string> Buffer { get; } = new();

    public IEnumerable<string> GetBuffer()
    {
        RwLock.EnterReadLock();
        foreach (var s in Buffer) yield return s;
        RwLock.ExitReadLock();
    }

    public void Start()
    {
        CancellationTokenSource = new CancellationTokenSource();
        SerialPortStream = new SerialPortStream(PortName, 115200);
        SerialPortStream.Open();
        ReadTask = Task.Run(async () => await Read(SerialPortStream), CancellationTokenSource.Token);
    }

    public async Task Stop()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource = null;
        while (ReadTask is { IsCompleted: false })
        {
            await Task.Delay(500);
        }
        ReadTask = null;
        SerialPortStream?.Close();
        SerialPortStream = null;
    }

    public async Task Write(string data)
    {
        if (SerialPortStream is { IsOpen: true })
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            RwLock.EnterWriteLock();
            Buffer.Add(">>> " + data);
            RwLock.ExitWriteLock();
            await SerialPortStream.WriteAsync(bytes);
        }
    }

    private async Task Read(SerialPortStream? serialPortStream)
    {
        while (!CancellationTokenSource!.Token.IsCancellationRequested)
        {
            var buffer = new byte[1024];
            var read = await serialPortStream!.ReadAsync(buffer);
            var data = new byte[read];
            Array.Copy(buffer, 0, data, 0, read);
            var output = BitConverter.ToString(data);
            RwLock.EnterWriteLock();
            Buffer.Add(output);
            RwLock.ExitWriteLock();
        }
    }
}
