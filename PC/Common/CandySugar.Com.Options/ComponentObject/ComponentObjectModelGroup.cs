using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Options.ComponentObject
{
    public class ComponentObjectModel
    {
        public string Plugin { get; set; }
        public string Bootstrapper { get; set; }
        public string Description { get; set; }
        public string Ioc { get; set; }
        public int Code {  get; set; }
    }
    public class ComponentObjectModelGroup
    {
        public List<ComponentObjectModel> Vip { get; set; }
        public List<ComponentObjectModel> Normal { get; set; }
    }
}
