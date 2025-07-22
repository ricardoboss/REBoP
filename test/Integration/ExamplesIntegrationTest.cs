using REBoP.Services;

namespace REBoP.Test.Integration;

public static class ExamplesIntegrationTest
{
    [Theory]
    [TestCaseSource(nameof(ExamplePathsProvider))]
    public static async Task TestGetReceiptFromExample(FileInfo file)
    {
        var pdf = new FilesystemPdfSource(file);
        var scanner = new PdfPigReceiptScanner();
        var interpreter = new ReweReceiptLineInterpreter();

        var content = await scanner.ScanAsync(pdf);
        var receipt = await interpreter.ProcessAsync(content);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(receipt.Total, Is.GreaterThan(1m));
            Assert.That(receipt.Items, Has.Count.GreaterThan(1));
            Assert.That(receipt.TaxDetails, Has.Count.GreaterThanOrEqualTo(1));
        }

        // FIXME: doesn't work when payment is split between, say, card and some other benefit;
        //        only the card payment has tax details
        // Assert.That(receipt.TaxTotal.Gross + receipt.PartnerTaxTotals.Sum(t => t.Gross), Is.EqualTo(receipt.Total));

        using (Assert.EnterMultipleScope())
        {
            var itemTotal = receipt.Items.Sum(i => i.LineTotal);
            Assert.That(itemTotal, Is.EqualTo(receipt.Total),
                $"Sum of items ({itemTotal:C}) doesn't match receipt total ({receipt.Total:C})");

            var paidTotal = receipt.Payment.Sum(i =>
            {
                var total = i.Total;

                if (!i.IsPaying)
                    total *= -1;

                return total;
            });
            Assert.That(paidTotal, Is.EqualTo(receipt.Total),
                $"Sum of payment items ({paidTotal:C}) doesn't match receipt total ({receipt.Total:C})");

            var lineNumbers = receipt.Items.Select(i => i.LineNumber).ToList();
            Assert.That(lineNumbers, Is.EquivalentTo(lineNumbers.Distinct()), "Line numbers are not unique");

            foreach (var multiItem in receipt.Items.Where(i => i.Quantity != 1m))
            {
                Assert.That(multiItem.UnitPrice, Is.Not.Null, "Missing unit price for item with non-1 quantity");

                var calculatedLineTotal = multiItem.Quantity * multiItem.UnitPrice.Value;

                // if item is Pfand or some other kind of discount, the unit price is positive, but actually must be
                // deducted from the total
                if (multiItem.LineTotal < 0m)
                    calculatedLineTotal *= -1m;

                Assert.That(calculatedLineTotal, Is.EqualTo(multiItem.LineTotal),
                    $"Calculated line total ({calculatedLineTotal:C}) doesn't match expected line total ({multiItem.LineTotal:C})");
            }
        }
    }

    private static IEnumerable<FileInfo> ExamplePathsProvider =>
        new DirectoryInfo("Examples/").EnumerateFiles("*.pdf", SearchOption.AllDirectories);
}
