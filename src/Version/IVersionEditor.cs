namespace Version
{
    internal interface IVersionEditor
    {
        System.Version Version { get; set; }
        string Namespace { get; set; }

        string TransformText();
    }
}