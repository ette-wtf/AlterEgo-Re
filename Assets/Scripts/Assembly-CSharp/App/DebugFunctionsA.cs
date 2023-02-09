using System.Collections;

namespace App
{
	[AppUtil.Title("デバッグ機能")]
	public static class DebugFunctionsA
	{
		[AppUtil.Title("パラメータ更新")]
		public static AppUtil.Coroutine UpdateParameter = UpdateParams;

		private static IEnumerator UpdateParams()
		{
			yield return GssDataHelper.Overwrite();
		}
	}
}
