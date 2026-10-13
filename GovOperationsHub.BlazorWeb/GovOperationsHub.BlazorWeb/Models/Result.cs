namespace GovOperationsHub.BlazorWeb.Models
{
    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
