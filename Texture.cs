using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Textures_Generator
{
	internal class Texture
	{
		string textureName, relativePath;
		Image baseTexture, mask;



		/// <summary>
		/// Применяет модификации к текстуре и сохраняет её.
		/// </summary>
		/// <param name="hue"></param>
		/// <param name="shade"></param>
		/// <param name="id"></param>
		/// <param name="globalId"></param>
		public void Process(float hue, float shade, int id, int globalId)
		{

			var processingTexture = baseTexture.Clone(ctx => { });

			if (HasMask())
			{
				var processingMask = mask.Clone(ApplyDiff(hue, shade, id, globalId));
				processingTexture.Mutate(ctx =>
				{
					ctx.DrawImage(processingMask, 1);
				});
			}
			else
			{
				processingTexture.Mutate(ApplyDiff(hue, shade, id, globalId));
			}

			var extension = Path.GetExtension(relativePath);
			

			Directory.CreateDirectory(Path.Combine("Output",Path.GetDirectoryName(relativePath)));

			string output = Path.Combine("Output", relativePath.Replace(extension, id + extension));
			processingTexture.SaveAsPng(output);
			Console.WriteLine($"[{globalId}] Saved texture variation (hue: {hue}, value: {shade}): {output}");
		}

		/// <summary>
		/// Госпаде ну и костыль, мне стыдно, правда
		/// </summary>
		private Action<IImageProcessingContext> ApplyDiff(float hue, float shade, int id, int globalId)
		{
			Action<IImageProcessingContext> action = ctx => {
				ctx.Hue(hue);
				ctx.Saturate(Program.Settings.saturation);
				ctx.GaussianSharpen(Program.Settings.sharpen);
				ctx.Lightness(shade);
			};
			return action;
		}


		/// <summary>
		/// Пытается загрузить указанный файл как изображение
		/// </summary>
		/// <param name="path">Путь до файла</param>
		/// <returns>Текстуру или null если что-то пошло не так</returns>
		internal static Texture? LoadTexture(String path)
		{
			var texture = new Texture();
			texture.textureName = Path.GetFileNameWithoutExtension(path);

			/// не нужно грузить маски отдельно
			if (texture.textureName.ToLower().EndsWith("_mask"))
				return null;

			texture.relativePath = Path.GetRelativePath("Input", path);
			try
			{
				texture.baseTexture = Image.Load(path);
			}
			catch(Exception e)
			{
				Console.WriteLine("Не удалось загрузить изображение: " + texture.relativePath);
				return null;
			}

			/// В будущем будет добавлена генерация текстур с наложением масок. Пока что оно умеет только находить эти маски
			try
			{
				texture.mask = Image.Load(texture.GetMaskPath());
				Console.WriteLine("Найдена маска для "+texture.textureName);
			}
			catch(Exception e)
			{
				// Ничего не делать, так и должно быть
			}

			Console.WriteLine($"Загружена текстура {texture.textureName} {(texture.HasMask() ? " с маской" : "")}");
			return texture;
		}




		/// <summary>
		/// Попытка найти путь до маски по имени основного файла
		/// </summary>
		/// <returns></returns>
		private string GetMaskPath()
		{
			var extension = Path.GetExtension(relativePath);
			var maskName = textureName + "_MASK" + extension;
			var maskPath = Path.Combine("Input", Path.GetDirectoryName(relativePath), maskName);
			return maskPath;
		}




		/// <summary>
		/// Проверка наличия маски в текстуре
		/// </summary>
		/// <returns>true если текстура обрабатывается в режиме наложения маски</returns>
		internal bool HasMask()
		{
			return mask != null;
		}
	}
}
