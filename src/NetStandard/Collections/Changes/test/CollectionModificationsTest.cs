﻿using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Teronis.NetStandard.Collections.Changes.Test
{
    public class CollectionModificationsTest
    {
        [Theory]
        [ClassData(typeof(Generator))]
        public void Synchronize_collections(int[] leftItems, int[] rightItems)
        {
            var collection = new SynchronizableCollection<int>(leftItems);
            collection.SynchronizeCollection(rightItems);
            Assert.Equal(rightItems, collection);
        }

        public class Generator : IEnumerable<object[]>
        {
            private T[] arrayt<T>(params T[] values) =>
                values;

            private object[] array(params object[] values) =>
                values;

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return array(arrayt(9), arrayt(9));
                yield return array(arrayt(9, 9), arrayt(1, 9));
                yield return array(array(9, 6), array(6, 9));
                yield return array(array(4, 4, 9), array(9, 9, 4));
                yield return array(array(4, 9, 5), array(5, 9, 4));
                yield return array(array(4, 9, 5), array(5, 9, 4));
                yield return array(array(3, 4, 9, 5), array(5, 6, 9, 4));
                yield return array(array(4, 9, 5, 6), array(9, 4, 3));
                yield return array(array(9, 6), array(6, 9, 4, 5));
                yield return array(array(9, 0, 6), array(6, 9));
                yield return array(array(9, 5, 6), array(6, 9, 4));
                yield return array(array(9, 5, 6), array(6, 9, 4, 3));
                yield return array(array(4, 9, 5, 6), array(6, 9, 4, 3));
                yield return array(array(5, 4, 3, 2, 1), array(3, 4, 5, 6, 7, 8, 9));
                yield return array(array(3, 4, 9, 5, 6), array(5, 6, 9, 3, 4));
                yield return array(array(0, 0, 0, 0, 1), array(1, 0, 0, 0, 1));
                yield return array(array(0, 0, 1), array(1, 0, 1));

                //var run = 5;
                //var random = new Random();

                //var leftItems = new List<int>(run);
                //var rightItems = new List<int>(run);

                //for (var index = 0; index < run; index++) {
                //    leftItems.Add(random.Next(index * 10, (index * 10) + 10));
                //    rightItems.Add(random.Next(index * 10, (index * 10) + 10));
                //}

                //yield return array(leftItems, rightItems);
            }

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }
    }
}
