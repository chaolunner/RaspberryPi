using System.Threading;
using System.IO.Ports;
using UnityEngine;
using System.Linq;

public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    public string PortName = "/dev/tty.raspberrypi-SerialPort";
    public int BaudRate = 9600;

    private SerialPort serialPort;
    private Thread thread;
    private bool isRunning;

    private string message;
    private bool isNewMessageReceived;
    private const string newLine = "\n";

    private void Awake()
    {
        Open();
    }

    private void Update()
    {
        if (isNewMessageReceived && OnDataReceived != null)
        {
            OnDataReceived(message);
        }
        isNewMessageReceived = false;
    }

    private void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        if (SerialPort.GetPortNames().Contains(PortName))
        {
            serialPort = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
            serialPort.Open();

            isRunning = true;

            thread = new Thread(Read);
            thread.Start();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("The port named " + PortName + " could not be found!");
            Debug.Log("You can try to use the following ports:");

            foreach (var portName in SerialPort.GetPortNames())
            {
                Debug.Log(portName);
            }
#endif
        }
    }

    private void Close()
    {
        isNewMessageReceived = false;
        isRunning = false;

        if (serialPort != null)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            serialPort.Dispose();
            serialPort = null;
        }

        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }
    }

    public bool IsConnected()
    {
        return isRunning && serialPort != null && serialPort.IsOpen;
    }

    private void Read()
    {
        while (IsConnected())
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    message = serialPort.ReadLine();
                    isNewMessageReceived = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void Write(string message)
    {
        if (IsConnected())
        {
            try
            {
                if (!message.EndsWith(newLine, System.StringComparison.Ordinal))
                {
                    message += newLine;
                }
                serialPort.Write(message);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}