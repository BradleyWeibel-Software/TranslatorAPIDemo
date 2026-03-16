using Microsoft.AspNetCore.Mvc;

namespace TranslatorAPIDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslateController : ControllerBase
    {
        private readonly ITranslator _translator;
        private readonly TranslationService _service;
        public TranslateController()
        {
            _translator = new StandardLanguageTranslator();
            var router = new TranslatorRouter(new ITranslator[] { _translator });
            _service = new TranslationService(router);
        }

        // GET: https://localhost:7032/api/translate
        [HttpGet]
        public string GetSingleTranslatedText()
        {
            return "Translate this...";
        }

        // POST: https://localhost:7032/api/translate/test
        [HttpPost]
        [Route("test")]
        public async Task<string> Translate()
        {
            TranslationRequest request = new TranslationRequest
            {
                Text = "Given authority to go fourth and get things done",
                SourceLanguage = "en",
                TargetLanguage = "de"
            };

            if (request == null || string.IsNullOrEmpty(request.Text))
                return "Error: Text is required";

            var result = await _service.TranslateAsync(request);

            return result.TranslatedText;
        }

        // POST: https://localhost:7032/api/translate/translatetext
        // Body > raw > JSON:
        //{
        //    "sourceLanguage": "de",
        //    "targetLanguage": "en",
        //    "text": "Guten Morgen Welt. Komm schon, lass uns auf laufen gehen."
        //}
        [HttpPost]
        [Route("translatetext")]
        public async Task<string> TranslateText([FromBody] TranslationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Text))
                return "Error: Text is required";

            var result = await _service.TranslateAsync(request);

            return result.TranslatedText;
        }
    }
}
