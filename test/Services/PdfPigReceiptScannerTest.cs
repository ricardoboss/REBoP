using REBoP.Services;

namespace REBoP.Test.Services;

public static class PdfPigReceiptScannerTest
{
    [Test]
    public static async Task TestScansExamplePdf()
    {
        var pdf = new FilesystemPdfSource(new("Examples/6.pdf"));
        var scanner = new PdfPigReceiptScanner();

        var lines = await scanner.ScanAsync(pdf);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lines.Lines.Count(), Is.EqualTo(84));
            Assert.That(lines.Source, Is.SameAs(pdf));
        }
    }
}
