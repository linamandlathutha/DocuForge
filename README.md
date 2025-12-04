# PdfCompressor

PdfCompressor is an ASP.NET Core web application (targeting .NET 8) that provides tools to handle PDF files from a web UI. The workspace contains a Razor Pages–style web app (Views, Controllers and Models are present) and is intended to run in Visual Studio 2022.

## Current purpose
- Upload, inspect and compress PDF files from a browser.
- Provide a simple UI (Views/PdfCompressor/Index.cshtml and PdfCompressorController) for file upload and results.

## Planned next features (priority)
1. PDF combining (merge multiple PDFs into a single PDF)
2. PDF separating (split a multi-page PDF into single-page PDFs or configurable ranges)
3. Word ↔ PDF conversion (DOCX to PDF and PDF to DOCX where feasible)
4. Image → PDF conversion (PNG, JPG, TIFF, etc. → single- or multi-page PDF)

## Suggested API and UI surface
- POST /PdfCompressor/Upload — single-file upload for compression/inspection
- POST /PdfCompressor/Merge — multipart/form-data: multiple PDF files; returns merged PDF stream
- POST /PdfCompressor/Split — file + split options (single pages, ranges); returns ZIP with output PDFs
- POST /PdfCompressor/ConvertToPdf — accepts DOCX or images; returns PDF stream
- POST /PdfCompressor/ConvertToWord — accepts PDF; returns DOCX (best-effort; may be limited for complex layouts)

## Implementation notes and recommendations
- Libraries
  - Merge / Split / Create: PdfSharpCore or PdfPig (MIT-friendly options). iText7 is powerful but AGPL/commercial — check licensing for distribution.
  - Word handling: Open XML SDK (for DOCX manipulation). For reliable DOCX→PDF conversion consider calling LibreOffice in headless mode or using a commercial converter (Aspose.Words, GroupDocs) if licensing permits.
  - Image → PDF: ImageSharp + PdfSharpCore or System.Drawing.Common (careful with cross-platform issues).
- Conversion approaches
  - Server-side headless conversion (LibreOffice) is reliable for complex DOCX → PDF: run LibreOffice in headless mode on the server and stream results.
  - PDF → DOCX is lossy and limited; prefer exporting text and images, then reconstructing a DOCX (or offer OCR-based paths for scanned PDFs).
- Storage & streaming
  - Avoid long-lived storage of uploaded files. Use memory streams for small files and a temp directory with cleanup for large files.
  - For split outputs, return a ZIP archive when multiple files are produced.
- Security
  - Enforce file size limits and extension/content-type checks.
  - Sanitize filenames and store files in isolated temp folders.
  - Scan or validate uploaded content if possible.
- Performance & reliability
  - Use background processing for long-running conversions (e.g., a queue / background service).
  - Provide progress/reporting on long operations if needed.
- Testing
  - Add unit tests for small pure functions and integration tests for conversion flows.
  - Add end-to-end tests for upload → convert → download scenarios.

## Developer workflow (Visual Studio 2022)
- Open solution in Visual Studio 2022.
- Set the web project as the startup project.
- Build and run using __Build Solution__ then __Debug > Start Debugging__ or __Debug > Start Without Debugging__.
- Add NuGet packages for selected libraries and register any required services in Program.cs.

## Next actionable tasks (short)
1. Choose primary conversion libraries (open-source vs commercial) and add corresponding NuGet packages.
2. Implement controller actions and views for Merge, Split, ConvertToPdf, ConvertToWord, and ImageToPdf.
3. Implement temp file management and stream responses for downloads.
4. Add server-side validation and file size limits, update UI to accept multiple files where required.
5. Add unit/integration tests and CI step to validate conversions.

## License & notes
- Be mindful of third-party licensing (especially iText/Aspose).
- Document any external dependencies (e.g., LibreOffice) required on the host machine.
