using REBoP.Services;

namespace REBoP.Test.Services;

public static class ReweReceiptLineInterpreterTest
{
    [Test]
    public static async Task TestInterpretsLines()
    {
        var file = new FileInfo("Examples/6.pdf");
        var pdf = new FilesystemPdfSource(file);
        var content = new ListReceiptContent(pdf, [
            " ",
            " ",
            " ",
            "               R E W E               ",
            "         Kulenkampffallee 184         ",
            "             28213 Bremen             ",
            "       Telefon: 0421 / 2010554       ",
            "         UID Nr.: DE812706034         ",
            "                                   EUR",
            "GEFLUEGEL SALAMI                 1,95 B",
            "JA! GOUDA JUNG                   2,99 B",
            "ZWIEBELBROT                      1,15 B",
            "FLEISCHSALAT KR.                 1,89 B",
            "VITAL                            1,99 B",
            "BIO H-MILCH 1,5%                 2,30 B",
            "             2 Stk x    1,15",
            "MONSTER ZERO                     1,54 A",
            "             2 Stk x    0,77",
            "PFAND 0,25 EURO                  0,50 A *",
            "             2 Stk x    0,25",
            " --------------------------------------",
            " SUMME                   EUR     14,31",
            " ======================================",
            " Geg. Mastercard         EUR     14,31",
            "                    ",
            "          * *  Kundenbeleg  * *          ",
            "Datum:                        18.07.2025",
            "Uhrzeit:                    21:42:34 Uhr",
            "Beleg-Nr.                           2413",
            "Trace-Nr.                         161891",
            "                Bezahlung                ",
            "               Contactless               ",
            "             DEBIT MASTERCARD             ",
            "Nr.                ############2138 0000",
            "VU-Nr.                        4556082320",
            "Terminal-ID                     56053158",
            "Pos-Info                       00 075 00",
            "AS-Zeit 18.07.                 21:42 Uhr",
            "        AS-Proc-Code = 00 075 00",
            "            Capt.-Ref.= 0000",
            "                APPROVED",
            "Betrag EUR                         14,31",
            "             Zahlung erfolgt             ",
            "                 Approved                 ",
            "                    ",
            " Steuer  %          Netto        Steuer        Brutto",
            " A=  19,0%           1,71          0,33          2,04",
            " B=   7,0%          11,47          0,80         12,27",
            " Gesamtbetrag       13,18          1,13         14,31",
            "                    ",
            "TSE-Signatur:        E7sF6H+t9wctHrMEh+XX88llteNoGdCKR",
            "                     c4lKT3qRiGx0D0GOQVj27QfmQJWdffJPh",
            "                     zTDplx+x/xe5QXUAilVCn+5501fAOfhAm",
            "                     HzuOKxNnxDtsJEERscmMTRee7oqmh    ",
            "        TSE-Signaturzähler:  3586457         ",
            "        TSE-Transaktion:     1723013         ",
            "    TSE-Start:           2025-07-18T21:41:36.000 ",
            "    TSE-Stop:            2025-07-18T21:42:41.000 ",
            "    Seriennnummer Kasse: REWE:74:56:3c:83:9b:00:00",
            "  18.07.2025     21:42     Bon-Nr.:2140",
            "  Markt:4213     Kasse:2   Bed.:432102",
            "                    ",
            " ****************************************",
            "Entdecke und aktiviere alle REWE Bonus-",
            "    Vorteile jetzt in der REWE App!",
            "                    ",
            "   Aktuelles Bonus-Guthaben: 4,34 EUR",
            "  Sammle noch mehr REWE Bonus-Guthaben",
            "   mit Coupons und weiteren Vorteilen",
            "  - gleich in der REWE App aktivieren!",
            "                    ",
            "   Keine Rabatte oder Bonus-Guthaben",
            "   auf mit * gekennzeichnete Produkte",
            "                    ",
            " ****************************************",
            "                    ",
            "            REWE Markt GmbH           ",
            "                                      ",
            "     Vielen Dank für Ihren Einkauf    ",
            "   Bitte beachten Sie unsere kunden-  ",
            "                                      ",
            "            Sie haben Fragen           ",
            "      Antworten gibt es auch unter     ",
            "              www.rewe.de             ",
        ]);
        var interpreter = new ReweReceiptLineInterpreter();

        var receipt = await interpreter.ProcessAsync(content);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(receipt.Market.Address, Does.StartWith("R E W E"));
            Assert.That(receipt.TaxTotal.Net, Is.EqualTo(13.18m));
            Assert.That(receipt.Total, Is.EqualTo(14.31m));
            Assert.That(receipt.Timestamp,
                Is.EqualTo(new DateTimeOffset(2025, 7, 18, 21, 42, 34, TimeSpan.FromHours(2))));
            Assert.That(receipt.TaxDetails, Has.Count.EqualTo(2));
            Assert.That(receipt.Items, Has.Count.EqualTo(8));
        }

        using (Assert.EnterMultipleScope())
        {
            var firstTaxItem = receipt.TaxDetails.First();
            Assert.That(firstTaxItem.BracketAlias.Alias, Is.EqualTo("A"));
            Assert.That(firstTaxItem.BracketAlias.Bracket.Multiplier, Is.EqualTo(0.19m));
            Assert.That(firstTaxItem.Net, Is.EqualTo(1.71));
            Assert.That(firstTaxItem.Tax, Is.EqualTo(0.33));
            Assert.That(firstTaxItem.Gross, Is.EqualTo(2.04));

            var firstItem = receipt.Items.First();
            Assert.That(firstItem.LineNumber, Is.EqualTo(1));
            Assert.That(firstItem.Label, Is.EqualTo("GEFLUEGEL SALAMI"));
            Assert.That(firstItem.Quantity, Is.EqualTo(1));
            Assert.That(firstItem.Unit, Is.Null);
            Assert.That(firstItem.UnitPrice, Is.Null);
            Assert.That(firstItem.LineTotal, Is.EqualTo(1.95m));
            Assert.That(firstItem.TaxBracket.Alias, Is.EqualTo("B"));
            Assert.That(firstItem.IsNonDiscountable, Is.False);
            Assert.That(firstItem.PartnerCode, Is.Null);

            var lastItem = receipt.Items.Last();
            Assert.That(lastItem.LineNumber, Is.EqualTo(8));
            Assert.That(lastItem.Label, Is.EqualTo("PFAND 0,25 EURO"));
            Assert.That(lastItem.Quantity, Is.EqualTo(2));
            Assert.That(lastItem.Unit, Is.EqualTo("Stk"));
            Assert.That(lastItem.UnitPrice, Is.EqualTo(0.25m));
            Assert.That(lastItem.LineTotal, Is.EqualTo(0.5m));
            Assert.That(lastItem.TaxBracket.Alias, Is.EqualTo("A"));
            Assert.That(lastItem.IsNonDiscountable, Is.True);
            Assert.That(lastItem.PartnerCode, Is.Null);
        }
    }
}
