namespace MabrukBlazor2026.Client.Repository
{
    public interface IRepository
    {
        Task<HttpResponseWrapper<object>> Delete(string url);
        Task<HttpResponseWrapper<T>> Get<T>(string url);
        Task<HttpResponseWrapper<Stream>> GetFileContent(string url);
        Task<HttpResponseWrapper<string>> GetString<T>(string url);
        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T toSend);
        Task<HttpResponseWrapper<object>> Post<T>(string url, T toSend);
        Task<HttpResponseWrapper<TResponse>> PostFile<TResponse>(string url, MultipartFormDataContent toSend);
        Task<HttpResponseWrapper<object>> Put<T>(string url, T toSend);
    }
}
