using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ConvertToPDFController : ControllerBase
{
	private readonly IConverter _converter;
	private readonly IWebHostEnvironment _env;

	public ConvertToPDFController(IConverter converter, IWebHostEnvironment env)
	{
		_converter = converter;
		_env = env;
	}

	[HttpPost]
	public IActionResult ConvertHtmlToPdf()
	{
		string rootPath = _env.ContentRootPath;
		string htmlFilePath = Path.Combine(rootPath, "index.html");
		string outputDirectory = Path.Combine(rootPath, "output");

		if (!Directory.Exists(outputDirectory))
		{
			Directory.CreateDirectory(outputDirectory);
		}

		string htmlContent = System.IO.File.ReadAllText(htmlFilePath);

		string htmlOutputPath = Path.Combine(outputDirectory, "document.html");
		System.IO.File.WriteAllText(htmlOutputPath, htmlContent);

		var doc = new HtmlToPdfDocument()
		{
			GlobalSettings = {
				PaperSize = PaperKind.A4,
				Orientation = Orientation.Portrait
			},
			Objects = {
				new ObjectSettings() {
					HtmlContent = htmlContent,
					WebSettings = {
						DefaultEncoding = "utf-8",
						EnableJavascript = true,
						LoadImages = true,
					},
				}
			}
		};

		byte[] pdf = _converter.Convert(doc);
		string pdfOutputPath = Path.Combine(outputDirectory, "document.pdf");
		System.IO.File.WriteAllBytes(pdfOutputPath, pdf);

		return Ok(new { HtmlPath = htmlOutputPath, PdfPath = pdfOutputPath });
	}

	[HttpGet("download/html")]
	public IActionResult DownloadHtml()
	{
		string rootPath = _env.ContentRootPath;
		string htmlOutputPath = Path.Combine(rootPath, "output", "document.html");

		if (!System.IO.File.Exists(htmlOutputPath))
		{
			return NotFound();
		}

		byte[] fileBytes = System.IO.File.ReadAllBytes(htmlOutputPath);
		return File(fileBytes, "text/html", "document.html");
	}

	[HttpGet("download/pdf")]
	public IActionResult DownloadPdf()
	{
		string rootPath = _env.ContentRootPath;
		string pdfOutputPath = Path.Combine(rootPath, "output", "document.pdf");

		if (!System.IO.File.Exists(pdfOutputPath))
		{
			return NotFound();
		}

		byte[] fileBytes = System.IO.File.ReadAllBytes(pdfOutputPath);
		return File(fileBytes, "application/pdf", "document.pdf");
	}
}
