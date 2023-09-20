using System;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using Serilog;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Library.FFMPegFactory
{
    public class FMBuild
    {
        private StringBuilder sb = new StringBuilder();
        public FMBuild(FMArgs args, string pipe = null)
        {
            if (!pipe.IsNullOrEmpty())
                sb.Append($"{pipe} ").Append(" -y ").Append(args.Build());
            else sb.Append(" -y ").Append(args.Build());
        }
        public FMBuild Output(string catalog)
        {   
            sb.Append($" {catalog}");
            return this;
        }
        public async Task<bool> RunAsync(Action<string> action=null)
        {
            StringBuilder Info = new StringBuilder();
            var args = sb.ToString();
            action?.Invoke(args);
            var cmd = await Cli.Wrap(CommonHelper.FFMPEG)
              .WithArguments(args)
                   .WithStandardErrorPipe(PipeTarget.ToStringBuilder(Info))
                   .ExecuteAsync();
            Log.Logger.Information(Info.ToString());
            return cmd.ExitCode == 0;
        }
    }
}
