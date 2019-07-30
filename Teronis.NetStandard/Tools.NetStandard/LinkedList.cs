﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Teronis.Tools.NetStandard
{
    public static class LinkedListTools
    {
        public static IEnumerable<T> YieldReversedItems<T>(this LinkedList<T> list)
        {
            var node = list.Last;

            while (node != null) {
                yield return node.Value;
                node = node.Previous;
            }
        }
    }
}
