using System;
using System.IO.Ports;
using System.Diagnostics;

namespace MachineMonitoringApp.Services
{
    public class SerialDataLogger
    {
        private SerialPort _serialPort;

        // Delegasi untuk event data diterima
        public delegate void DataReceivedEventHandler(string rawData);
        public event DataReceivedEventHandler DataReceived;

        public SerialDataLogger(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.ReadTimeout = 500; // 5 detik untuk toleransi jeda
            _serialPort.NewLine = "\r\n";
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        public void Open()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Membaca satu baris penuh
                string rawData = _serialPort.ReadLine();
                // Memicu event agar MainForm dapat memproses data
                DataReceived?.Invoke(rawData); 
            }
            catch (TimeoutException) 
            {
                // Abaikan TimeoutException. Ini normal saat buffer kosong.
                Debug.WriteLine("Error reading from serial port: The operation has timed out.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General error reading from serial port: {ex.Message}");
            }
        }
    }
}