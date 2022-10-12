using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Textures_Generator;
namespace Textures_Generator;

internal class Program
{
	public static Settings? Settings { get; private set; }



	static void Main(string[] args)
	{
		Settings = Settings.Load();

		Directory.CreateDirectory("Input");
		Directory.CreateDirectory("Output");

		List<Texture> textures = ParseTextures("Input");
		ProcessTextures(textures);
	}


	/// <summary>
	/// Подготавливает список задач и запускает его выполнение. Количество задач примерно равно [количество текстур] * [hues] * [shades]
	/// </summary>
	/// <param name="textures">Список текстур для обработки</param>
	private static void ProcessTextures(List<Texture> textures)
	{
		List<Job> jobs = PrepareJobs(textures);


		int cooldown = 2;
		Console.WriteLine($"\nПодготовлено {jobs.Count} задач на отрисовку. Старт через { (cooldown) } секунд");
		Thread.Sleep(cooldown*1000);



		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();

		if (Program.Settings.painMode)
		{
			WorkConsistently(jobs); // Для любителей долгих вычислений.
		}
		else
		{
			WorkInParallel(jobs);
		}

		stopWatch.Stop();
		Console.WriteLine($"Все задачи выполнены за {stopWatch.Elapsed}");
		Console.ReadKey();
	}




	/// <summary>
	/// Генерирует список задач для последующего выполнения
	/// </summary>
	/// <param name="textures">Список текстур</param>
	/// <returns></returns>
	private static List<Job> PrepareJobs(List<Texture> textures)
	{
		List<Job> jobs = new List<Job>();
		foreach (var texture in textures)
		{
			var counter = 0;
			for (int i = 0; i < Program.Settings.hues; i++)
			{
				for (int j = 0; j < Program.Settings.shades; j++)
				{
					counter++;
					float hue = (360f / Program.Settings.hues) * i;
					float shade = Program.Settings.maxShade - ((Program.Settings.maxShade - Program.Settings.minShade) / Program.Settings.shades) * j;
					jobs.Add(new Job(texture, hue, shade, counter, jobs.Count));
				}
			}
		}

		return jobs;
	}



	/// <summary>
	/// Чем больше у вас ядер, тем больнее на это будет смотреть
	/// </summary>
	/// <param name="jobs">Список задач</param>
	private static void WorkConsistently(List<Job> jobs)
	{
		foreach (Job job in jobs)
		{
			job.DoJob();
		}
	}



	/// <summary>
	/// Чем меньше у вас ядер, тем меньше отличий от WorkConsistently
	/// </summary>
	/// <param name="jobs">Список задач</param>
	private static void WorkInParallel(List<Job> jobs)
	{
		var result = Parallel.ForEach(jobs, job => job.DoJob());
	}



	/// <summary>
	/// Пытается рекурсивно загрузить все изображения по указанному пути
	/// </summary>
	/// <param name="path">Путь до папки с текстурами</param>
	/// <returns></returns>
	private static List<Texture> ParseTextures(string path)
	{
		List<Texture> textures = new List<Texture>();
		var files = Directory.GetFiles(path);
		var dirs = Directory.GetDirectories(path);

		foreach(var file in files)
		{
			var texture = Texture.LoadTexture(file);
			if (texture != null)
			{
				textures.Add(texture);
			}
		}

		foreach (var dir in dirs)
		{
			textures.AddRange(ParseTextures(dir));
		}

		return textures;
	}
}