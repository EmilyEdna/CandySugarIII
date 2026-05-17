using System.Collections.Generic;
using System.Text;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Library.FFMPegFactory
{
    public class FMArgs
    {
        private StringBuilder sb = new StringBuilder();
        public FMArgs Loop(double args)
        {
            sb.Append($" -loop {args} ");
            return this;
        }
        public FMArgs FrameRate(double args)
        {
            sb.Append($" -framerate {args} ");
            return this;
        }
        public FMArgs Thread(double args)
        {
            sb.Append($" -threads {args} ");
            return this;
        }
        public FMArgs ConCat(List<string> args)
        {
            sb.Append($" -i \"concat:{string.Join("|", args)}\" ");
            return this;
        }
        public FMArgs InputFile(string args)
        {
            sb.Append($" -i \"{args}\" ");
            return this;
        }
        public FMArgs AudioCodec(string args)
        {
            sb.Append($" -acodec {args} ");
            return this;
        }
        public FMArgs VideoCodec(string args)
        {
            sb.Append($" -c:v {args} ");
            return this;
        }
        public FMArgs VideoFormat(string args)
        {
            sb.Append($" -pix_fmt {args} ");
            return this;
        }
        public FMArgs Rate(double args)
        {
            sb.Append($" -r {args} ");
            return this;
        }
        public FMArgs Sreen(double w, double h)
        {
            sb.Append($" -s {w}*{h} ");
            return this;
        }
        public FMArgs Aspect(double w, double h)
        {
            sb.Append($" -aspect {w}:{h} ");
            return this;
        }
        public FMArgs AudioTime(string args)
        {
            sb.Append($" -t {args} ");
            return this;
        }
        public FMArgs Quality(double args)
        {
            sb.Append($" -ab {args}k ");
            return this;
        }
        public FMArgs Codec(string args)
        {
            sb.Append($" -codec {args} ");
            return this;
        }

        public FMArgs NoVideo()
        {
            sb.Append($" -vn ");
            return this;
        }

        public FMArgs BitAudio(int args)
        {
            sb.Append($" -b:a {args}k ");
            return this;
        }

        public FMArgs AudioRate(double args)
        {
            sb.Append($" -ar {args} ");
            return this;
        }

        public FMArgs CodecMp3()
        {
            sb.Append($" -c:a mp3 ");
            return this;
        }

        public FMArgs Args(params string[] args)
        {
            if (args.Length > 0)
                args.ForArrayEach<string>(item =>
                {
                    if (!item.IsNullOrEmpty()) sb.Append($" {item} ");
                });
            return this;
        }
        internal string Build() => sb.ToString();
    }
}
