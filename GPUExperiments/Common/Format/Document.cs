namespace GPUExperiments.Common.Format
{
    public class Document
    {
        private static Document _activeInstance = new Document();
        public static Document ActiveInstance => _activeInstance;

	    public FloatBlocks FloatBlocks { get; } = new FloatBlocks();
	    public IntBlocks IntBlocks { get; } = new IntBlocks();
    }
}
