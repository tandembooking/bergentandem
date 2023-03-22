// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
//TandemBooking.Services.ContentService
using Newtonsoft.Json;
using System.IO;
using TandemBooking.Services;

public class ContentService : IContentService
{
	private dynamic _contentEn;

	private dynamic _contentNo;

	public dynamic content;

	public dynamic LoadJson(string filename)
	{
		using (StreamReader r = new StreamReader(filename))
		{
			string json = r.ReadToEnd();
			return JsonConvert.DeserializeObject(json);
		}
	}

	public ContentService(string lang = "NO")
	{
		_contentEn = LoadJson("PageContent/bergen-en.json");
		_contentNo = LoadJson("PageContent/bergen-no.json");
		content = _contentEn;
	}

	public void setLanguage(string st)
	{
		content = ((st == "EN") ? _contentEn : _contentNo);
	}
}
