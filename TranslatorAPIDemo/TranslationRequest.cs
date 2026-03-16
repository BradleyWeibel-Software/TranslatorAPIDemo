namespace TranslatorAPIDemo
{
    public class TranslationRequest
    {
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public string Text { get; set; }
    }
}