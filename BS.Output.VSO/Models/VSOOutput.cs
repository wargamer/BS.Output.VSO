using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BS.Output.VSO.Models
{
    public class VSOOutput : IOutput
    {
        private static readonly ICollection<PropertyInfo> AllProperties;

        static VSOOutput()
        {
            AllProperties = typeof(VSOOutput)
                .GetProperties()
                .Where(p => p.GetSetMethod() != null)
                .ToList();
        }

        private VSOOutput()
        {
            
        }

        public VSOOutput(string strName)
        {
            Name = strName;
        }

        public VSOOutput(VSOOutput output)
        {
            URL = output.URL;
            ProjectName = output.ProjectName;
            IterationName = output.IterationName;
            BuildDefinitionName = output.BuildDefinitionName;
            Name = output.Name;
        }

        /// <summary>
        /// URL to Visual Studio Online
        /// </summary>
        public Uri URL { get; set; }

        /// <summary>
        /// Name of the project
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Iteration to link bugs with, bugs are put on the backlog when this is left empty
        /// </summary>
        public string IterationName { get; set; }

        /// <summary>
        /// Name of the build definition, which allows for easy selection of a related build during submission
        /// </summary>
        public string BuildDefinitionName { get; set; }

        /// <summary>
        /// Friendly name for this output
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Second line shown in outputs selection window
        /// </summary>
        public string Information => URL.ToString();

        public OutputValueCollection Serialize()
        {
            OutputValueCollection objOutputAttributes = new OutputValueCollection();

            foreach (var prop in AllProperties)
            {
                var val = prop.GetValue(this)?.ToString();
                if(val != null)
                    objOutputAttributes.Add(new OutputValue(prop.Name, val));
            }

            return objOutputAttributes;
        }

        public static VSOOutput Deserialize(OutputValueCollection objOutputValues, string name)
        {
            var output = new VSOOutput();

            foreach (var prop in AllProperties)
            {
                var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                if (converter.CanConvertTo(typeof(string)))
                {
                    prop.SetValue(output, converter.ConvertFrom(objOutputValues[prop.Name, string.Empty].Value));
                }
            }

            return output;
        }
    }
}