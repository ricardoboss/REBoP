# Unreleased

* Added support for reading payment items

# 1.0.2

* Fixed some warnings related to parsing
* Fixed an issue where the whole receipt was treated as the market address

# 1.0.1

* Added GitHub Actions workflows (build & publish)
* Widened return type for `IPdfSource.OpenRead` from `FileStream` to `Stream` to allow more generic implementations
* Include `PartnerCode` in debugger display for `TaxDetailItem` and `TaxTotalItem`

# 1.0.0

* Initial release
* Provides services for scanning REWE receipt PDFs and interpreting them to generate usable data structures
