
namespace TranslatorAPIDemo
{
    public class TranslationService
    {
        private readonly TranslatorRouter _router;

        public TranslationService(TranslatorRouter router)
        {
            _router = router;
        }

        public Task<TranslationResult> TranslateAsync(TranslationRequest request)
        {
            var translator = _router.Resolve(request.TargetLanguage);
            return translator.TranslateAsync(request);
        }
    }
}