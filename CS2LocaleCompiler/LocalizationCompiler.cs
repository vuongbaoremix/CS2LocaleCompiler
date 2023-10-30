namespace CS2LocaleCompiler
{
    internal class LocalizationCompiler
    {
        public static LocaleInfo ReadLocale(string path)
        {
            if (Path.GetExtension(path) == ".json")
                return ReadLocaleFromJson(path);


            ushort version;
            string systemLanguage;
            string localeId;
            string localizedName;
            Dictionary<string, int> indexCounts = new Dictionary<string, int>();
            Dictionary<string, string> entries = new Dictionary<string, string>();

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    // Read the values from the file
                    version = binaryReader.ReadUInt16();
                    systemLanguage = binaryReader.ReadString();
                    localeId = binaryReader.ReadString();
                    localizedName = binaryReader.ReadString();
                    int entryCount = binaryReader.ReadInt32();

                    for (int i = 0; i < entryCount; i++)
                    {
                        string fullIdentifier = binaryReader.ReadString();
                        string value = binaryReader.ReadString();
                        entries.Add(fullIdentifier, value);
                    }

                    int indexCount = binaryReader.ReadInt32();
                    for (int i = 0; i < indexCount; i++)
                    {
                        string key = binaryReader.ReadString();
                        int value = binaryReader.ReadInt32();

                        indexCounts.Add(key, value);
                    }

                    // Now, you have all the data read from the file.
                    // You can use the 'version', 'systemLanguage', 'localeId', 'localizedName', 'entries', and 'indexCounts' as needed.
                }
            }

            return new LocaleInfo(version, localeId, systemLanguage, localizedName, entries, indexCounts);
        }

        public static LocaleInfo ReadLocaleFromJson(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                throw new FileNotFoundException($"File \"{Path.GetFileName(path)}\" not found");

            var source = File.ReadAllText(path);

            return System.Text.Json.JsonSerializer.Deserialize<LocaleInfo>(source, new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        public static void WriteLocale(string path, LocaleInfo info)
        {
            if (Path.GetExtension(path) == ".json")
            {
                WriteLocaleToJson(path, info);
                return;
            }
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(info.version);
                    binaryWriter.Write(info.systemLanguage);
                    binaryWriter.Write(info.localeId);
                    binaryWriter.Write(info.localizedName);

                    var entries = info.entries.Where(entry => !string.IsNullOrEmpty(entry.Value));
                    binaryWriter.Write(entries.Count());

                    foreach (var localizationEntry in entries)
                    {
                        binaryWriter.Write(localizationEntry.Key);
                        binaryWriter.Write(localizationEntry.Value);
                    }

                    binaryWriter.Write(info.indexCounts.Count);
                    foreach (var keyValuePair in info.indexCounts)
                    {
                        binaryWriter.Write(keyValuePair.Key);
                        binaryWriter.Write(keyValuePair.Value);
                    }
                }
            }
        }

        public static void WriteLocaleToJson(string path, LocaleInfo info)
        {
            var source = System.Text.Json.JsonSerializer.Serialize(info, new System.Text.Json.JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            File.WriteAllText(path, source);
        }

        public static bool Verify(LocaleInfo source, LocaleInfo target)
        {
            bool isValid = true;

            foreach (var item in source.indexCounts)
            {
                if (!target.indexCounts.ContainsKey(item.Key))
                {
                    Console.WriteLine($"Missing Index: \"{item.Key}\":\"{item.Value}\"");
                    isValid = false;
                }
            }

            foreach (var item in target.indexCounts)
            {
                if (!source.indexCounts.ContainsKey(item.Key))
                {
                    Console.WriteLine($"Index Not available: {item.Key}= {item.Value}");
                }
            }

            foreach (var item in source.entries)
            {
                if (!target.entries.ContainsKey(item.Key))
                {
                    Console.WriteLine($"Missing Entry: \"{item.Key}\":\"{item.Value}\"");
                    isValid = false;
                }
            }

            foreach (var item in target.entries)
            {
                if (!source.entries.ContainsKey(item.Key))
                {
                    Console.WriteLine($"Entry Not available: {item.Key}= {item.Value}");
                }
            }

            return isValid;
        }
    }
}
