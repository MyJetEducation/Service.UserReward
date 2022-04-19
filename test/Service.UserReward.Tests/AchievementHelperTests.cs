using System;
using NUnit.Framework;
using Service.UserReward.Helpers;

namespace Service.UserReward.Tests
{
	public class AchievementHelperTests
	{
		private static readonly object[] ValidPurchaseDates =
		{
			new[] {new DateTime(2022, 04, 03), new DateTime(2022, 04, 10), new DateTime(2022, 04, 17), new DateTime(2022, 04, 24)},
			new[] {new DateTime(2022, 04, 02), new DateTime(2022, 04, 09), new DateTime(2022, 04, 16), new DateTime(2022, 04, 23)},
			new[] {new DateTime(2022, 04, 03), new DateTime(2022, 04, 09), new DateTime(2022, 04, 15), new DateTime(2022, 04, 21)},
			new[] {new DateTime(2022, 04, 02), new DateTime(2022, 04, 08), new DateTime(2022, 04, 14), new DateTime(2022, 04, 20)},
			new[] {new DateTime(2022, 04, 01), new DateTime(2022, 04, 07), new DateTime(2022, 04, 13), new DateTime(2022, 04, 19)},
			new[] {new DateTime(2022, 03, 31), new DateTime(2022, 04, 06), new DateTime(2022, 04, 12), new DateTime(2022, 04, 18)},
			new[] {new DateTime(2022, 03, 30), new DateTime(2022, 04, 05), new DateTime(2022, 04, 11), new DateTime(2022, 04, 17)},
			new[] {new DateTime(2022, 03, 29), new DateTime(2022, 04, 04), new DateTime(2022, 04, 10), new DateTime(2022, 04, 16)},
			new[] {new DateTime(2022, 03, 28), new DateTime(2022, 04, 03), new DateTime(2022, 04, 09), new DateTime(2022, 04, 15)},
			new[] {new DateTime(2022, 03, 27), new DateTime(2022, 04, 02), new DateTime(2022, 04, 08), new DateTime(2022, 04, 13)},
			new[] {new DateTime(2022, 03, 28), new DateTime(2022, 04, 07), new DateTime(2022, 04, 12), new DateTime(2022, 04, 24)},
			new[] {new DateTime(2022, 03, 28), new DateTime(2022, 04, 07), new DateTime(2022, 04, 12), new DateTime(2022, 04, 24)},
			new[] {new DateTime(2022, 03, 28), new DateTime(2022, 04, 10), new DateTime(2022, 04, 11), new DateTime(2022, 04, 24)}
		};

		[TestCaseSource(nameof(ValidPurchaseDates))]
		public void CheckInterval_intervals_are_correct(DateTime[] dateTimes)
		{
			bool result = AchievementHelper.CheckPurchaseDates(4, dateTimes);

			Assert.IsTrue(result);
		}

		private static readonly object[] InvalidPurchaseDates =
		{
			new[] {new DateTime(2022, 02, 01), new DateTime(2022, 04, 02), new DateTime(2022, 04, 03), new DateTime(2022, 04, 04)},
			new[] {new DateTime(2022, 02, 28), new DateTime(2022, 04, 10), new DateTime(2022, 04, 17), new DateTime(2022, 04, 24)},
			new[] {new DateTime(2022, 04, 02), new DateTime(2022, 04, 09), new DateTime(2022, 04, 16), new DateTime(2022, 05, 01)},
			new[] {new DateTime(2022, 04, 03), new DateTime(2022, 04, 09), new DateTime(2022, 04, 15), new DateTime(2022, 04, 30)},
			new[] {new DateTime(2022, 03, 20), new DateTime(2022, 04, 14), new DateTime(2022, 04, 24)}
		};

		[TestCaseSource(nameof(InvalidPurchaseDates))]
		public void CheckInterval_intervals_are_incorrect(DateTime[] dateTimes)
		{
			bool result = AchievementHelper.CheckPurchaseDates(4, dateTimes);

			Assert.IsFalse(result);
		}
	}
}