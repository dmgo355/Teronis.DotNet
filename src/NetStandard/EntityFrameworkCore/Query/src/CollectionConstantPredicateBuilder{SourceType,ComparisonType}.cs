﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Teronis.Linq.Expressions;

namespace Teronis.EntityFrameworkCore.Query
{
    public class CollectionConstantPredicateBuilder<SourceType, ComparisonType> : IThenInCollectionConstantPredicateBuilder<SourceType, ComparisonType>, IChildCollectionConstantPredicateBuilder
    {
        private readonly IParentCollectionConstantPredicateBuilder parentBuilder;
        private List<SourceAndValueComparison> sourceAndValueComparisons = null!;
        private readonly Func<Expression, Expression, BinaryExpression> consecutiveItemBinaryExpressionFactory;
        private readonly Func<Expression, Expression, BinaryExpression>? parentBinaryExpressionFactory;

        /// <summary>
        /// Creates a collection constant predicate builder from a non-null and non-empty collection.
        /// via <paramref name="consecutiveItemBinaryExpressionFactory"/>.
        /// </summary>
        /// <param name="consecutiveItemBinaryExpressionFactory">The binary expression factory combines an item predicate with a previous item predicate.</param>
        /// <param name="comparisonValues">A collection of comparison values with at least one item.</param>
        /// <param name="sourceAndItemPredicate">The expression that represents the predicate.</param>
        /// <exception cref="ArgumentNullException">A parameter is null.</exception>
        /// <exception cref="ArgumentException">The parameter <paramref name="comparisonValues"/> is empty.</exception>
        public CollectionConstantPredicateBuilder(
            Func<Expression, Expression, BinaryExpression> consecutiveItemBinaryExpressionFactory,
            IEnumerable<ComparisonType> comparisonValues,
            Expression<SourceInConstantPredicateDelegate<SourceType, ComparisonType>> sourceAndItemPredicate)
        {
            if (comparisonValues is null) {
                throw new ArgumentNullException(nameof(comparisonValues));
            }

            var comparisonValueEnumerator = comparisonValues.GetEnumerator();

            if (!comparisonValueEnumerator.MoveNext()) {
                throw new ArgumentException("The comparison list cannot be empty.", nameof(comparisonValues));
            }

            this.consecutiveItemBinaryExpressionFactory = consecutiveItemBinaryExpressionFactory ?? throw new ArgumentNullException(nameof(consecutiveItemBinaryExpressionFactory));
            onConstruction(comparisonValueEnumerator, true, sourceAndItemPredicate, out var sourceParameterExpression, null);
            parentBuilder = new RootBuilder(new Stack<IChildCollectionConstantPredicateBuilder>(), sourceParameterExpression);
        }

