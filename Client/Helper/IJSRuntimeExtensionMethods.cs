using Microsoft.JSInterop;

namespace MabrukBlazor2026.Client.Helper
{
    public static class IJSRuntimeExtensionMethods
    {

        /* SAVING DATA TO SESSION STORAGE */
        public static ValueTask<object> SaveInSessionStorage(this IJSRuntime js, string key, string content)
        {
            return js.InvokeAsync<object>("sessionStorage.setItem", key, content);
        }


        public static ValueTask<object> GetFromSessionStorage(this IJSRuntime js, string key)
        {
            return js.InvokeAsync<object>("sessionStorage.getItem", key);
        }


        public static ValueTask<object> RemoveFromSessionStorage(this IJSRuntime js, string key)
        {
            return js.InvokeAsync<object>("sessionStorage.removeItem", key);
        }


        public static async ValueTask<string> GetCurrentCulture(this IJSRuntime js)
        {
            return await js.InvokeAsync<string>("getCulture");
        }

    }
}
