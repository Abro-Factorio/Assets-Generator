using System.Text.Json;
using System.Text.Json.Serialization;

namespace Textures_Generator
{
	public class Settings
	{
		[JsonInclude]
		public int hues = 5; // количество оттенков для генерации. Чем меньше значение, тем меньше вариаций будет сгенерировано, но тем более явные будут различия.
		[JsonInclude] 
		public int shades = 5; // количество степеней затенения.
		[JsonInclude]
		public float baseHue = 0; // базовый сдвиг значения цвета. Чтоб первый был не прям такойже как оригинал. По сути бесполезен, так как помимо смены цвета применяется резкость и насыщение цветов.

		[JsonInclude]
		public float minShade = 0.75f; // Минимальный коэффициент затенения. 0 - полностью чёрный, 1 - оригрнал. 75% достаточно, текстура и так тёмная.
		[JsonInclude]
		public float maxShade = 1; // Максимальный коэффициент затенения. Значения больше 1 будут осветлять текстуру.

		[JsonInclude]
		public float sharpen = 2; // Значение резкости. Даже 2 почти превращает разделители в кашу, но мало влияет на конвейеры.
		[JsonInclude]
		public float saturation = 2; // Значение насыщения. Делает текстуру сочнее.

		[JsonInclude]
		public bool painMode = false; // Включает однопоточный режим.
		[JsonInclude]
		public bool ignoreMasks = false; // Отключает генерацию текстур через наложение маски.

		

		/// <summary>
		/// Пытаеся загрузить настройки из файла. В случае неудачи создаёт стандартные настройки и пытается записать их в файл
		/// </summary>
		/// <returns>Настройки генератора</returns>
		public static Settings Load()
		{
			Settings settings = new Settings();

			try
			{
				settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("config.json"));
			}
			catch
			{
				Console.WriteLine("Не удалось загрузить конфиг. Попытка сохранить стандартный...");
			}

			try
			{
				File.WriteAllText("config.json", JsonSerializer.Serialize(settings,
					new JsonSerializerOptions { WriteIndented = true }));
			}
			catch
			{
				Console.WriteLine("Не удалось сохранить конфигурацию. Работа продолжится со стандартными настройками.");
			}

			return settings;
		}
	}
}
