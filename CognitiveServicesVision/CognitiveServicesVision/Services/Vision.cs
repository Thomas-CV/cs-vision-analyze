using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CognitiveServicesVision.Services
{
	public static class Vision
	{
		#region AnalyzeEmotions
		public static async Task<string> AnalyzeImage(string filename, string subscriptionKey, string serviceEndpoint)
		{
			try
			{
				HttpResponseMessage response;

				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
				string uri = serviceEndpoint + "analyze?visualFeatures=Categories,Tags,Description,ImageType,Color,Adult";

				byte[] image = GetImageAsByteArray(filename);

				using (ByteArrayContent content = new ByteArrayContent(image))
				{
					content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
					response = await client.PostAsync(uri, content);
				}
				return await response.Content.ReadAsStringAsync();
			}
			catch
			{
				throw;  // TODO: handle exception here
			}
		}
		#endregion


		#region GetImageAsByteArray
		private static byte[] GetImageAsByteArray(string filename)
		{
			using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				BinaryReader reader = new BinaryReader(stream);
				return reader.ReadBytes((int)stream.Length);
			}
		}
		#endregion
	}
}
