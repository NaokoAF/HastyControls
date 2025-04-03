using System.Runtime.CompilerServices;

namespace HastyControls.Core.Gyro;

public class TimedMovingAverage
{
	public float TimeWindow { get; set; }

	Entry[] buffer;
	int head;
	int tail;
	int count;
	float currentTime;
	float currentSum;

	record struct Entry(float Timestamp, float Value);

	/// <summary>
	/// Creates a TimedMovingAverage
	/// </summary>
	/// <param name="timeWindow">Window of time to average inputs for</param>
	/// <param name="inputsPerSecond">Estimated amount of inputs per second. If inputs exceed this value, the internal buffer grows accordingly</param>
	public TimedMovingAverage(float timeWindow, int inputsPerSecond)
	{
		if (timeWindow < 0) throw new ArgumentOutOfRangeException(nameof(timeWindow));
		if (inputsPerSecond < 0) throw new ArgumentOutOfRangeException(nameof(inputsPerSecond));

		TimeWindow = timeWindow;
		buffer = new Entry[(int)(inputsPerSecond * timeWindow)];
	}

	public float Add(float deltaTime, float value)
	{
		// add entry to buffer
		Enqueue(deltaTime, value);
		currentSum += value;

		// remove old entries
		Entry entry;
		while (count > 0 && currentTime - (entry = buffer[head]).Timestamp > TimeWindow)
		{
			currentSum -= entry.Value;
			count--;
			MoveNext(ref head);
		}

		return count > 0 ? currentSum / count : 0f;
	}

	public void Reset()
	{
		head = 0;
		tail = 0;
		count = 0;
		currentTime = 0f;
		currentSum = 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void Enqueue(float deltaTime, float value)
	{
		// advance time
		currentTime += deltaTime;

		// grow buffer if necessary
		if (count == buffer.Length)
			Grow();

		// add entry to buffer
		buffer[tail] = new Entry(currentTime, value);
		count++;
		MoveNext(ref tail);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void MoveNext(ref int value)
	{
		// rare branch is seemingly faster than ternary or modulo
		int temp = value + 1;
		if (temp == buffer.Length)
			temp = 0;
		value = temp;
	}

	// big method that doesn't get called often. don't inline
	[MethodImpl(MethodImplOptions.NoInlining)]
	void Grow()
	{
		int capacity = buffer.Length * 2;
		if ((uint)capacity > 2147483591)
			capacity = 2147483591;

		// cant grow any further
		if (capacity < buffer.Length)
			capacity = buffer.Length;

		// create new buffer and copy data
		Entry[] newBuffer = new Entry[capacity];
		if (count > 0)
		{
			if (head < tail)
			{
				Array.Copy(buffer, head, newBuffer, 0, count);
			}
			else
			{
				Array.Copy(buffer, head, newBuffer, 0, buffer.Length - head);
				Array.Copy(buffer, 0, newBuffer, buffer.Length - head, tail);
			}
		}

		buffer = newBuffer;
		head = 0;
		tail = count == capacity ? 0 : count;
	}
}
