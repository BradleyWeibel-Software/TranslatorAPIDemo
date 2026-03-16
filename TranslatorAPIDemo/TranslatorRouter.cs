
namespace TranslatorAPIDemo
{
    public class TranslatorRouter
    {
        private readonly IEnumerable<ITranslator> _translators;

        public TranslatorRouter(IEnumerable<ITranslator> translators)
        {
            _translators = translators;
        }

        public ITranslator Resolve(string targetLanguage)
        {
            var translator = _translators.FirstOrDefault(t => t.CanTranslate(targetLanguage));

            if (translator == null)
                throw new InvalidOperationException("No translator registered for " + targetLanguage);

            return translator;
        }
    }
}