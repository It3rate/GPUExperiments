using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUExperiments.Common.Series
{
    public class Document
    {
        private static Document _activeInstance;
        public static Document ActiveInstance => _activeInstance;

	    public FloatDataStore FloatDataStore { get; } = new FloatDataStore();
	    public IntDataStore IntDataStore { get; } = new IntDataStore();
    }
}
