
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class ProcessStartHelper
{

    public event DataReceivedEventHandler OutputDataReceived;

    public event DataReceivedEventHandler ErrorDataReceived;

    public event EventHandler Exited;

    public event EventHandler<ProcessStartEventArgs> Error;

    // private IList<string> _args;

    // public ProcessStartHelper(IList<string> args)
    // {
    //     _args = args;
    // }


    public string Resize(string encoderPath, string input, string output)
    {
        try
        {
            string ffmpegArguments = $"{input} {output} 640";

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = encoderPath,
                Arguments = ffmpegArguments
            };


            Process ffProcess = new Process { StartInfo = psi };

            ffProcess.EnableRaisingEvents = true;
            ffProcess.OutputDataReceived += OutputDataReceived;
            ffProcess.ErrorDataReceived += ErrorDataReceived;
            ffProcess.Exited += Exited;


            ffProcess.Start();

            ffProcess.BeginOutputReadLine();
            ffProcess.BeginErrorReadLine();
            ffProcess.WaitForExit();

            return output;

        }
        catch (Exception ex)
        {
            ProcessStartEventArgs args = new ProcessStartEventArgs();
            args.ErrorMessage = ex.Message;
            OnError(args);
        }
        return string.Empty;
    }

    protected virtual void OnError(ProcessStartEventArgs e)
    {
        EventHandler<ProcessStartEventArgs> handler = Error;
        if (handler != null)
        {
            handler(this, e);
        }
    }
}