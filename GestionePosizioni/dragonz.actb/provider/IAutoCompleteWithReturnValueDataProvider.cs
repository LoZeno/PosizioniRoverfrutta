namespace dragonz.actb.provider
{
    public interface IAutoCompleteWithReturnValueDataProvider : IAutoCompleteDataProvider
    {
        object GetValue(string selectedText);
    }
}
