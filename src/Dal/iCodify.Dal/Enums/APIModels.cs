using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCodify.Dal.Enums
{
	public enum APIModels
	{
		[Description("The Opper AI interface from the Swedish company Opper.")]
		Opper,
		[Description("The OpenAI GPT models.")]
		OpenAI,
		[Description("The Gemini AI from Google DeepMind.")]
		Gemini
	}
}