        /// <summary>
        /// You have to call 
        /// <see cref="onConstruction(IEnumerator{ComparisonType}, bool, Expression{SourceInConstantPredicateDelegate{SourceType, ComparisonType}}, out ParameterExpression, ParameterExpression?)"/>
        /// manually.
        /// </summary>
        /// <param name="parentBuilder"></param>
        /// <param name="consecutiveItemBinaryExpressionFactory"></param>
        /// <param name="parentBinaryExpressionFactory"></param>
        private CollectionConstantPredicateBuilder(IParentCollectionConstantPredicateBuilder parentBuilder,
            Func<Expression, Expression, BinaryExpression> consecutiveItemBinaryExpressionFactory,
            Func<Expression, Expression, BinaryExpression> parentBinaryExpressionFactory)
        {
            this.parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));
            this.consecutiveItemBinaryExpressionFactory = consecutiveItemBinaryExpressionFactory ?? throw new ArgumentNullException(nameof(consecutiveItemBinaryExpressionFactory));
            this.parentBinaryExpressionFactory = parentBinaryExpressionFactory ?? throw new ArgumentNullException(nameof(parentBinaryExpressionFactory));
        }

        private void onConstruction(IEnumerator<ComparisonType> comparisonValueEnumerator, bool enumeratorMovedToFirst,
            Expression<SourceInConstantPredicateDelegate<SourceType, ComparisonType>> sourceAndItemPredicate,
            out ParameterExpression sourceParameterExpression, ParameterExpression? sourceParameterReplacement)
        {
            sourceAndValueComparisons = new List<SourceAndValueComparison>();
            sourceParameterExpression = null!;

            while (enumeratorMovedToFirst || comparisonValueEnumerator.MoveNext()) {
                enumeratorMovedToFirst = false;
                var comparisonValue = comparisonValueEnumerator.Current;

                var whereInConstantExpression = SourceExpression.WhereInConstant(comparisonValue, sourceAndItemPredicate,
                    out sourceParameterExpression, sourceParameterReplacement: sourceParameterReplacement);

                var sourceValueComparison = new SourceAndValueComparison(comparisonValue, whereInConstantExpression);
                sourceAndValueComparisons.Add(sourceValueComparison);
            }
        }

        private CollectionConstantPredicateBuilder<SourceType, ComparisonType> thenDefinePredicatePerItemInCollection<ThenComparisonType>(
            Func<Expression, Expression, BinaryExpression> parentBinaryExpressionFactory,
            Func<ComparisonType, IEnumerable<ThenComparisonType>?> comparisonValuesFactory,
            ComparisonValuesBehaviourFlags comparisonValuesBehaviourFlags,
            Func<Expression, Expression, BinaryExpression> consecutiveItemBinaryExpressionFactory,
            Expression<SourceInConstantPredicateDelegate<SourceType, ThenComparisonType>> sourceAndItemPredicate,
            Action<IThenInCollectionConstantPredicateBuilder<SourceType, ThenComparisonType>>? thenSourcePredicate = null)
        {
            var sourceValueComparisonsCount = sourceAndValueComparisons.Count;

            for (int comparisonIndex = 0; comparisonIndex < sourceValueComparisonsCount; comparisonIndex++) {
                var sourceValueComparison = sourceAndValueComparisons[comparisonIndex];
                var enumerable = comparisonValuesFactory(sourceValueComparison.ComparisonValue);
                var enumerator = enumerable?.GetEnumerator();
                var enumeratorIsNull = enumerator is null;
                var enumeratorIsEmpty = !enumeratorIsNull && !enumerator!.MoveNext();

                var evaluateToFalse = (enumeratorIsNull && comparisonValuesBehaviourFlags.HasFlag(ComparisonValuesBehaviourFlags.NullLeadsToFalse))
                                        || enumeratorIsEmpty && comparisonValuesBehaviourFlags.HasFlag(ComparisonValuesBehaviourFlags.EmptyLeadsToFalse);

                if ((enumeratorIsNull || enumeratorIsEmpty)
                    && !evaluateToFalse) {
                    continue;
                }

                var expressionAppender = new ExpressionAppender(comparisonIndex, sourceAndValueComparisons);
                var parentBuilder = new ParentBuilder(this, expressionAppender);

                IChildCollectionConstantPredicateBuilder childBuilder;
                IThenInCollectionConstantPredicateBuilder<SourceType, ThenComparisonType>? thenInCollectionBuilder;

                if (evaluateToFalse) {
                    childBuilder = new FalseEvaluatingChildBuilder(parentBuilder);
                    thenInCollectionBuilder = null;
                } else {
                    var builder = new CollectionConstantPredicateBuilder<SourceType, ThenComparisonType>(parentBuilder,
                        consecutiveItemBinaryExpressionFactory, parentBinaryExpressionFactory);

                    builder.onConstruction(enumerator!, true, sourceAndItemPredicate, out _, parentBuilder.SourceParameterExpression);
                    childBuilder = builder;
                    thenInCollectionBuilder = builder;
                }

                // We only want to build level one and upward levels.
                parentBuilder.StackBuilder(childBuilder);

                if (!(thenInCollectionBuilder is null)) {
                    thenSourcePredicate?.Invoke(thenInCollectionBuilder);
                }
            }

            return this;
        }

        /// <summary>
        /// Creates a deferred collection constant predicate builder from <paramref name="comparisonValuesFactory"/> that might return a null or 
        /// empty collection.
        /// </summary>
        /// <typeparam name="ThenComparisonType">The type of an item of the return value of <paramref name="comparisonValuesFactory"/>.</typeparam>
        /// <param name="comparisonValuesFactory">A collection expression that might return a comparison value list with at least one item.</param>
        /// <returns>A deferred collection constant predicate builder.</returns>
        public DeferredThenCreateBuilder<ThenComparisonType> ThenCreateFromCollection<ThenComparisonType>(
            Func<Expression, Expression, BinaryExpression> parentBinaryExpressionFactory,
            Func<ComparisonType, IEnumerable<ThenComparisonType>?> comparisonValuesFactory,
            ComparisonValuesBehaviourFlags comparisonValuesBehaviourFlags = ComparisonValuesBehaviourFlags.NullOrEmptyLeadsToSkip) =>
            new DeferredThenCreateBuilder<ThenComparisonType>(this, parentBinaryExpressionFactory, comparisonValuesFactory, comparisonValuesBehaviourFlags);

        IDeferredThenCreateCollectionConstantBuilder<ThenComparisonType> IThenInCollectionConstantPredicateBuilder<SourceType, ComparisonType>.ThenCreateFromCollection<ThenComparisonType>(
            Func<Expression, Expression, BinaryExpression> parentBinaryExpressionFactory,
            Func<ComparisonType, IEnumerable<ThenComparisonType>?> comparisonValuesFactory,
            ComparisonValuesBehaviourFlags comparisonValuesBehaviourFlags) =>
            ThenCreateFromCollection(parentBinaryExpressionFactory, comparisonValuesFactory, comparisonValuesBehaviourFlags);

        protected Expression concatenateComparisonExpressions()
        {
            var sourceValueComparisonCount = sourceAndValueComparisons.Count;
            var concatenatedExpression = sourceAndValueComparisons[0].SourceAndItemPredicate;

            for (int index = 1; index < sourceValueComparisonCount; index++) {
                var sourceValueComparison = sourceAndValueComparisons[index];
                concatenatedExpression = consecutiveItemBinaryExpressionFactory(concatenatedExpression, sourceValueComparison.SourceAndItemPredicate);
            }

            return concatenatedExpression;
        }

        private void appendConcatenatedExpressionToParent()
        {
            if (parentBinaryExpressionFactory == null) {
                throw new InvalidOperationException("You cannot build the root builder.");
            }

            var concatenatedExpression = concatenateComparisonExpressions();
            parentBuilder.AppendExpression(concatenatedExpression, parentBinaryExpressionFactory);
        }

        void IChildCollectionConstantPredicateBuilder.AppendConcatenatedExpressionToParent() =>
            appendConcatenatedExpressionToParent();

        /// <summary>
        /// Builds the body expression for a possible lambda expression.
        /// </summary>
        /// <param name="concatenatedExpressionFactory">Manipulates the concatenated expression result.</param>
        /// <returns>The body expression for a possible lambda expression.</returns>
        public Expression BuildBodyExpression(Func<Expression, Expression>? concatenatedExpressionFactory = null)
        {
            while (parentBuilder.TryPopBuilder(out var builder)) {
                builder.AppendConcatenatedExpressionToParent();
            }

            var concatenatedExpression = concatenateComparisonExpressions();

            if (concatenatedExpressionFactory != null) {
                concatenatedExpression = concatenatedExpressionFactory(concatenatedExpression);
            }

            return concatenatedExpression;
        }

        /// <summary>
        /// Builds the body expression for a possible lambda expression.
        /// </summary>
        /// <typeparam name="TargetType">The target type you can use for mapping in <paramref name="configureMemberMappings"/>.</typeparam>
        /// <param name="configureMemberMappings">Lets you map source type member accesses to target type member accesses.</param>
        /// <param name="targetParameter">The parameter that served as replacement for the member mappings.</param>
        /// <param name="concatenatedExpressionFactory">Manipulates the concatenated expression result.</param>
        /// <returns>The body expression for a possible lambda expression.</returns>
        internal Expression BuildBodyExpression<TargetType>(Action<ICollectionConstantPredicateBuilderExpressionMapper<SourceType, TargetType>> configureMemberMappings,
            out ParameterExpression targetParameter, Func<Expression, Expression>? concatenatedExpressionFactory = null)
        {
            var concatenatedExpression = BuildBodyExpression(concatenatedExpressionFactory);
            targetParameter = Expression.Parameter(typeof(TargetType), "sourceAsTarget");

            var expressionMapper = new CollectionConstantPredicateBuilderExpressionMapper<TargetType>(parentBuilder.SourceParameterExpression,
                targetParameter);

            configureMemberMappings(expressionMapper);
            var expressionMappings = expressionMapper.GetMappings();
            var expressionReplacer = new EqualityComparingExpressionReplacerVisitor(expressionMappings);
            concatenatedExpression = expressionReplacer.Visit(concatenatedExpression);
            return concatenatedExpression;
        }

        /// <summary>
        /// Builds the body expression for a possible lambda expression.
        /// </summary>
        /// <typeparam name="TargetType">The target type you can use for mapping in <paramref name="configureMemberMappings"/>.</typeparam>
        /// <param name="configureMemberMappings">Lets you map source type member accesses to target type member accesses.</param>
        /// <param name="concatenatedExpressionFactory">Manipulates the concatenated expression result.</param>
        /// <returns>The body expression for a possible lambda expression.</returns>
        public Expression BuildBodyExpression<TargetType>(Action<ICollectionConstantPredicateBuilderExpressionMapper<SourceType, TargetType>> configureMemberMappings,
            Func<Expression, Expression>? concatenatedExpressionFactory = null) =>
            BuildBodyExpression(configureMemberMappings, out _, concatenatedExpressionFactory);

        private Expression<Func<ParameterType, bool>> buildLambdaExpression<ParameterType>(Expression body, ParameterExpression parameter) =>
            Expression.Lambda<Func<ParameterType, bool>>(body, parameter);

        /// <summary>
        /// Builds the lambda expression.
        /// </summary>
        /// <param name="concatenatedExpressionFactory">Manipulates the concatenated expression result.</param>
        /// <returns>The lambda expression.</returns>
        public Expression<Func<SourceType, bool>> BuildLambdaExpression(Func<Expression, Expression>? concatenatedExpressionFactory = null)
        {
            var concatenatedExpression = BuildBodyExpression(concatenatedExpressionFactory: concatenatedExpressionFactory);
            return buildLambdaExpression<SourceType>(concatenatedExpression, parentBuilder.SourceParameterExpression);
        }

        /// <summary>
        /// Builds the lambda expression.
        /// </summary>
        /// <typeparam name="TargetType">The target type you can use for mapping in <paramref name="configureMemberMappings"/>.</typeparam>
        /// <param name="configureMemberMappings">Lets you map source type member accesses to target type member accesses.</param>
        /// <param name="concatenatedExpressionFactory">Manipulates the concatenated expression result.</param>
        /// <returns>The lambda expression.</returns>
        public Expression<Func<TargetType, bool>> BuildLambdaExpression<TargetType>(Action<ICollectionConstantPredicateBuilderExpressionMapper<SourceType, TargetType>> configureMemberMappings,
            Func<Expression, Expression>? concatenatedExpressionFactory = null)
        {
            var bodyExpression = BuildBodyExpression(configureMemberMappings, out var targetParameter, concatenatedExpressionFactory);
            return buildLambdaExpression<TargetType>(bodyExpression, targetParameter);
        }

        private readonly struct RootBuilder : IParentCollectionConstantPredicateBuilder
        {
            public readonly Stack<IChildCollectionConstantPredicateBuilder> BuilderStack { get; }
            public readonly ParameterExpression SourceParameterExpression { get; }

            public readonly bool IsRoot => true;

            public RootBuilder(Stack<IChildCollectionConstantPredicateBuilder> builderStack, ParameterExpression sourceParameterExpression)
            {
                BuilderStack = builderStack ?? throw new ArgumentNullException(nameof(builderStack));
                SourceParameterExpression = sourceParameterExpression ?? throw new ArgumentNullException(nameof(sourceParameterExpression));
            }

            public void StackBuilder(IChildCollectionConstantPredicateBuilder builder) =>
                BuilderStack.Push(builder);

            public bool TryPopBuilder([MaybeNullWhen(false)] out IChildCollectionConstantPredicateBuilder builder)
            {
                if (BuilderStack.Count == 0) {
                    builder = default;
                    return false;
                }

                builder = BuilderStack.Pop();
                return true;
            }

            public void AppendExpression(Expression expression, Func<Expression, Expression, BinaryExpression> binaryExpression) =>
                throw new NotImplementedException();
        }

        private readonly struct SourceAndValueComparison
        {
            public readonly ComparisonType ComparisonValue;
            public readonly Expression SourceAndItemPredicate;

            public SourceAndValueComparison(ComparisonType comparisonValue, Expression sourceAndItemPredicate)
            {
                ComparisonValue = comparisonValue;
                SourceAndItemPredicate = sourceAndItemPredicate;
            }
        }

        private readonly struct ExpressionAppender
        {
            private readonly int comparisonIndex;
            private readonly IList<SourceAndValueComparison> comparisons;

            public ExpressionAppender(int comparisonIndex, IList<SourceAndValueComparison> comparisons)
            {
                this.comparisonIndex = comparisonIndex;
                this.comparisons = comparisons ?? throw new ArgumentNullException(nameof(comparisons));

                if (comparisonIndex < 0 || comparisonIndex >= comparisons.Count) {
                    throw new ArgumentOutOfRangeException(nameof(comparisonIndex));
                }
            }

            public void AppendExpression(Expression expression, Func<Expression, Expression, BinaryExpression> binaryExpressionFactory)
            {
                var comparison = comparisons[comparisonIndex];
                var concatenatedExpression = binaryExpressionFactory(comparison.SourceAndItemPredicate, expression);
                comparisons[comparisonIndex] = new SourceAndValueComparison(comparison.ComparisonValue, concatenatedExpression);
            }
        }

        private readonly struct ParentBuilder : IParentCollectionConstantPredicateBuilder
        {
            public bool IsRoot => false;
            public ParameterExpression SourceParameterExpression => builder.parentBuilder.SourceParameterExpression;

            private readonly CollectionConstantPredicateBuilder<SourceType, ComparisonType> builder;
            private readonly CollectionConstantPredicateBuilder<SourceType, ComparisonType>.ExpressionAppender expressionAppender;

            public ParentBuilder(CollectionConstantPredicateBuilder<SourceType, ComparisonType> builder, ExpressionAppender expressionAppender)
            {
                this.builder = builder;
                this.expressionAppender = expressionAppender;
            }

            public void StackBuilder(IChildCollectionConstantPredicateBuilder builder) =>
                this.builder.parentBuilder.StackBuilder(builder);

            public bool TryPopBuilder([MaybeNullWhen(false)] out IChildCollectionConstantPredicateBuilder builder) =>
                this.builder.parentBuilder.TryPopBuilder(out builder);

            public void AppendExpression(Expression expression, Func<Expression, Expression, BinaryExpression> binaryExpression) =>
                expressionAppender.AppendExpression(expression, binaryExpression);
        }

        private readonly struct FalseEvaluatingChildBuilder : IChildCollectionConstantPredicateBuilder
        {
            private readonly ParentBuilder? parentBuilder;

            public FalseEvaluatingChildBuilder(ParentBuilder parentBuilder) =>
                this.parentBuilder = parentBuilder;

            public void AppendConcatenatedExpressionToParent()
            {
                var parentBuilder = this.parentBuilder ?? throw new InvalidOperationException("Parent builder has not been initialized.");
                parentBuilder.AppendExpression(Expression.Constant(false), Expression.AndAlso);
            }
        }

        public interface IDeferredThenCreateCollectionConstantBuilder<ThenComparisonType>
        {
            IThenInCollectionConstantPredicateBuilder<SourceType, ComparisonType> DefinePredicatePerItem(
                Func<Expression, Expression, BinaryExpression> consecutiveItemBinaryExpressionFactory,
                Expression<SourceInConstantPredicateDelegate<SourceType, ThenComparisonType>> sourceAndItemPredicate,
                Action<IThenInCollectionConstantPredicateBuilder<SourceType, ThenComparisonType>>? thenSourcePredicate = null);
        }

        public readonly struct DeferredThenCreateBuilder<ThenComparisonType> : IDeferredThenCreateCollectionConstantBuilder<ThenComparisonType>
        {
            private readonly CollectionConstantPredicateBuilder<SourceType, ComparisonType> currentBuilder;
            private readonly Func<Expression, Expression, BinaryExpression> parentBinaryExpressionFactory;
            private readonly Func<ComparisonType, IEnumerable<ThenComparisonType>?> comparisonValuesFactory;
            private readonly ComparisonValuesBehaviourFlags comparisonValuesBehaviourFlags;

            public DeferredThenCreateBuilder(CollectionConstantPredicateBuilder<SourceType, ComparisonType> currentBuilder,
                Func<Expression, Expression, BinaryExpression> parentBinaryExpressionFactory,
                Func<ComparisonType, IEnumerable<ThenComparisonType>?> comparisonValuesFactory,
                ComparisonValuesBehaviourFlags comparisonValuesBehaviourFlags)
            {
                this.currentBuilder = currentBuilder;
                this.parentBinaryExpressionFactory = parentBinaryExpressionFactory;
                this.comparisonValuesFactory = comparisonValuesFactory;
                this.comparisonValuesBehaviourFlags = comparisonValuesBehaviourFlags;
            }

            public CollectionConstantPredicateBuilder<SourceType, ComparisonType> DefinePredicatePerItem(
                Func<Expression, Expression, BinaryExpression> consecutiveItemBinaryExpressionFactory,
                Expression<SourceInConstantPredicateDelegate<SourceType, ThenComparisonType>> sourceAndItemPredicate,
                Action<IThenInCollectionConstantPredicateBuilder<SourceType, ThenComparisonType>>? thenSourcePredicate = null) =>
                currentBuilder.thenDefinePredicatePerItemInCollection(parentBinaryExpressionFactory, comparisonValuesFactory,
                    comparisonValuesBehaviourFlags, consecutiveItemBinaryExpressionFactory, sourceAndItemPredicate, thenSourcePredicate);

            IThenInCollectionConstantPredicateBuilder<SourceType, ComparisonType> CollectionConstantPredicateBuilder<SourceType, ComparisonType>.IDeferredThenCreateCollectionConstantBuilder<ThenComparisonType>.DefinePredicatePerItem(
                Func<Expression, Expression, BinaryExpression> consecutiveItemBinaryExpressionFactory,
                Expression<SourceInConstantPredicateDelegate<SourceType, ThenComparisonType>> sourceAndItemPredicate,
                Action<IThenInCollectionConstantPredicateBuilder<SourceType, ThenComparisonType>>? thenSourcePredicate) =>
                DefinePredicatePerItem(consecutiveItemBinaryExpressionFactory, sourceAndItemPredicate, thenSourcePredicate);
        }

        private class CollectionConstantPredicateBuilderExpressionMapper<TargetType> : ParameterReplacingExpressionMapper<SourceType, TargetType>,
            ICollectionConstantPredicateBuilderExpressionMapper<SourceType, TargetType>
        {
            public CollectionConstantPredicateBuilderExpressionMapper(ParameterExpression sourceParameterReplacement, ParameterExpression targetParameterReplacement)
                : base(sourceParameterReplacement, targetParameterReplacement) { }
        }
    }
}
