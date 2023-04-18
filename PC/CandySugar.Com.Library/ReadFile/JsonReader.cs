using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace CandySugar.Com.Library.ReadFile
{
    public class JsonReader
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static void JsonRead(string path, List<string> jsonFile)
        {
            var builder = new ConfigurationBuilder().SetBasePath(path);
            jsonFile.ForEach(item =>
            {
                builder = builder.AddJsonFile(item,true,true);
            });
            Configuration = builder.Build();
        }
    }
}
