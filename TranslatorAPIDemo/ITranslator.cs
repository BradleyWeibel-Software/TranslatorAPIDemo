
namespace TranslatorAPIDemo
{
    public interface ITranslator
    {
        bool CanTranslate(string targetLanguage);
        Task<TranslationResult> TranslateAsync(TranslationRequest request);

        // Add these for API testing
        Task<bool> TestConnectionAsync();
        IReadOnlyList<string> SupportedLanguages { get; }
    }
}