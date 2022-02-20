using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtension
{
	public static void DelayAction(this MonoBehaviour runner, Action callback, float delay)
	{
		runner.StartCoroutine(DelayRoutine(callback, delay));
	}

	static IEnumerator DelayRoutine(Action callback, float delay)
	{
		yield return new WaitForSeconds(delay);

		callback.Invoke();
	}
}