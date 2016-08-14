
using System;
using System.Diagnostics;

public class ProcessStartHelper
{

    public static event DataReceivedEventHandler OutputDataReceived;
    public static event EventHandler Exited;
    public static string Resize(string encoderPath, string input, string output, int width, int height)
    {
        try
        {
            string ffmpegArguments = string.Format("-i \"{0}\" -vf scale={1}:{2} -y \"{3}\"", input, width, height, output);

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = encoderPath,
                Arguments = ffmpegArguments
            };


            Process ffProcess = new Process { StartInfo = psi };

            //* Set your output and error (asynchronous) handlers

            // ffProcess.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
            // ffProcess.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);
            ffProcess.EnableRaisingEvents = true;
            ffProcess.OutputDataReceived += OutputDataReceived;
            ffProcess.ErrorDataReceived += OutputDataReceived;
            ffProcess.Exited += Exited;


            ffProcess.Start();

            ffProcess.BeginOutputReadLine();
            ffProcess.BeginErrorReadLine();
            ffProcess.WaitForExit();

            return output;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return string.Empty;
    }

    private static void OutputHandler(object sender, DataReceivedEventArgs e)
    {
        //* Do your stuff with the output (write to console/log/StringBuilder)
        Console.WriteLine(e.Data);

    }


}