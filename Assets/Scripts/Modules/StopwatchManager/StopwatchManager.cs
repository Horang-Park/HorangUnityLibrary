﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Structural;
using UniRx;
using Utilities;
using Utilities.Debug;

namespace Modules.StopwatchManager
{
	public class StopwatchManager : MonoSingleton<StopwatchManager>
	{
		private readonly Dictionary<int, Stopwatch> stopwatches = new();
		private readonly Dictionary<int, (bool, Stopwatch, ReactiveProperty<long>)> runningStopwatches = new();

		#region 외부 사용 인터페이스

		/// <summary>
		/// 스톱워치 시작 (키에 해당하는 스톱워치가 있으면 재활용, 없으면 새로 생성)
		/// </summary>
		/// <param name="stopwatchName">스톱워치 이름 (대소문자 구분)</param>
		public void StartStopwatch(string stopwatchName)
		{
			var hashKey = stopwatchName.GetHashCode();
			
			if (runningStopwatches.ContainsKey(hashKey))
			{
				Logger.Log(LogPriority.Error, $"{stopwatchName} 스톱워치는 이미 실행중입니다. 정지 후 사용해주세요.");
				
				return;
			}

			ActivateStopwatch(hashKey, out var stopwatch);

			runningStopwatches.Add(stopwatchName.GetHashCode(), (false, stopwatch, new ReactiveProperty<long>(0)));

			stopwatch.Start();
		}

		/// <summary>
		/// 스톱워치 일시정지 (Start 혹은 Resume 상태일 때 사용가능)
		/// </summary>
		/// <param name="stopwatchName">스톱워치 이름 (대소문자 구분)</param>
		public void PauseStopwatch(string stopwatchName)
		{
			var hashKey = stopwatchName.GetHashCode();

			if (TryGetActivatedStopwatch(hashKey, out var isPause, out var stopwatch))
			{
				if (isPause)
				{
					Logger.Log(LogPriority.Error, $"{stopwatchName} 스톱워치가 이미 일시정지 상태입니다.");

					return;
				}

				stopwatch.Stop();

				SetActivatedStopwatchPauseStatus(hashKey, true);
			}
			else
			{
				Logger.Log(LogPriority.Error, $"{stopwatchName} 스톱워치가 실행중이지 않습니다.");
			}
		}

		/// <summary>
		/// 스톱워치 재개 (Pause 상태일 때만 사용가능)
		/// </summary>
		/// <param name="stopwatchName">스톱워치 이름 (대소문자 구분)</param>
		public void ResumeStopwatch(string stopwatchName)
		{
			var hashKey = stopwatchName.GetHashCode();

			if (TryGetActivatedStopwatch(hashKey, out var isPause, out var stopwatch))
			{
				if (isPause)
				{
					stopwatch.Start();

					SetActivatedStopwatchPauseStatus(hashKey, false);
				}
				else
				{
					Logger.Log(LogPriority.Error, $"{stopwatchName} 스톱워치가 이미 실행중입니다. 일시정지 후 사용해주세요.");
				}
			}
			else
			{
				Logger.Log(LogPriority.Error, $"{stopwatchName} 스톱워치가 실행중이지 않습니다.");
			}
		}

		/// <summary>
		/// 스톱워치 중지
		/// </summary>
		/// <param name="stopwatchName">스톱워치 이름 (대소문자 구분)</param>
		/// <returns>스톱워치의 현재까지 지난 시간</returns>
		public long StopStopwatch(string stopwatchName)
		{
			return DeactivateStopwatch(stopwatchName.GetHashCode());
		}

		/// <summary>
		/// 모든 스톱워치 중지
		/// </summary>
		/// <returns>작동하던 스톱워치들의 현재까지 지난 시간</returns>
		public IEnumerable<long> StopAllStopwatch()
		{
			return runningStopwatches
				.Select(runningStopwatch =>
					DeactivateStopwatch(runningStopwatch.Key));
		}

