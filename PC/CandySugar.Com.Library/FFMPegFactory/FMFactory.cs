using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.FFMPegFactory
{
    public class FMFactory
    {
        public static FMBuild ImagePipe(Action<FMArgs> action)
        {
            FMArgs args = new FMArgs();
            action.Invoke(args);
            return new FMBuild(args,"-f image2pipe");
        }
        public static FMBuild Default(Action<FMArgs> action) {
            FMArgs args = new FMArgs();
            action.Invoke(args);
            return new FMBuild(args);
        }
    }
}
