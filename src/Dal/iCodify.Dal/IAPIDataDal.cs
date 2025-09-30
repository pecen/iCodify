using iCodify.Dal.Dto;
using iCodify.Dal.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCodify.Dal
{
	public interface IAPIDataDal
	{
		APIDataDto Fetch(int id);
		APIDataDto Fetch(string name);
		APIDataDto Fetch(APIModels model);
	}
}