		/// <summary>
		/// 현재 작동하고 있는 스톱워치의 현재 지난 시간 반환 (일회성)
		/// </summary>
		/// <param name="stopwatchName">스톱워치 이름</param>
		/// <returns>스톱워치의 현재까지 지난 시간</returns>
		public long CurrentTime(string stopwatchName)
		{
			if (TryGetActivatedStopwatch(stopwatchName.GetHashCode(), out var isPause, out var stopwatch))
			{
				return stopwatch.ElapsedMilliseconds;
			}

			Logger.Log(LogPriority.Error, $"{stopwatchName} 이름의 스톱워치가 작동중이지 않습니다.");

			return 0;
		}

		/// <summary>
		/// 작동중인 스톱워치의 시간값을 실시간으로 받아옴 [반드시 StartStopwatch를 호출한 후 호출할 것 (타이밍이 맞지 않아 정상동작 하지 않을 수 있음)]
		/// </summary>
		/// <param name="stopwatchName">스톱워치 이름</param>
		/// <param name="subscriberAction">스트림을 구독할 콜백 액션</param>
		public void SubscribeStopwatchTimeUpdate(string stopwatchName, System.Action<long> subscriberAction)
		{
			var hashKey = stopwatchName.GetHashCode();
			
			if (runningStopwatches.ContainsKey(hashKey))
			{
				var st = runningStopwatches[hashKey];

				st.Item3.Dispose();
				st.Item3 = new ReactiveProperty<long>(st.Item2.ElapsedMilliseconds);

				Observable.FromMicroCoroutine(() => PropertyUpdater(st.Item2, st.Item3, subscriberAction)).Subscribe();
			}
		}

		#endregion

		#region 내부 사용 메서드

		private void ActivateStopwatch(int key, out Stopwatch stopwatch)
		{
			if (stopwatches.ContainsKey(key))
			{
				if (stopwatches[key] is not null)
				{
					stopwatch = stopwatches[key];
					stopwatch.Reset();

					stopwatches[key] = null;
				}
				else
				{
					stopwatch = null;
				}

				return;
			}

			Logger.Log(LogPriority.Information, $"{key} 이름을 갖고 있는 스톱워치가 없어, 새로 생성합니다.");

			var st = new Stopwatch();
			st.Reset();

			stopwatch = st;
		}

		private long DeactivateStopwatch(int key)
		{
			long time;

			if (runningStopwatches.ContainsKey(key))
			{
				var st = runningStopwatches[key];

				runningStopwatches.Remove(key);

				st.Item2.Stop();

				time = st.Item2.ElapsedMilliseconds;

				stopwatches[key] = st.Item2;
			}
			else
			{
				Logger.Log(LogPriority.Error, $"{key} 스톱워치가 실행되지 않았습니다. Start 후 사용해주세요.");

				time = 0;
			}

			return time;
		}

		private bool TryGetActivatedStopwatch(int key, out bool isPause, out Stopwatch runningStopwatch)
		{
			if (runningStopwatches.ContainsKey(key))
			{
				isPause = runningStopwatches[key].Item1;
				runningStopwatch = runningStopwatches[key].Item2;

				return true;
			}

			isPause = false;
			runningStopwatch = null;

			return false;
		}

		private void SetActivatedStopwatchPauseStatus(int key, bool isPause)
		{
			var st = runningStopwatches[key];

			st.Item1 = isPause;

			runningStopwatches[key] = st;
		}

		private static IEnumerator PropertyUpdater(Stopwatch st, IReactiveProperty<long> rp, System.Action<long> sa)
		{
			rp.Subscribe(sa);
			
			while (true)
			{
				if (st.IsRunning is false)
				{
					yield return null;
					
					continue;
				}
				
				rp.Value = st.ElapsedMilliseconds;

				yield return null;
			}
			// ReSharper disable once IteratorNeverReturns
		}

		#endregion
	}
}