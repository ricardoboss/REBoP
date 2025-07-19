using UglyToad.PdfPig;

namespace REBoP.Services;

public class PdfPigReceiptScanner : IReceiptScanner
{
    public async Task<IReceiptContent> ScanAsync(IPdfSource pdf, ScanOptions? options = null, CancellationToken cancellationToken = default)
    {
        var lines = await ReadLinesAsync(pdf);

        return new ListReceiptContent(pdf, lines);
    }

    private static async Task<List<string>> ReadLinesAsync(IPdfSource pdf)
    {
        await using var pdfStream = pdf.OpenRead();
        using var document = PdfDocument.Open(pdfStream);

        List<string> lines = [];
        foreach (var page in document.GetPages())
        {
            // this only works because the lines are all perfectly horizontal
            var thisPageLines = page.Letters.AggregateBy(l => l.Location.Y, "", (line, letter) => line + letter.Value).Select(kvp => kvp.Value).ToList();

            lines.AddRange(thisPageLines);
        }

        return lines;
    }
}
