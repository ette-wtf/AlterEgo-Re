using System.Collections.Generic;

namespace NCMB
{
	public delegate void NCMBQueryCallback<T>(List<T> objects, NCMBException error);
}
