﻿namespace SuperLinq.Async;

public static partial class AsyncSuperEnumerable
{
	/// <summary>
	/// Creates a right-aligned sliding window over the source sequence
	/// of a given size.
	/// </summary>
	/// <typeparam name="TSource">
	/// The type of the elements of <paramref name="source"/>.</typeparam>
	/// <param name="source">
	/// The sequence over which to create the sliding window.</param>
	/// <param name="size">Size of the sliding window.</param>
	/// <returns>A sequence representing each sliding window.</returns>
	/// <remarks>
	/// <para>
	/// A window can contain fewer elements than <paramref name="size"/>,
	/// especially as it slides over the start of the sequence.</para>
	/// <para>
	/// This operator uses deferred execution and streams its results.</para>
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Console.WriteLine(
	///     Enumerable
	///         .Range(1, 5)
	///         .WindowRight(3)
	///         .Select(w => "AVG(" + w.ToDelimitedString(",") + ") = " + w.Average())
	///         .ToDelimitedString(Environment.NewLine));
	///
	/// // Output:
	/// // AVG(1) = 1
	/// // AVG(1,2) = 1.5
	/// // AVG(1,2,3) = 2
	/// // AVG(2,3,4) = 3
	/// // AVG(3,4,5) = 4
	/// ]]></code>
	/// </example>

	public static IAsyncEnumerable<IList<TSource>> WindowRight<TSource>(this IAsyncEnumerable<TSource> source, int size)
	{
		Guard.IsNotNull(source);
		Guard.IsGreaterThanOrEqualTo(size, 1);

		return Core(source, size);

		static async IAsyncEnumerable<IList<TSource>> Core(
			IAsyncEnumerable<TSource> source, int size,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			await using var e = source.GetConfiguredAsyncEnumerator(cancellationToken);
			if (!await e.MoveNextAsync())
				yield break;

			var window = new TSource[1] { e.Current };

			for (var i = 1; i < size; i++)
			{
				if (!await e.MoveNextAsync())
				{
					yield return window;
					yield break;
				}

				var newWindow = new TSource[i + 1];
				window.AsSpan().CopyTo(newWindow);
				newWindow[i] = e.Current;

				yield return window;
				window = newWindow;
			}

			while (await e.MoveNextAsync())
			{
				var newWindow = new TSource[size];
				window.AsSpan()[1..].CopyTo(newWindow);
				newWindow[^1] = e.Current;

				yield return window;
				window = newWindow;
			}

			yield return window;
		}
	}
}
