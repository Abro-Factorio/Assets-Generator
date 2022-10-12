using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Textures_Generator
{
	internal class Job
	{
		public Texture texture;
		public int id, globalId;
		public float hue, shade;

		public Job(Texture texture, float hue, float shade, int id, int globalId)
		{
			this.texture = texture;
			this.id = id;
			this.hue = hue;
			this.shade = shade;
			this.globalId = globalId;
		}


		/// <summary>
		/// запускает обработку назначенной текстуры
		/// </summary>
		public void DoJob()
		{
			texture.Process(Program.Settings.baseHue + hue, shade, id, globalId);
		}
	}
}
