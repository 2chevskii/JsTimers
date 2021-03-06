﻿using System;

namespace JsTimers.Internal
{
	internal sealed class Interval:BaseTimer
	{
		protected override BaseTimer InitNew(Action callback, uint timeMS) => base.InitNew(callback, timeMS);

		internal BaseTimer InitNew(Action callback, uint timeMS,bool executeImmediate)
		{
			var _this = InitNew(callback, timeMS);

			if(executeImmediate)
			{
				ExecuteImmediate();
			}

			return _this;
		}
	}
}
