using CognitiveServicesVision.Models;
using CognitiveServicesVision.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CognitiveServicesVision.Pages
{
	public class IndexModel : PageModel
	{
		private static class CacheKeys
		{
			public const string Images = "img";
		}
		private const string RootFolderName = "wwwroot\\";
		private const string ImagesFolderName = RootFolderName + "images";

		private IConfiguration Configuration { get; }
		private IMemoryCache MemoryCache { get; }
		public string MessageText { get; private set; } = "null";
		public string ImageFilename { get; private set; } = "ImagePlaceholder.png";
		public ImageInfo ImageInformation { get; set; } = new ImageInfo() { Percentage = "", Description = "" };
		public int ImageIndex { get; private set; } = 1;

		public IndexModel(IConfiguration configuration, IMemoryCache cache)
		{
			Configuration = configuration;
			MemoryCache = cache;
		}


		public async Task OnGet(int? index)
		{
			if (index.HasValue == false || index.Value < 1) return;

			string filename = null;
			var images = GetListOfImages();

			if (images != null && images.Length > 0)
			{
				if (index < images.Length) ImageIndex = index.Value + 1;
				else ImageIndex = 1;

				if (--index >= images.Length) index = 0;
				filename = images[index.Value];
				ImageFilename = filename.Replace(RootFolderName, String.Empty);
			}
			if (ImageIndex != 0 && String.IsNullOrEmpty(filename) == false)
			{
				string subscriptionKey = Configuration.GetValue<string>("SubscriptionKey");
				string serviceEndpoint = Configuration.GetValue<string>("ServiceEndpoint");
				MessageText = await Vision.AnalyzeImage(filename, subscriptionKey, serviceEndpoint);
			}

			ShowConfidenceAndDescription(MessageText);
		}

		private string[] GetListOfImages()
		{
			string[] images = MemoryCache.Get<string[]>(CacheKeys.Images);
			if (images == null)
			{
				images = Directory.GetFiles(ImagesFolderName);
				MemoryCache.Set(CacheKeys.Images, images);
			}
			return images;
		}

		private void ShowConfidenceAndDescription(string text)
		{
			if (string.IsNullOrEmpty(text) == false)
			{
				try
				{
					var data = Newtonsoft.Json.Linq.JObject.Parse(text);
					var captions = data["description"]["captions"];
					ImageInformation.Description = (string)captions[0]["text"];
					ImageInformation.Percentage = string.Format("{0:N2}%", ((double)captions[0]["confidence"] * 100));
				}
				catch
				{
					// TODO: handle exception here
				}
			}
		}
	}
}
