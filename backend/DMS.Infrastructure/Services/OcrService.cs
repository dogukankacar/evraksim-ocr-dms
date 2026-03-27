using DMS.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace DMS.Infrastructure.Services;

public class OcrService : IOcrService
{
    private readonly ILogger<OcrService> _logger;
    private readonly string _tessDataPath;

    public OcrService(ILogger<OcrService> logger)
    {
        _logger = logger;
        _tessDataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");

        if (!Directory.Exists(_tessDataPath))
        {
            var projectTessData = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
            if (Directory.Exists(projectTessData))
                _tessDataPath = projectTessData;
        }
    }

    public async Task<string?> ExtractTextAsync(string filePath, string contentType)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Dosya bulunamadı: {FilePath}", filePath);
                return null;
            }

            _logger.LogInformation("OCR işlemi başlatıldı: {FilePath}, ContentType: {ContentType}", filePath, contentType);

            string? extractedText = null;

            if (contentType == "application/pdf")
            {
                extractedText = ExtractTextFromPdf(filePath);
            }
            else if (contentType.StartsWith("image/"))
            {
                extractedText = ExtractTextFromImage(filePath);
            }

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                _logger.LogInformation("OCR ile metin çıkarılamadı: {FilePath}", filePath);
                return null;
            }

            _logger.LogInformation("OCR başarılı, {CharCount} karakter çıkarıldı: {FilePath}",
                extractedText.Length, filePath);

            return await Task.FromResult(extractedText.Trim());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR işlemi sırasında hata: {FilePath}", filePath);
            return null;
        }
    }

    private string? ExtractTextFromPdf(string filePath)
    {
        try
        {
            using var document = PdfDocument.Open(filePath);
            var textParts = new List<string>();

            foreach (var page in document.GetPages())
            {
                var pageText = page.Text;
                if (!string.IsNullOrWhiteSpace(pageText))
                {
                    textParts.Add(pageText);
                }
            }

            var fullText = string.Join("\n", textParts);

            if (!string.IsNullOrWhiteSpace(fullText))
            {
                _logger.LogInformation("PDF'den doğrudan metin çıkarıldı ({CharCount} karakter)", fullText.Length);
                return fullText;
            }

            _logger.LogInformation("PDF'de gömülü metin bulunamadı, taranmış PDF olabilir");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PDF metin çıkarma hatası: {FilePath}", filePath);
            return null;
        }
    }

    private string? ExtractTextFromImage(string filePath)
    {
        try
        {
            if (!Directory.Exists(_tessDataPath) ||
                !Directory.GetFiles(_tessDataPath, "*.traineddata").Any())
            {
                _logger.LogWarning("Tessdata bulunamadı: {Path}. OCR yapılamıyor.", _tessDataPath);
                return null;
            }

            var language = File.Exists(Path.Combine(_tessDataPath, "tur.traineddata")) ? "tur+eng" : "eng";

            using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
            using var img = Pix.LoadFromFile(filePath);
            using var page = engine.Process(img);

            var text = page.GetText();
            var confidence = page.GetMeanConfidence();

            _logger.LogInformation("OCR tamamlandı. Güven: {Confidence:P0}, Dil: {Language}", confidence, language);

            return string.IsNullOrWhiteSpace(text) ? null : text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tesseract OCR hatası: {FilePath}", filePath);
            return null;
        }
    }
}
