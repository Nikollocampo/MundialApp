namespace MundialApp.Services;

public sealed class PdfViewerService
{
    public string? CurrentDocumentName { get; private set; }
    public string? CurrentDocumentBase64 { get; private set; }
    public bool HasDocument => !string.IsNullOrWhiteSpace(CurrentDocumentBase64);

    public event Action? Changed;

    public void Show(string fileName, byte[] bytes)
    {
        CurrentDocumentName = fileName;
        CurrentDocumentBase64 = Convert.ToBase64String(bytes);
        Changed?.Invoke();
    }

    public void Clear()
    {
        CurrentDocumentName = null;
        CurrentDocumentBase64 = null;
        Changed?.Invoke();
    }
}
