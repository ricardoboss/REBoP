# Rewe E-Bon Processor (REBoP)

Read your Rewe e-bon PDFs using C#!

## Features

> The component doing all the heavy work is [`ReweReceiptLineInterpreter`].
> It can interpret the most important stuff, but is nowhere near to reading everything that is available.
>
> It's also pretty fragile, so newer e-bon layouts might break it.
> Support this project by providing your own examples!
> See [CONTRIBUTING.md].

This library can read the following details off of a REWE receipt:

* market details:
  * address
  * VAT-ID
  * REWE market ID
* receipt metadata:
  * total
  * date & time
  * receipt number
  * (if available) trace number
* receipt items:
  * line number
  * label
  * line total
  * quantity bought
  * (if available) unit prices
  * (if available) unit (e.g. "kg", "Stk")
  * tax bracket applied
  * whether or not it qualifies for discounts and REWE bonus points
  * (if available) concessionaire (e.g. "X01" when buying from eat happy ToGo GmbH inside REWE)
* tax details:
  * tax brackets used on the receipt
  * amounts paid per bracket in net, tax and gross
  * (if applicable) tax details for concessionaires

What it can't read:

* images
* payment details (e.g. payment method, last 4 digits of card number, security info...)
* PAYBACK/REWE Bonus information
* additional text below the receipt (special offers, ads)

## Installation

```powershell
dotnet add package REBoP
```

## License

MIT. For full license, see [LICENSE.md].

## Disclaimer

This project is not affiliated with REWE Group or any of its retailers.

[`ReweReceiptLineInterpreter`]: ./src/Services/ReweReceiptLineInterpreter.cs
[CONTRIBUTING.md]: ./CONTRIBUTING.md
[LICENSE.md]: ./LICENSE.md
