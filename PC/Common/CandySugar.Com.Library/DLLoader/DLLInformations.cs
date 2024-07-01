using System;

namespace CandySugar.Com.Library.DLLoader
{
    public class DLLInformations
    {
        public Type IocModule { get; set; }
        public Type InstanceType { get; set; }
        public Type InstanceViewModel { get; set; }
        public bool IsEnable { get; set; }
        public string Description { get; set; }
    }
}
