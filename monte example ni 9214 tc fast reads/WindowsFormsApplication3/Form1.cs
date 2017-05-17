using System;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;

using NationalInstruments.DAQmx;
using NationalInstruments;


namespace WindowsFormsApplication3
{
  public partial class Form1 : Form
  {
    private Task myTask;
    private Task runningTask;

    private AsyncCallback myAsyncCallback;
    private AnalogWaveform<double>[] data;

    private AnalogMultiChannelReader analogInReader;

    private int nSamplesPerChannel = 1;


    public Form1()
    {

      InitializeComponent();

      myAsyncCallback = new AsyncCallback(AnalogInCallback);
      string [] physicalChanels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External);



    }

    private void AnalogInCallback(IAsyncResult ar)
    {
      double[] dDataArray = new double[40];
      try
      {
        if (runningTask != null && runningTask == ar.AsyncState)
        {
          Trace.WriteLine("EndReadWaveform");
          data = analogInReader.EndReadWaveform(ar);
          for (int nIndex = 0; nIndex < 40; nIndex++)
          {
            dDataArray[nIndex] = data[nIndex].Samples[nSamplesPerChannel - 1].Value;
          }

          Trace.WriteLine("BeginMemoryOPtimizedReadWaveform");
          analogInReader.BeginMemoryOptimizedReadWaveform(nSamplesPerChannel, myAsyncCallback, myTask, data);
        }
      }
      catch (DaqException exception)
      {
        MessageBox.Show(exception.Message);
        myTask.Dispose();
        runningTask = null;
      }

    }


  }
}
