using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class FirebaseService
{
    private readonly string _bucket = "keepaneye5b.appspot.com";
    private readonly string _uploadUrl = "https://firebasestorage.googleapis.com/v0/b/{0}/o?uploadType=multipart";
    private readonly string _downloadUrl = "https://firebasestorage.googleapis.com/v0/b/{0}/o/{1}?alt=media";

    public async Task<string> UploadFileToFirebaseAsync(Stream fileStream, string fileName)
    {
        using var httpClient = new HttpClient();
        var url = string.Format(_uploadUrl, _bucket);

        var requestContent = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        requestContent.Add(fileContent, "file", fileName);

        var response = await httpClient.PostAsync(url, requestContent);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var downloadUrl = string.Format(_downloadUrl, _bucket, Uri.EscapeDataString(fileName));

        return downloadUrl;
    }
}
