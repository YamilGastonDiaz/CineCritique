using OpenAI;
using OpenAI.Chat;

namespace CineCritique.Service
{
    public class LlmService
    {
        private readonly string _apiKey;
        private readonly string _model;

        public LlmService()
        {
            _apiKey = Environment.GetEnvironmentVariable("GroqApiKey")
                ?? throw new InvalidOperationException("La variable de entorno 'GroqApiKey' no está configurada.");
            _model = "llama-3.3-70b-versatile";
        }

        public async Task<string> ObtenerSpoilerAsync(string tituloPelicula)
        {
            if (string.IsNullOrWhiteSpace(tituloPelicula))
                throw new ArgumentException("El título de la película no puede estar vacío.", nameof(tituloPelicula));

            string prompt = $@"Genera un pequeño spoiler (máximo 2-3 oraciones) sobre la película ""{tituloPelicula}"". 
                        El spoiler debe revelar algún giro interesante de la trama sin arruinar completamente la experiencia. 
                        Sé conciso y cautivador.";

            return await ConsultarLlmAsync(prompt);
        }


        public async Task<string> ObtenerResumenAsync(string tituloPelicula)
        {
            if (string.IsNullOrWhiteSpace(tituloPelicula))
                throw new ArgumentException("El título de la película no puede estar vacío.", nameof(tituloPelicula));

            string prompt = $@"Proporciona un resumen breve (máximo 3-4 oraciones) de la película ""{tituloPelicula}"". 
                        Incluye el género, la premisa principal y por qué es relevante o interesante. 
                        No incluyas spoilers importantes.";

            return await ConsultarLlmAsync(prompt);
        }


        private async Task<string> ConsultarLlmAsync(string prompt)
        {
            try
            {
                // 1. Configuramos el endpoint apuntando a Groq
                OpenAIClientOptions options = new()
                {
                    Endpoint = new Uri("https://api.groq.com/openai/v1")
                };

                // 2. Inicializamos directamente el ChatClient pasándole el modelo, la clave y las opciones.
                // Esta es la sobrecarga más limpia y compatible del SDK para desvíos de Endpoint.
                System.ClientModel.ApiKeyCredential credential = new(_apiKey);
                ChatClient client = new(model: _model, credential: credential, options: options);

                // 3. Realizamos la consulta enviando el prompt al entorno de Groq
                ChatCompletion completion = await client.CompleteChatAsync(prompt);

                // 4. Retornamos el texto de la respuesta usando la propiedad estándar Content
                return completion.Content[0].Text?.Trim() ?? "No se pudo obtener una respuesta.";
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Error de conexión con la API de OpenAI: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al procesar la solicitud: {ex.Message}", ex);
            }
        }

        public async Task<string> ConsultaSimpleAsync(string pregunta)
        {
            if (string.IsNullOrWhiteSpace(pregunta))
                throw new ArgumentException("La pregunta no puede estar vacía.", nameof(pregunta));

            return await ConsultarLlmAsync(pregunta);
        }
    }
}
