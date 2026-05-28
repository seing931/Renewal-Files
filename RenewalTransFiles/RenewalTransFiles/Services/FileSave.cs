using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RenewalTransFiles.Services
{
    public interface IFileSave
    {
        Task SaveAsBase64(string filename, string base64str, string type = "application/zip");
    }
    public class FileSave : IFileSave
    {
        private IJSRuntime JSRuntime { get; }

        public FileSave(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
        }
        public async Task SaveAsBase64(string filename, string base64str, string type = "application/zip")
        {
            await JSRuntime.InvokeVoidAsync("BlazorFileSaver.saveAsBase64", filename, base64str, type);
        }
    }
}
