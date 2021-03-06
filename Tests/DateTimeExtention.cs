﻿using NUnit.Framework;
using Shared.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestFixture]
    public class DateTimeExtentionTest
    {
        [Test]
        public void TruncateToWeekStartTest()
        {
            var date = new DateTime(2018, 4, 26, 12, 0, 0);
            var weekStart = date.TruncateToWeekStart();
            Assert.IsTrue(weekStart.DayOfWeek == DayOfWeek.Monday);
            Assert.IsTrue(weekStart.Day == 23);
            Assert.IsTrue(weekStart.Month == 4);
            Assert.IsTrue(weekStart.Year == 2018);
        }

        [Test]
        public void IsOverlapTest()
        {
            // ----
            //   ----
            var b1 = new DateTime(2017, 1, 1, 12, 0, 0);
            var e1 = new DateTime(2017, 1, 1, 13, 0, 0);

            var b2 = new DateTime(2017, 1, 1, 12, 30, 0);
            var e2 = new DateTime(2017, 1, 1, 13, 30, 0);

            Assert.IsTrue(DateTimeExtentions.IsOverlap(b1, e1, b2, e2));

            //   ----
            // ----
            b1 = new DateTime(2017, 1, 1, 12, 0, 0);
            e1 = new DateTime(2017, 1, 1, 13, 0, 0);

            b2 = new DateTime(2017, 1, 1, 11, 30, 0);
            e2 = new DateTime(2017, 1, 1, 12, 30, 0);

            Assert.IsTrue(DateTimeExtentions.IsOverlap(b1, e1, b2, e2));

            // ------
            //  ----
            b1 = new DateTime(2017, 1, 1, 11, 0, 0);
            e1 = new DateTime(2017, 1, 1, 13, 0, 0);

            b2 = new DateTime(2017, 1, 1, 12, 0, 0);
            e2 = new DateTime(2017, 1, 1, 12, 30, 0);

            Assert.IsTrue(DateTimeExtentions.IsOverlap(b1, e1, b2, e2));

            //  ----
            // ------
            b1 = new DateTime(2017, 1, 1, 12, 0, 0);
            e1 = new DateTime(2017, 1, 1, 12, 30, 0);

            b2 = new DateTime(2017, 1, 1, 11, 0, 0);
            e2 = new DateTime(2017, 1, 1, 13, 30, 0);

            Assert.IsTrue(DateTimeExtentions.IsOverlap(b1, e1, b2, e2));

            //  ----
            //        ----
            b1 = new DateTime(2017, 1, 1, 11, 0, 0);
            e1 = new DateTime(2017, 1, 1, 11, 30, 0);

            b2 = new DateTime(2017, 1, 1, 13, 0, 0);
            e2 = new DateTime(2017, 1, 1, 13, 30, 0);

            Assert.IsFalse(DateTimeExtentions.IsOverlap(b1, e1, b2, e2));

            //        ----
            //  ----
            b1 = new DateTime(2017, 1, 1, 13, 0, 0);
            e1 = new DateTime(2017, 1, 1, 13, 30, 0);

            b2 = new DateTime(2017, 1, 1, 11, 0, 0);
            e2 = new DateTime(2017, 1, 1, 11, 30, 0);

            Assert.IsFalse(DateTimeExtentions.IsOverlap(b1, e1, b2, e2));
        }

    }
}
