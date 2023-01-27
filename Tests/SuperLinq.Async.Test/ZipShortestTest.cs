﻿namespace Test.Async;

public class ZipShortestTest
{
	[Fact]
	public void ZipShortestIsLazy()
	{
		var bs = new AsyncBreakingSequence<int>();
		bs.ZipShortest(bs, BreakingFunc.Of<int, int, int>());
		bs.ZipShortest(bs, bs, BreakingFunc.Of<int, int, int, int>());
		bs.ZipShortest(bs, bs, bs, BreakingFunc.Of<int, int, int, int, int>());
	}

	[Fact]
	public async Task TwoParamsDisposesInnerSequencesCaseGetEnumeratorThrows()
	{
		using var s1 = TestingSequence.Of(1, 2);

		await Assert.ThrowsAsync<TestException>(async () =>
			await s1.ZipShortest(new AsyncBreakingSequence<int>()).Consume());
	}

	[Theory]
	[InlineData(1), InlineData(2)]
	public async Task TwoParamsWorksProperly(int offset)
	{
		var o1 = (offset + 0) % 2 + 2;
		var o2 = (offset + 1) % 2 + 2;

		await using var ts1 = Enumerable.Range(1, o1).AsTestingSequence();
		await using var ts2 = Enumerable.Range(1, o2).AsTestingSequence();

		await ts1.ZipShortest(ts2).AssertSequenceEqual(
			Enumerable.Range(1, 2).Select(x => (x, x)));
	}

	[Fact]
	public async Task ThreeParamsDisposesInnerSequencesCaseGetEnumeratorThrows()
	{
		using var s1 = TestingSequence.Of(1, 2);
		using var s2 = TestingSequence.Of(1, 2);

		await Assert.ThrowsAsync<TestException>(async () =>
			await s1.ZipShortest(s2, new AsyncBreakingSequence<int>()).Consume());
	}

	[Theory]
	[InlineData(1), InlineData(2), InlineData(3)]
	public async Task ThreeParamsWorksProperly(int offset)
	{
		var o1 = (offset + 0) % 3 + 2;
		var o2 = (offset + 1) % 3 + 2;
		var o3 = (offset + 2) % 3 + 2;

		await using var ts1 = Enumerable.Range(1, o1).AsTestingSequence();
		await using var ts2 = Enumerable.Range(1, o2).AsTestingSequence();
		await using var ts3 = Enumerable.Range(1, o3).AsTestingSequence();

		await ts1.ZipShortest(ts2, ts3).AssertSequenceEqual(
			Enumerable.Range(1, 2)
				.Select(x => (x, x, x)));
	}

	[Fact]
	public async Task FourParamsDisposesInnerSequencesCaseGetEnumeratorThrows()
	{
		using var s1 = TestingSequence.Of(1, 2);
		using var s2 = TestingSequence.Of(1, 2);
		using var s3 = TestingSequence.Of(1, 2);

		await Assert.ThrowsAsync<TestException>(async () =>
			await s1.ZipShortest(s2, s3, new AsyncBreakingSequence<int>()).Consume());
	}

	[Theory]
	[InlineData(1), InlineData(2), InlineData(3), InlineData(4)]
	public async Task FourParamsWorksProperly(int offset)
	{
		var o1 = (offset + 0) % 4 + 2;
		var o2 = (offset + 1) % 4 + 2;
		var o3 = (offset + 2) % 4 + 2;
		var o4 = (offset + 3) % 4 + 2;

		await using var ts1 = Enumerable.Range(1, o1).AsTestingSequence();
		await using var ts2 = Enumerable.Range(1, o2).AsTestingSequence();
		await using var ts3 = Enumerable.Range(1, o3).AsTestingSequence();
		await using var ts4 = Enumerable.Range(1, o4).AsTestingSequence();

		await ts1.ZipShortest(ts2, ts3, ts4).AssertSequenceEqual(
			Enumerable.Range(1, 2)
				.Select(x => (x, x, x, x)));
	}
}
