using System.Text.Json;
using System.Text;

namespace MabrukBlazor2026.Client.Repository
{
    public class Repository : IRepository
    {
        private readonly HttpClient httpClient;

        public Repository(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private JsonSerializerOptions DefaultJsonOptions =>
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };


        public async Task<HttpResponseWrapper<T>> Get<T>(string url)
        {
            var httpResponse = await httpClient.GetAsync(url);
            //Console.WriteLine(url);
            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await DeserializeResponse<T>(httpResponse, DefaultJsonOptions);
                return new HttpResponseWrapper<T>(response, false, httpResponse);
            }
            else
            {
                return new HttpResponseWrapper<T>(default, true, httpResponse);
            }
        }


        public async Task<HttpResponseWrapper<string>> GetString<T>(string url)
        {
            var httpResponse = await httpClient.GetAsync(url);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseString))
                {
                    return new HttpResponseWrapper<string>(responseString, false, httpResponse);
                }
                return new HttpResponseWrapper<string>(default, true, httpResponse);
            }
            else
            {
                return new HttpResponseWrapper<string>(default, true, httpResponse);
            }
        }


        public async Task<HttpResponseWrapper<Stream>> GetFileContent(string url)
        {
            var httpResponse = await httpClient.GetAsync(url);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return new HttpResponseWrapper<Stream>(default, true, httpResponse);
            }

            var fileStream = await httpResponse.Content.ReadAsStreamAsync();
            if (fileStream != null)
            {
                return new HttpResponseWrapper<Stream>(fileStream, false, httpResponse);
            }

            return new HttpResponseWrapper<Stream>(default, true, httpResponse);
        }


        public async Task<HttpResponseWrapper<object>> Post<T>(string url, T toSend)
        {
            var JSONToSend = JsonSerializer.Serialize(toSend);
            var contentToSend = new StringContent(JSONToSend, Encoding.UTF8, "application/json");
            var responseHttp = await httpClient.PostAsync(url, contentToSend);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }


        public async Task<HttpResponseWrapper<object>> Put<T>(string url, T toSend)
        {
            var JSONToSend = JsonSerializer.Serialize(toSend);
            var contentToSend = new StringContent(JSONToSend, Encoding.UTF8, "application/json");
            var responseHttp = await httpClient.PutAsync(url, contentToSend);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }


        public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T toSend)
        {
            var JSONToSend = JsonSerializer.Serialize(toSend);
            var contentToSend = new StringContent(JSONToSend, Encoding.UTF8, "application/json");
            var responseHttp = await httpClient.PostAsync(url, contentToSend);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await DeserializeResponse<TResponse>(responseHttp, DefaultJsonOptions);
                return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
            }
            else
            {
                return new HttpResponseWrapper<TResponse>(default, true, responseHttp);
            }
        }


        public async Task<HttpResponseWrapper<TResponse>> PostFile<TResponse>(string url, MultipartFormDataContent toSend)
        {
            var responseHttp = await httpClient.PostAsync(url, toSend);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await DeserializeResponse<TResponse>(responseHttp, DefaultJsonOptions);
                return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
            }
            else
            {
                return new HttpResponseWrapper<TResponse>(default, true, responseHttp);
            }
        }


        public async Task<HttpResponseWrapper<object>> Delete(string url)
        {
            var responseHTTP = await httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, !responseHTTP.IsSuccessStatusCode, responseHTTP);
        }


        private async Task<T> DeserializeResponse<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            //Console.WriteLine(responseString);
            return JsonSerializer.Deserialize<T>(responseString, jsonSerializerOptions);
        }
    }
}
