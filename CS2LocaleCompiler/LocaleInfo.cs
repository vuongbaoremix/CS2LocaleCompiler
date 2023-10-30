namespace CS2LocaleCompiler
{
    public class LocaleInfo
	{ 
		public ushort version { set; get; }
		public string localeId { get; }
		 
		public string systemLanguage { get; }
		 
		public string localizedName { get; }
		 
		public Dictionary<string, string> entries { get; }
		 
		public Dictionary<string, int> indexCounts { get; } 

		public LocaleInfo(ushort version, string localeId, string systemLanguage, string localizedName, Dictionary<string, string> entries, Dictionary<string, int> indexCounts)
		{
			this.version = version;
			this.systemLanguage = systemLanguage;
			this.localeId = localeId;
			this.entries = entries;
			this.localizedName = localizedName;
			this.indexCounts = indexCounts;
		}
	}
}
