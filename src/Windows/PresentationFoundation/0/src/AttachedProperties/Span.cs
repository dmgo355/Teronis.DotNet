﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Windows;
using RunControl = System.Windows.Documents.Run;
using SpanControl = System.Windows.Documents.Span;

namespace Teronis.Windows.PresentationFoundation.AttachedProperties
{
    public static class Span
    {
        public static readonly DependencyProperty TrimRunsProperty =
            DependencyProperty.RegisterAttached("TrimRuns", typeof(bool), typeof(Span),
                new PropertyMetadata(false, trimRunsChanged));

        public static bool GetTrimRuns(SpanControl span)
            => (bool)span.GetValue(TrimRunsProperty);

        public static void SetTrimRuns(SpanControl span, bool value)
            => span.SetValue(TrimRunsProperty, value);

        private static SpanControl GetSpanControl(object @object) {
            var spanControl = @object as SpanControl;

            if (spanControl is null) {
                throw new InvalidOperationException($"Dependency object is not of type {typeof(SpanControl)}.");
            }

            return spanControl;
        }

        private static void trimRunsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = GetSpanControl(d);
            textBlock.Loaded += OnTextBlockLoaded;
        }

        private static void TrimRunWhitespace(RunControl run)
        {
            var skipTrimStart = Run.GetSkipTrimStart(run);
            var skipTrimEnd = Run.GetSkipTrimEnd(run);
            var text = run.Text;

            if (!skipTrimStart && text.FirstOrDefault() == ' ') {
                text = text.Substring(1);
            }

            if (!skipTrimEnd && text.LastOrDefault() == ' ') {
                text = text[0..^1];
            }

            run.Text = text;
        }

        static void OnTextBlockLoaded(object sender, EventArgs args)
        {
            var span = GetSpanControl(sender);
            span.Loaded -= OnTextBlockLoaded;

            var runs = span.Inlines
                .OfType<RunControl>()
                .ToList();

            foreach (var run in runs) {
                TrimRunWhitespace(run);
            }
        }
    }
}
